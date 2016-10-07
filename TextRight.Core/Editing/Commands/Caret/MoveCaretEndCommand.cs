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
    public override bool Activate(DocumentCursor caret, CaretMovementMode movementMode)
    {
      using (var current = caret.Cursor.Copy())
      {
        BlockCursorMover.ForwardMover.MoveCaretTowardsLineEdge(current.Cursor);
        caret.MoveTo(current);
        movementMode.SetModeToEnd();
        return true;
      }
    }
  }
}