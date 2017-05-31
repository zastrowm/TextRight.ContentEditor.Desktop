using System;
using System.Collections.Generic;
using System.Linq;
using TextRight.Core.ObjectModel.Cursors;

namespace TextRight.Core.Editing.Commands.Caret
{
  /// <summary> Moves the caret to the end of the current line. </summary>
  public class MoveCaretEndCommand : CaretCommand
  {
    /// <inheritdoc />
    public override string Id
      => "caret.moveEnd";

    /// <inheritdoc />
    protected override bool ShouldPreserveCaretMovementMode
      => true;

    /// <inheritdoc/>
    public override bool Activate(DocumentCursor cursor, CaretMovementMode movementMode)
    {
      using (var current = cursor.Cursor.Copy())
      {
        BlockCursorMover.ForwardMover.MoveCaretTowardsLineEdge(current.Cursor);
        cursor.MoveTo(current);
        movementMode.SetModeToEnd();
        return true;
      }
    }
  }
}