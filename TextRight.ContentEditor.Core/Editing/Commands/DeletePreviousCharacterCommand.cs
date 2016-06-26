﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TextRight.ContentEditor.Core.Editing.Actions;
using TextRight.ContentEditor.Core.ObjectModel.Blocks;

namespace TextRight.ContentEditor.Core.Editing
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
      var cursor = context.Cursor;
      return cursor.Is<TextBlockCursor>() && !cursor.IsAtBeginning;
    }

    /// <inheritdoc />
    public void Activate(DocumentEditorContext context, ActionStack actionStack)
    {
      using (var copy = context.Cursor.Copy())
      {
        var textCursor = (TextBlockCursor)copy.Cursor;
        actionStack.Do(new UndoableAction(textCursor));
      }
    }

    /// <summary> Deletes text from the document. </summary>
    public class UndoableAction : IUndoableAction
    {
      private readonly string _originalText;
      private readonly DocumentCursorHandle _cursorHandle;

      public UndoableAction(TextBlockCursor cursor)
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
}