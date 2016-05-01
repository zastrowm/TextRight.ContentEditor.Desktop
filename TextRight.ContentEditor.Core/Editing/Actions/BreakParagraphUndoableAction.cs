using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TextRight.ContentEditor.Core.ObjectModel.Blocks;

namespace TextRight.ContentEditor.Core.Editing.Actions
{
  /// <summary> Breaks a paragraph at the given caret location. </summary>
  public class BreakParagraphUndoableAction : IUndoableAction
  {
    private readonly DocumentCursorHandle _handle;

    /// <summary> Constructor. </summary>
    /// <param name="handle"> The location at which the paragraph should be broken. </param>
    public BreakParagraphUndoableAction(DocumentCursorHandle handle)
    {
      _handle = handle;
    }

    /// <inheritdoc />
    public string Name { get; }
      = "Break Paragraph";

    /// <inheritdoc />
    public string Description { get; }
      = "Break paragraph into two";

    /// <inheritdoc />
    public void Do(DocumentEditorContext context)
    {
      var blockCursor = _handle.Get(context);
      var blockCollection = blockCursor.Block.Parent;

      var newBlock = blockCollection.TryBreakBlock(context.Cursor);
      if (newBlock == null)
        return;

      context.Caret.MoveTo(newBlock.GetCursor().ToBeginning());
    }

    /// <inheritdoc />
    public void Undo(DocumentEditorContext context)
    {
      var blockCursor = _handle.Get(context);
      var previousBlock = blockCursor.Block;
      var nextBlock = previousBlock.NextBlock;

      nextBlock.Parent.MergeWithPrevious(nextBlock);

      // move it to where it was when we wanted to break the paragraph.  It's safer to deserialize
      // again, as the cursor above is not guaranteed to be valid. 
      context.Caret.MoveTo(_handle.Get(context));
    }
  }
}