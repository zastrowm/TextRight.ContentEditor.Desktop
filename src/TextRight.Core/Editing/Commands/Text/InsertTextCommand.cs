﻿using System;
using System.Collections.Generic;
using System.Linq;
using TextRight.Core.Actions;
using TextRight.Core.ObjectModel.Blocks.Text;
using TextRight.Core.ObjectModel.Cursors;

namespace TextRight.Core.Commands.Text
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
      // TODO what else to check
      return context.Selection.Start.TryCast(out TextCaret textCaret);
    }

    /// <inheritdoc />
    void IContextualCommand<string>.Activate(DocumentEditorContext context, IActionStack actionStack, string text)
    {
      // TODO delete any text that is selected
      actionStack.Do(new InsertTextUndoableAction(context.Selection, text));
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
        var textCaret = _insertionPoint.GetCaret(context).As<TextCaret>();
        textCaret = textCaret.InsertText(Text);
        context.Selection.Replace(textCaret);
      }

      /// <inheritdoc />
      public override void Undo(DocumentEditorContext context)
      {
        // TODO graphemes?
        var textCaret = (TextCaret)_insertionPoint.GetCaret(context);
        textCaret = textCaret.DeleteText(Text.Length);
        context.Selection.Replace(textCaret);
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
        var myCopy = (TextCaret)_insertionPoint.GetCaret(context);
        var otherCopy = (TextCaret)action._insertionPoint.GetCaret(context);

        if (myCopy.Block != otherCopy.Block)
          return false;

        if (myCopy.Offset.GraphemeOffset + Text.Length != otherCopy.Offset.GraphemeOffset)
          return false;

        Text += action.Text;

        return true;
      }

      private bool TryMergeWith(DocumentEditorContext context, DeletePreviousCharacterCommand.DeletePreviousCharacterAction action)
      {
        var myCopy = (TextCaret)_insertionPoint.GetCaret(context);
        var otherCopy = (TextCaret)action.CursorHandle.GetCaret(context);

        if (Text.Length == 0)
          return false;

        if (!Text.EndsWith(action.OriginalText))
          return false;

        if (myCopy.Block != otherCopy.Block)
          return false;

        var insertionPointAfterInsert = myCopy.Offset.GraphemeOffset + Text.Length;
        var insertionPointAtDeletion = otherCopy.Offset.GraphemeOffset + action.OriginalText.Length;

        if (insertionPointAfterInsert != insertionPointAtDeletion)
          return false;

        Text = Text.Remove(Text.Length - action.OriginalText.Length, action.OriginalText.Length);
        return true;
      }

      private bool TryMergeWith(DocumentEditorContext context, DeleteNextCharacterCommand.DeleteNextCharacterAction action)
      {
        return false;
      }
    }
  }
}