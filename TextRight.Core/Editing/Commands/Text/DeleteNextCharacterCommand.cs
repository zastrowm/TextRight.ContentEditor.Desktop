using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TextRight.Core.Editing.Actions;
using TextRight.Core.Editing.Actions.Text;
using TextRight.Core.ObjectModel.Blocks.Text;

namespace TextRight.Core.Editing.Commands.Text
{
  /// <summary> Deletes the next character in the document. </summary>
  public class DeleteNextCharacterCommand : IContextualCommand
  {
    /// <inheritdoc />
    public string Id
      => "text.deleteNextChar";

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
    public void Activate(DocumentEditorContext context, IActionStack actionStack)
    {
      using (var copy = context.Cursor.Copy())
      {
        var textCursor = (TextBlockCursor)copy.Cursor;
        actionStack.Do(new DeleteNextCharacterAction(textCursor));
      }
    }

    /// <summary> Deletes text from the document. </summary>
    internal class DeleteNextCharacterAction : UndoableAction
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
}