using System;
using System.Collections.Generic;
using System.Linq;
using TextRight.Core.ObjectModel.Blocks;
using TextRight.Core.ObjectModel.Cursors;

namespace TextRight.Core.Editing.Commands.Caret
{
  /// <summary> Moves the caret backwards. </summary>
  public class MoveCaretUpCommand : CaretCommand
  {
    /// <inheritdoc />
    public override string Id
      => "caret.moveUp";

    /// <inheritdoc />
    protected override bool ShouldPreserveCaretMovementMode
      => true;

    /// <inheritdoc />
    public override bool Activate(DocumentCursor cursor, CaretMovementMode movementMode)
    {
      using (var current = cursor.Cursor.Copy())
      {
        if (movementMode.CurrentMode == CaretMovementMode.Mode.None)
        {
          movementMode.SetModeToPosition(current.MeasureCursorPosition().Left);
        }

        bool didMove = BlockCursorMover.BackwardMover.MoveCaretTowardsPositionInNextLine(current.Cursor, movementMode);

        if (didMove)
        {
          cursor.MoveTo(current.Cursor);
          return true;
        }

        // TODO work the way up the document
        var previousBlock = current.Block.Parent.GetBlockTo(BlockDirection.Top, current.Block);
        if (previousBlock != null)
        {
          var newCursor = previousBlock.GetCaretFromBottom(movementMode);
          cursor.MoveTo(newCursor);

          return true;
        }

        // we couldn't do it
        return false;
      }
    }
  }
}