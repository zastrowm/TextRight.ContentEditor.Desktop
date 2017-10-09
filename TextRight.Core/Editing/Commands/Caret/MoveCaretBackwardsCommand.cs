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
    public override bool Activate(DocumentSelection cursor, CaretMovementMode movementMode)
    {
      var caret = cursor.Start;
      var next = caret.MoveBackward();

      // try a simply to move the cursor backwards
      if (next.IsValid)
      {
        cursor.MoveTo(next);
        return true;
      }

      // try get the block backward to get a cursor that looks at the end of that block
      var block = caret.Block?.Parent.GetBlockTo(BlockDirection.Backward, caret.Block) as ContentBlock;
      if (block != null)
      {
        cursor.MoveTo(block.GetCaretAtEnd());
        return true;
      }

      // we couldn't do it
      return false;
    }
  }
}