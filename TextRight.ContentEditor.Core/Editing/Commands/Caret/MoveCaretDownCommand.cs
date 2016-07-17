using System;
using System.Collections.Generic;
using System.Linq;
using TextRight.ContentEditor.Core.ObjectModel.Blocks;
using TextRight.ContentEditor.Core.ObjectModel.Cursors;

namespace TextRight.ContentEditor.Core.Editing
{
  /// <summary> Moves the caret down in the document. </summary>
  public class MoveCaretDownCommand : CaretCommand
  {
    /// <inheritdoc />
    public override string Id
      => "caret.moveDown";

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

        bool didMove = BlockCursorMover.ForwardMover.MoveCaretTowardsPositionInNextLine(current.Cursor, movementMode);

        if (didMove)
        {
          caret.MoveTo(current.Cursor);
          return true;
        }

        // TODO work the way up the document
        var previousBlock = current.Block.Parent.GetBlockTo(BlockDirection.Bottom, current.Block);
        if (previousBlock != null)
        {
          var newCursor = previousBlock.GetCaretFromTop(movementMode);
          caret.MoveTo(newCursor);

          return true;
        }

        // we couldn't do it
        return false;
      }
    }
  }
}