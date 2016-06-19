using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TextRight.ContentEditor.Core.Editing.Actions;
using TextRight.ContentEditor.Core.ObjectModel.Blocks;

namespace TextRight.ContentEditor.Core.Editing
{
  /// <summary> Deletes the next character in the document. </summary>
  public class DeleteNextCharacterCommand : IContextualCommand
  {
    /// <inheritdoc />
    public string GetName(DocumentEditorContext context)
      => "Delete character";

    /// <inheritdoc />
    public string GetDescription(DocumentEditorContext context)
      => "Deletes the next character in the document";

    /// <inheritdoc />
    public bool CanActivate(DocumentEditorContext context)
    {
      var cursor = context.Cursor;
      return cursor.Is<TextBlockCursor>() && !cursor.IsAtEnd;
    }

    /// <inheritdoc />
    public void Activate(DocumentEditorContext context, ActionStack actionStack)
    {
      var textCursor = (TextBlockCursor)context.BlockCursor;
      actionStack.Do(new UndableAction(textCursor));
    }

    /// <summary> Deletes text from the document. </summary>
    public class UndableAction : IUndoableAction
    {
      private readonly string _originalText;
      private readonly DocumentCursorHandle _cursorHandle;

      public UndableAction(TextBlockCursor cursor)
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
}