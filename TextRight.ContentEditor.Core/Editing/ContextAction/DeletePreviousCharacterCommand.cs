using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TextRight.ContentEditor.Core.ObjectModel.Blocks;

namespace TextRight.ContentEditor.Core.Editing.Actions
{
  /// <summary> Deletes the previous character in the document. </summary>
  public class DeletePreviousCharacterCommand : IContextualCommand
  {
    /// <inheritdoc />
    public string GetName(DocumentEditorContext context)
      => "Delete previous character";

    /// <inheritdoc />
    public string GetDescription(DocumentEditorContext context)
      => "Deletes the previous character in the document";

    /// <inheritdoc />
    public bool CanActivate(DocumentEditorContext context)
    {
      var textCursor = context.Cursor as TextBlock.TextBlockCursor;
      if (textCursor == null)
        return false;

      var state = textCursor.State;
      if (!textCursor.MoveBackward())
        return false;

      textCursor.State = state;

      return true;
    }

    /// <inheritdoc />
    public void Activate(DocumentEditorContext context, ActionStack actionStack)
    {
      var textCursor = (TextBlock.TextBlockCursor)context.Cursor;
      actionStack.Do(new UndableAction(textCursor));
    }

    /// <summary> Deletes text from the document. </summary>
    internal class UndableAction : IUndoableAction
    {
      private readonly string _originalText;
      private readonly DocumentCursorHandle _cursorHandle;

      public UndableAction(TextBlock.TextBlockCursor cursor)
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
        var cursor = (TextBlock.TextBlockCursor)_cursorHandle.Get(context);
        cursor.MoveBackward();
        cursor.DeleteText(1);
        context.Caret.MoveTo(cursor);
      }

      /// <inheritdoc />
      public void Undo(DocumentEditorContext context)
      {
        var cursor = (TextBlock.TextBlockCursor)_cursorHandle.Get(context);
        cursor.MoveBackward();
        cursor.InsertText(_originalText);
        cursor.MoveForward();
        context.Caret.MoveTo(cursor);
      }
    }
  }
}