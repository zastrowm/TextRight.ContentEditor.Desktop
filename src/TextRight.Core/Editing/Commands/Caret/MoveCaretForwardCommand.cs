﻿using System;
using System.Collections.Generic;
using System.Linq;
using TextRight.Core.ObjectModel.Blocks;
using TextRight.Core.ObjectModel.Cursors;

namespace TextRight.Core.Commands.Caret
{
  /// <summary> Moves the caret backwards. </summary>
  public class MoveCaretForwardCommand : CaretCommand
  {
    /// <inheritdoc />
    public override string Id
      => "caret.moveForward";

    /// <inheritdoc />
    public override bool Activate(DocumentSelection cursor, CaretMovementMode movementMode, SelectionMode mode)
    {
      // first we try to move the cursor directly
      var caret = cursor.Start;
      var next = caret.MoveForward();

      // try a simply to move the cursor forwards
      if (next.IsValid)
      {
        cursor.MoveTo(next, mode);
        return true;
      }

      // try get the block backward to get a cursor that looks at the beginning of that block
      var block = caret.Block?.Parent.GetBlockTo(BlockDirection.Forward, caret.Block) as ContentBlock;
      if (block != null)
      {
        cursor.MoveTo(block.GetCaretAtStart(), mode);
        return true;
      }

      // we couldn't do it
      return false;
    }
  }
}