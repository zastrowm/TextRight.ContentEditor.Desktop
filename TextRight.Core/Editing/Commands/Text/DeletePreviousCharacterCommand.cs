using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TextRight.Core.Editing.Actions;
using TextRight.Core.Editing.Actions.Text;
using TextRight.Core.ObjectModel.Blocks.Text;

namespace TextRight.Core.Editing.Commands.Text
{
  /// <summary> Deletes the previous character in the document. </summary>
  public class DeletePreviousCharacterCommand : IContextualCommand
  {
    /// <inheritdoc />
    public string Id
      => "text.deletePreviousChar";

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
    public void Activate(DocumentEditorContext context, IActionStack actionStack)
    {
      using (var copy = context.Cursor.Copy())
      {
        var textCursor = (TextBlockCursor)copy.Cursor;
        actionStack.Do(new DeletePreviousCharacterAction(textCursor));
      }
    }

    /// <summary> Deletes text from the document. </summary>
    internal class DeletePreviousCharacterAction : UndoableAction
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
        using (var copy = CursorHandle.Get(context))
        {
          var cursor = (TextBlockCursor)copy.Cursor;
          cursor.MoveBackward();
          cursor.DeleteText(1);
          context.Caret.MoveTo(cursor);
        }
      }

      /// <inheritdoc />
      public override void Undo(DocumentEditorContext context)
      {
        using (var copy = CursorHandle.Get(context))
        {
          var cursor = (TextBlockCursor)copy.Cursor;
          cursor.MoveBackward();
          cursor.InsertText(OriginalText);
          cursor.MoveForward();
          context.Caret.MoveTo(cursor);
        }
      }
    }
  }
}