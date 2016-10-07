using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TextRight.Core.ObjectModel.Blocks;
using TextRight.Core.ObjectModel.Cursors;

namespace TextRight.Core.Editing.Commands.Caret
{
  /// <summary> Moves the caret backwards. </summary>
  public class MoveCaretBackwardCommand : CaretCommand
  {
    /// <inheritdoc />
    public override string Id
      => "caret.moveBackward";

    /// <inheritdoc />
    public override bool Activate(DocumentCursor caret, CaretMovementMode movementMode)
    {
      using (var current = caret.Cursor.Copy())
      {
        var blockCursor = current.Cursor;
        // try a simply to move the cursor backwards
        if (blockCursor.MoveBackward())
        {
          caret.MoveTo(blockCursor);
          return true;
        }

        // try get the block backward to get a cursor that looks at the end of that block
        var block = blockCursor.Block?.Parent.GetBlockTo(BlockDirection.Backward, blockCursor.Block) as ContentBlock;
        if (block != null)
        {
          using (var newCursor = block.CursorPool.GetCursorCopy(block))
          {
            newCursor.Cursor.MoveToEnd();
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