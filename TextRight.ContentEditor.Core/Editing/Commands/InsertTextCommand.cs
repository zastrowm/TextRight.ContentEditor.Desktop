using System;
using System.Collections.Generic;
using System.Linq;
using TextRight.ContentEditor.Core.ObjectModel.Blocks;

namespace TextRight.ContentEditor.Core.Editing.Commands
{
  /// <summary> Allows text to be inserted into document. </summary>
  public struct InsertTextCommand : IActionCommand
  {
    private readonly string _textToInsert;

    public InsertTextCommand(string textToInsert)
    {
      _textToInsert = textToInsert;
    }

    /// <summary> Executes the given context. </summary>
    /// <param name="context"> The context to which the text should be added. </param>
    public void Execute(DocumentEditorContext context)
    {
      var textCursor = context.Caret.BlockCursor as ITextContentCursor;
      if (textCursor?.CanInsertText() != true)
        return;

      textCursor.InsertText(_textToInsert);
    }
  }
}