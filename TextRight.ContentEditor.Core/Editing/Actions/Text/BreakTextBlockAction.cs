using System;
using System.Collections.Generic;
using System.Linq;
using TextRight.ContentEditor.Core.ObjectModel.Blocks;

namespace TextRight.ContentEditor.Core.Editing.Actions
{
  /// <summary> Breaks a paragraph at the given caret location. </summary>
  public class BreakTextBlockAction : UndoableAction
  {
    private readonly DocumentCursorHandle _handle;

    /// <summary> Constructor. </summary>
    /// <param name="handle"> The location at which the paragraph should be broken. </param>
    public BreakTextBlockAction(DocumentCursorHandle handle)
    {
      _handle = handle;
    }

    /// <inheritdoc />
    public override string Name { get; }
      = "Break Paragraph";

    /// <inheritdoc />
    public override string Description { get; }
      = "Break paragraph into two";

    /// <inheritdoc />
    public override void Do(DocumentEditorContext context)
    {
      using (var copy = _handle.Get(context))
      {
        var blockCursor = copy.Cursor;
        var blockCollection = blockCursor.Block.Parent;

        var newBlock = blockCollection.TryBreakBlock(blockCursor);
        if (newBlock == null)
          return;

        context.Caret.MoveTo(newBlock.GetCursor().ToBeginning());
      }
      
    }

    /// <inheritdoc />
    public override void Undo(DocumentEditorContext context)
    {
      using (var copy = _handle.Get(context))
      {
        var blockCursor = copy.Cursor;
        var previousBlock = blockCursor.Block;
        var nextBlock = previousBlock.NextBlock;

        nextBlock.Parent.MergeWithPrevious(nextBlock);

        // move it to where it was when we wanted to break the paragraph.  It's safer to deserialize
        // again, as the cursor above is not guaranteed to be valid. 
        context.Caret.MoveTo(_handle.Get(context));
      }
    }
  }
}