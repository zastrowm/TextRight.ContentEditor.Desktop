using System;
using System.Collections.Generic;
using System.Linq;
using TextRight.ContentEditor.Core.ObjectModel.Blocks;

namespace TextRight.ContentEditor.Core.Editing.Actions
{
  /// <summary> Inserts text at the specified location. </summary>
  public class InsertTextAction : IAction
  {
    private readonly DocumentCursorHandle _insertionPoint;
    private readonly string _text;

    /// <summary> Constructor. </summary>
    /// <param name="insertionPoint"> The point at which text should be inserted. </param>
    /// <param name="text"> The text to insert. </param>
    public InsertTextAction(DocumentCursorHandle insertionPoint, string text)
    {
      _insertionPoint = insertionPoint;
      _text = text;
    }

    /// <inheritdoc />
    public void Do(DocumentEditorContext context)
    {
      var textBlockCursor = GetTextCursor(context);
      textBlockCursor.InsertText(_text);

      context.Caret.MoveTo(textBlockCursor);
    }

    /// <inheritdoc />
    public void Undo(DocumentEditorContext context)
    {
      var textBlockCursor = GetTextCursor(context);

      // TODO delete more than one at a time
      for (int i = 0; i < _text.Length; i++)
      {
        textBlockCursor.DeleteText(1);
      }

      context.Caret.MoveTo(textBlockCursor);
    }

    /// <summary> Turn the cursor handle into a text cursor. </summary>
    private TextBlock.TextBlockCursor GetTextCursor(DocumentEditorContext context)
    {
      var cursor = _insertionPoint.Get(context);
      var textBlockCursor = (TextBlock.TextBlockCursor)cursor.BlockCursor;
      return textBlockCursor;
    }
  }
}