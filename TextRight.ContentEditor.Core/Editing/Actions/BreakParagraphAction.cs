using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TextRight.ContentEditor.Core.Editing.Actions
{
  /// <summary> Breaks a paragraph at the given caret location. </summary>
  public class BreakParagraphAction : IAction
  {
    private readonly DocumentCursorHandle _handle;

    /// <summary> Constructor. </summary>
    /// <param name="handle"> The location at which the paragraph should be broken. </param>
    public BreakParagraphAction(DocumentCursorHandle handle)
    {
      _handle = handle;
    }

    /// <inheritdoc />
    public void Do(DocumentEditorContext context)
    {
      var documentCursor = _handle.Get(context);
      var blockCollection = documentCursor.BlockCursor.Block.Parent;

      var newBlock = blockCollection.TryBreakBlock(context.Cursor);
      if (newBlock == null)
        return;

      var cursor = newBlock.GetCursor();
      cursor.MoveToBeginning();
      context.Caret.MoveTo(cursor);
    }

    /// <inheritdoc />
    public void Undo(DocumentEditorContext context)
    {
      var documentCursor = _handle.Get(context);
      var previousBlock = documentCursor.BlockCursor.Block;
      var nextBlock = previousBlock.NextBlock;

      nextBlock.Parent.MergeWithPrevious(nextBlock);

      // move it to where it was when we wanted to break the paragraph.  It's safer to deserialize
      // again, as the cursor above is not guaranteed to be valid. 
      context.Caret.MoveTo(_handle.Get(context).BlockCursor);
    }
  }
}