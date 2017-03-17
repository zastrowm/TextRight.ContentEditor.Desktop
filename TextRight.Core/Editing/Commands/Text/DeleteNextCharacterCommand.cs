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
  }
}