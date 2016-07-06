using System;
using System.Collections.Generic;
using System.Linq;
using TextRight.ContentEditor.Core.ObjectModel.Blocks;
using TextRight.ContentEditor.Core.ObjectModel.Cursors;

namespace TextRight.ContentEditor.Core.Editing
{
  /// <summary> Moves the caret backwards. </summary>
  public class MoveCaretForwardCommand : CaretCommand
  {
    /// <inheritdoc />
    public override bool Activate(DocumentCursor caret, CaretMovementMode movementMode)
    {
      using (var current = caret.Cursor.Copy())
      {
        // first we try to move the cursor directly
        var blockCursor = current.Cursor;
        if (blockCursor.MoveForward())
        {
          caret.MoveTo(blockCursor);
          return true;
        }

        // try get the block backward to get a cursor that looks at the beginning of that block
        var block = blockCursor.Block?.Parent.GetBlockTo(BlockDirection.Forward, blockCursor.Block) as ContentBlock;
        if (block != null)
        {
          using (var newCursor = block.CursorPool.GetCursorCopy(block))
          {
            newCursor.Cursor.MoveToBeginning();
            caret.MoveTo(newCursor.Cursor);
            return true;
          }
        }

        // we couldn't do it
        return false;
      }
    }
  }
}