using System;
using System.Collections.Generic;
using System.Linq;
using TextRight.Core.ObjectModel.Cursors;

namespace TextRight.Core.Editing.Commands.Caret
{
  /// <summary> Moves the caret to the beginning of the line. </summary>
  public class MoveCaretHomeCommand : CaretCommand
  {
    /// <inheritdoc />
    public override string Id
      => "caret.moveHome";

    /// <inheritdoc />
    protected override bool ShouldPreserveCaretMovementMode
      => true;

    /// <inheritdoc/>
    public override bool Activate(DocumentCursor cursor, CaretMovementMode movementMode)
    {
      using (var current = cursor.Cursor.Copy())
      {
        BlockCursorMover.BackwardMover.MoveCaretTowardsLineEdge(current.Cursor);
        cursor.MoveTo(current);
        movementMode.SetModeToHome();
        return true;
      }
    }
  }
}