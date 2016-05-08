using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TextRight.ContentEditor.Core.ObjectModel.Blocks;

namespace TextRight.ContentEditor.Core.Editing.Actions
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
      var textCursor = context.Cursor as TextBlock.TextBlockCursor;
      return textCursor?.IsAtEnd == false;
    }

    /// <inheritdoc />
    public void Activate(DocumentEditorContext context, ActionStack actionStack)
    {
      var textCursor = (TextBlock.TextBlockCursor)context.Cursor;
      actionStack.Do(new UndableAction(textCursor));
    }

    /// <summary> Deletes text from the document. </summary>
    public class UndableAction : IUndoableAction
    {
      private readonly string _originalText;
      private readonly DocumentCursorHandle _cursorHandle;

      public UndableAction(TextBlock.TextBlockCursor cursor)
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
        var cursor = (TextBlock.TextBlockCursor)_cursorHandle.Get(context);
        cursor.DeleteText(1);
      }

      /// <inheritdoc />
      public void Undo(DocumentEditorContext context)
      {
        var cursor = (TextBlock.TextBlockCursor)_cursorHandle.Get(context);
        cursor.InsertText(_originalText);
      }
    }
  }
}