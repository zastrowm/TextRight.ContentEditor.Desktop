using System;
using System.Collections.Generic;
using System.Linq;
using TextRight.ContentEditor.Core.ObjectModel.Blocks;

namespace TextRight.ContentEditor.Core.Editing.Actions
{
  /// <summary> Deletes text from the document. </summary>
  public class DeletePreviousCharacterAction : IUndoableAction
  {
    private readonly string _originalText;
    private readonly DocumentCursorHandle _cursorHandle;

    public DeletePreviousCharacterAction(TextBlockCursor cursor)
    {
      _cursorHandle = new DocumentCursorHandle(cursor);
      _originalText = cursor.CharacterBefore.ToString();
    }

    /// <inheritdoc />
    public string Name
      => "Delete Text";

    /// <inheritdoc />
    public string Description
      => "Delete text from the document";

    /// <inheritdoc />
    public void Do(DocumentEditorContext context)
    {
      var cursor = (TextBlockCursor)_cursorHandle.Get(context);
      cursor.MoveBackward();
      cursor.DeleteText(1);
      context.Caret.MoveTo(cursor);
    }

    /// <inheritdoc />
    public void Undo(DocumentEditorContext context)
    {
      var cursor = (TextBlockCursor)_cursorHandle.Get(context);
      cursor.MoveBackward();
      cursor.InsertText(_originalText);
      cursor.MoveForward();
      context.Caret.MoveTo(cursor);
    }
  }
}