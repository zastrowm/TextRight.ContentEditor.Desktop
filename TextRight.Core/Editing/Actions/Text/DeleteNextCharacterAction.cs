using System;
using System.Collections.Generic;
using System.Linq;
using TextRight.Core.ObjectModel.Blocks.Text;

namespace TextRight.Core.Editing.Actions.Text
{
  /// <summary> Deletes text from the document. </summary>
  public class DeleteNextCharacterAction : UndoableAction
  {
    private readonly string _originalText;
    private readonly DocumentCursorHandle _cursorHandle;

    public DeleteNextCharacterAction(TextBlockCursor cursor)
    {
      _cursorHandle = new DocumentCursorHandle(cursor);
      _originalText = cursor.CharacterAfter.ToString();
    }

    /// <inheritdoc />
    public override string Name
      => "Delete Text";

    /// <inheritdoc />
    public override string Description
      => "Delete text from the document";

    /// <inheritdoc />
    public override void Do(DocumentEditorContext context)
    {
      using (var copy = _cursorHandle.Get(context))
      {
        var cursor = (TextBlockCursor)copy.Cursor;
        cursor.DeleteText(1);
      }
    }

    /// <inheritdoc />
    public override void Undo(DocumentEditorContext context)
    {
      using (var copy = _cursorHandle.Get(context))
      {
        var cursor = (TextBlockCursor)copy.Cursor;
        cursor.InsertText(_originalText);
      }
    }
  }
}