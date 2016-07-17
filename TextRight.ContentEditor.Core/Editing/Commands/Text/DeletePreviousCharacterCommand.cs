using System;
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
    public void Activate(DocumentEditorContext context, ActionStack actionStack)
    {
      using (var copy = context.Cursor.Copy())
      {
        var textCursor = (TextBlockCursor)copy.Cursor;
        actionStack.Do(new DeletePreviousCharacterAction(textCursor));
      }
    }
  }
}