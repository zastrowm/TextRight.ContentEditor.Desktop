using System;
using System.Collections.Generic;
using System.Linq;
using TextRight.ContentEditor.Core.ObjectModel.Cursors;

namespace TextRight.ContentEditor.Core.Editing
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
    public override bool Activate(DocumentCursor caret, CaretMovementMode movementMode)
    {
      using (var current = caret.Cursor.Copy())
      {
        BlockCursorMover.BackwardMover.MoveCaretTowardsLineEdge(current.Cursor);
        caret.MoveTo(current);
        movementMode.SetModeToHome();
        return true;
      }
    }
  }
}