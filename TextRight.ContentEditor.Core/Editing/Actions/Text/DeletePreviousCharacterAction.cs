using System;
using System.Collections.Generic;
using System.Linq;
using TextRight.ContentEditor.Core.ObjectModel.Blocks;

namespace TextRight.ContentEditor.Core.Editing.Actions
{
  /// <summary> Deletes text from the document. </summary>
  public class DeletePreviousCharacterAction : UndoableAction
  {
    public DeletePreviousCharacterAction(TextBlockCursor cursor)
    {
      CursorHandle = new DocumentCursorHandle(cursor);
      OriginalText = cursor.CharacterBefore.ToString();
    }

    /// <summary> The location at which the character was deleted. </summary>
    public DocumentCursorHandle CursorHandle { get; }

    /// <summary> The text that was removed. </summary>
    public string OriginalText { get; }

    /// <inheritdoc />
    public override string Name
      => "Delete Text";

    /// <inheritdoc />
    public override string Description
      => "Delete text from the document";

    /// <inheritdoc />
    public override void Do(DocumentEditorContext context)
    {
      var cursor = (TextBlockCursor)CursorHandle.Get(context);
      cursor.MoveBackward();
      cursor.DeleteText(1);
      context.Caret.MoveTo(cursor);
    }

    /// <inheritdoc />
    public override void Undo(DocumentEditorContext context)
    {
      var cursor = (TextBlockCursor)CursorHandle.Get(context);
      cursor.MoveBackward();
      cursor.InsertText(OriginalText);
      cursor.MoveForward();
      context.Caret.MoveTo(cursor);
    }
  }
}