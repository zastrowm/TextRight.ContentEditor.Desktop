using System;
using System.Collections.Generic;
using System.Linq;
using TextRight.ContentEditor.Core.ObjectModel.Blocks;

namespace TextRight.ContentEditor.Core.Editing.Actions
{
  /// <summary> Inserts text at the specified location. </summary>
  public class InsertTextUndoableAction : IUndoableAction
  {
    private readonly DocumentCursorHandle _insertionPoint;
    private readonly string _text;

    /// <summary> Constructor. </summary>
    /// <param name="insertionPoint"> The point at which text should be inserted. </param>
    /// <param name="text"> The text to insert. </param>
    public InsertTextUndoableAction(DocumentCursorHandle insertionPoint, string text)
    {
      _insertionPoint = insertionPoint;
      _text = text;
    }

    /// <inheritdoc />
    public string Name
      => "Insert Text";

    /// <inheritdoc />
    public string Description
      => "Insert text into the paragraph";

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
      textBlockCursor.DeleteText(_text.Length);
      context.Caret.MoveTo(textBlockCursor);
    }

    /// <summary> Turn the cursor handle into a text cursor. </summary>
    private TextBlockCursor GetTextCursor(DocumentEditorContext context)
    {
      return (TextBlockCursor)_insertionPoint.Get(context);
    }
  }
}