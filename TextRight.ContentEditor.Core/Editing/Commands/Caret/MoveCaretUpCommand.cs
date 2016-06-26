using System;
using System.Collections.Generic;
using System.Linq;
using TextRight.ContentEditor.Core.ObjectModel.Blocks;
using TextRight.ContentEditor.Core.ObjectModel.Cursors;

namespace TextRight.ContentEditor.Core.Editing
{
  /// <summary> Moves the caret backwards. </summary>
  public class MoveCaretUpCommand : CaretCommand
  {
    /// <inheritdoc />
    protected override bool ShouldPreserveCaretMovementMode
      => true;

    /// <inheritdoc />
    public override bool Activate(DocumentCursor caret, CaretMovementMode movementMode)
    {
      using (var current = caret.Cursor.Copy())
      {
        if (movementMode.CurrentMode == CaretMovementMode.Mode.None)
        {
          movementMode.SetModeToPosition(current.MeasureCursorPosition().Left);
        }

        bool didMove = BlockCursorMover.BackwardMover.MoveCaretTowardsPositionInNextLine(current.Cursor, movementMode);

        if (didMove)
        {
          caret.MoveTo(current.Cursor);
          return true;
        }

        // TODO work the way up the document
        var previousBlock = current.Block.Parent.GetBlockTo(BlockDirection.Top, current.Block);
        if (previousBlock != null)
        {
          var newCursor = previousBlock.GetCaretFromBottom(movementMode);
          caret.MoveTo(newCursor);

          return true;
        }

        // we couldn't do it
        return false;
      }
    }
  }
}