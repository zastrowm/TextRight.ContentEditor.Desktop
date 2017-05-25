using System;
using System.Collections.Generic;
using System.Linq;
using TextRight.Core.Editing.Actions;
using TextRight.Core.Editing.Actions.Text;
using TextRight.Core.ObjectModel.Blocks.Text;

namespace TextRight.Core.Editing.Commands.Text
{
  /// <summary> Inserts the provided text into the document. </summary>
  public class InsertTextCommand : IContextualCommand<string>
  {
    /// <inheritdoc />
    public string Id
      => "text.insert";

    /// <inheritdoc />
    string IContextualCommand<string>.GetName(DocumentEditorContext context) 
      => "Insert Text";

    /// <inheritdoc />
    string IContextualCommand<string>.GetDescription(DocumentEditorContext context) 
      => "Inserts text into the document at the current location";

    /// <inheritdoc />
    bool IContextualCommand<string>.CanActivate(DocumentEditorContext context, string text)
    {
      var textBlock = context.Cursor.Block as TextBlock;
      // TODO what else to check
      return textBlock != null;
    }

    /// <inheritdoc />
    void IContextualCommand<string>.Activate(DocumentEditorContext context, IActionStack actionStack, string text)
    {
      // TODO delete any text that is selected
      actionStack.Do(new InsertTextUndoableAction(context.Caret, text));
    }

    /// <summary> Inserts text at the specified location. </summary>
    internal class InsertTextUndoableAction : UndoableAction
    {
      /// <summary> Constructor. </summary>
      /// <param name="insertionPoint"> The point at which text should be inserted. </param>
      /// <param name="text"> The text to insert. </param>
      public InsertTextUndoableAction(DocumentCursorHandle insertionPoint, string text)
      {
        _insertionPoint = insertionPoint;
        Text = text;
      }

      /// <summary> The text that is inserted into the document. </summary>
      public string Text { get; private set; }

      /// <summary> The insertion point where the text is inserted. </summary>
      private readonly DocumentCursorHandle _insertionPoint;

      /// <inheritdoc />
      public override string Name
        => "Insert Text";

      /// <inheritdoc />
      public override string Description
        => "Insert text into the paragraph";

      /// <inheritdoc />
      public override void Do(DocumentEditorContext context)
      {
        var textCaret = _insertionPoint.GetCaret(context, TextCaret.Factory);
        textCaret = textCaret.InsertText(Text);
        context.Caret.MoveTo(textCaret);
      }

      /// <inheritdoc />
      public override void Undo(DocumentEditorContext context)
      {
        // TODO graphemes?
        var textCaret = (TextCaret)_insertionPoint.GetCaret(context);
        textCaret = textCaret.DeleteText(Text.Length);
        context.Caret.MoveTo(textCaret);
      }

      /// <inheritdoc/>
      public override bool TryMerge(DocumentEditorContext context, UndoableAction action)
      {
        if (action is InsertTextUndoableAction)
        {
          return TryMergeWith(context, (InsertTextUndoableAction)action);
        }
        else if (action is DeleteNextCharacterCommand.DeleteNextCharacterAction)
        {
          return TryMergeWith(context, (DeleteNextCharacterCommand.DeleteNextCharacterAction)action);
        }
        else if (action is DeletePreviousCharacterCommand.DeletePreviousCharacterAction)
        {
          return TryMergeWith(context, (DeletePreviousCharacterCommand.DeletePreviousCharacterAction)action);
        }

        return false;
      }

      private bool TryMergeWith(DocumentEditorContext context, InsertTextUndoableAction action)
      {
        using (var myCopy = _insertionPoint.Get(context))
        using (var otherCopy = action._insertionPoint.Get(context))
        {
          var myCursor = (TextBlockCursor)myCopy.Cursor;
          var otherCursor = (TextBlockCursor)otherCopy.Cursor;

          if (myCursor.Block != otherCursor.Block)
            return false;

          if (myCursor.Fragment != otherCursor.Fragment)
            return false;

          if (myCursor.OffsetIntoSpan + Text.Length != otherCursor.OffsetIntoSpan)
            return false;

          Text += action.Text;

          return true;
        }
      }

      private bool TryMergeWith(DocumentEditorContext context, DeletePreviousCharacterCommand.DeletePreviousCharacterAction action)
      {
        using (var myCopy = _insertionPoint.Get(context))
        using (var otherCopy = action.CursorHandle.Get(context))
        {
          var myCursor = (TextBlockCursor)myCopy.Cursor;
          var otherCursor = (TextBlockCursor)otherCopy.Cursor;

          if (Text.Length == 0)
            return false;

          if (!Text.EndsWith(action.OriginalText))
            return false;

          if (myCursor.Block != otherCursor.Block)
            return false;

          if (myCursor.Fragment != otherCursor.Fragment)
            return false;

          var insertionPointAfterInsert = myCursor.OffsetIntoSpan + Text.Length;
          var insertionPointAtDeletion = otherCursor.OffsetIntoSpan + action.OriginalText.Length;

          if (insertionPointAfterInsert != insertionPointAtDeletion)
            return false;

          Text = Text.Remove(Text.Length - action.OriginalText.Length, action.OriginalText.Length);
          return true;
        }
      }

      private bool TryMergeWith(DocumentEditorContext context, DeleteNextCharacterCommand.DeleteNextCharacterAction action)
      {
        return false;
      }
    }
  }
}