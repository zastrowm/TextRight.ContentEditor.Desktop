using System;
using System.Collections.Generic;
using System.Linq;
using TextRight.ContentEditor.Core.ObjectModel.Blocks;

namespace TextRight.ContentEditor.Core.Editing.Actions
{
  /// <summary> Inserts text at the specified location. </summary>
  public class InsertTextUndoableAction : UndoableAction
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
      var textBlockCursor = GetTextCursor(context);
      textBlockCursor.InsertText(Text);

      context.Caret.MoveTo(textBlockCursor);
    }

    /// <inheritdoc />
    public override void Undo(DocumentEditorContext context)
    {
      var textBlockCursor = GetTextCursor(context);
      textBlockCursor.DeleteText(Text.Length);
      context.Caret.MoveTo(textBlockCursor);
    }

    /// <summary> Turn the cursor handle into a text cursor. </summary>
    private TextBlockCursor GetTextCursor(DocumentEditorContext context)
    {
      return (TextBlockCursor)_insertionPoint.Get(context);
    }

    /// <inheritdoc/>
    public override bool TryMerge(DocumentEditorContext context, UndoableAction action)
    {
      if (action is InsertTextUndoableAction)
      {
        return TryMergeWith(context, (InsertTextUndoableAction)action);
      }
      else if (action is DeleteNextCharacterAction)
      {
        return TryMergeWith(context, (DeleteNextCharacterAction)action);
      }
      else if (action is DeletePreviousCharacterAction)
      {
        return TryMergeWith(context, (DeletePreviousCharacterAction)action);
      }

      return false;
    }

    private bool TryMergeWith(DocumentEditorContext context, InsertTextUndoableAction action)
    {
      var myCursor = (TextBlockCursor)_insertionPoint.Get(context);
      var otherCursor = (TextBlockCursor)action._insertionPoint.Get(context);

      if (myCursor.Block != otherCursor.Block)
        return false;

      if (myCursor.Fragment != otherCursor.Fragment)
        return false;

      if (myCursor.OffsetIntoSpan + Text.Length != otherCursor.OffsetIntoSpan)
        return false;

      Text += action.Text;

      return true;
    }

    private bool TryMergeWith(DocumentEditorContext context, DeletePreviousCharacterAction action)
    {
      var myCursor = (TextBlockCursor)_insertionPoint.Get(context);
      var otherCursor = (TextBlockCursor)action.CursorHandle.Get(context);

      if (Text.Length == 0)
        return false;

      if (!Text.EndsWith(action.OriginalText))
        return false;

      if (myCursor.Block != otherCursor.Block)
        return false;

      if (myCursor.Fragment != otherCursor.Fragment)
        return false;

      var insertionPointAfterInsert = myCursor.OffsetIntoSpan + Text.Length;
      var insertionPointAtDeletion = otherCursor.OffsetIntoSpan;

      if (insertionPointAfterInsert != insertionPointAtDeletion)
        return false;

      Text = Text.Remove(Text.Length - action.OriginalText.Length, action.OriginalText.Length);
      return true;
    }

    private bool TryMergeWith(DocumentEditorContext context, DeleteNextCharacterAction action)
    {
      return false;
    }
  }
}