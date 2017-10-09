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
      var cursor = context.Selection.Start;
      return cursor.Is<TextCaret>() && !cursor.IsAtBlockEnd;
    }

    /// <inheritdoc />
    public void Activate(DocumentEditorContext context, IActionStack actionStack)
    {
      actionStack.Do(new DeleteNextCharacterAction((TextCaret)context.Selection.Start));
    }

    /// <summary> Deletes text from the document. </summary>
    internal class DeleteNextCharacterAction : UndoableAction
    {
      private readonly string _originalText;
      private readonly DocumentCursorHandle _cursorHandle;

      public DeleteNextCharacterAction(TextCaret caret)
      {
        _cursorHandle = new DocumentCursorHandle(caret);
        _originalText = caret.CharacterAfter.Text;
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
        var caret = (TextCaret)_cursorHandle.GetCaret(context);
        caret.DeleteText(1);
      }

      /// <inheritdoc />
      public override void Undo(DocumentEditorContext context)
      {
        var caret = (TextCaret)_cursorHandle.GetCaret(context);
        caret.InsertText(_originalText);
      }
    }
  }
}