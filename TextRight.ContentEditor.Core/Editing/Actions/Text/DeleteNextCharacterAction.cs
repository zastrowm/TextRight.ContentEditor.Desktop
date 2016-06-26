using System;
using System.Collections.Generic;
using System.Linq;
using TextRight.ContentEditor.Core.ObjectModel.Blocks;

namespace TextRight.ContentEditor.Core.Editing.Actions
{
  /// <summary> Deletes text from the document. </summary>
  public class DeleteNextCharacterAction : IUndoableAction
  {
    private readonly string _originalText;
    private readonly DocumentCursorHandle _cursorHandle;

    public DeleteNextCharacterAction(TextBlockCursor cursor)
    {
      _cursorHandle = new DocumentCursorHandle(cursor);
      _originalText = cursor.CharacterAfter.ToString();
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
      cursor.DeleteText(1);
    }

    /// <inheritdoc />
    public void Undo(DocumentEditorContext context)
    {
      var cursor = (TextBlockCursor)_cursorHandle.Get(context);
      cursor.InsertText(_originalText);
    }
  }
}