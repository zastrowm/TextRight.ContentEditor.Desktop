using System;
using System.Collections.Generic;
using System.Linq;
using TextRight.Core.ObjectModel.Blocks;
using TextRight.Core.ObjectModel.Blocks.Text;
using TextRight.Core.ObjectModel.Blocks.Text.View;
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

    public override bool CanActivate(DocumentEditorContext context)
      => context.Caret.Is<TextCaret>();

    /// <inheritdoc />
    public override bool Activate(DocumentCursor cursor, CaretMovementMode movementMode)
    {
      var textCaret = cursor.Caret.As<TextCaret>();
      // TODO
      //if (movementMode.CurrentMode == CaretMovementMode.Mode.None)
      {
        var measurement = textCaret.Measure();
        movementMode.SetModeToPosition(textCaret.Measure().Left);
      }

      if (!(textCaret.Fragment.Owner.Target is ILineBasedRenderer lineBasedRenderer))
        return false;

      var currentLine = lineBasedRenderer.GetLineFor(textCaret);
      var previousLine = currentLine.Previous;

      if (previousLine == null)
        return false;

      // TODO
      var caret = previousLine.FindClosestTo(movementMode.Position);
      if (!caret.IsValid)
        return false;

      cursor.MoveTo(caret);
      return true;

      // TODO work the way up the document
      //var previousBlock = current.Block.Parent.GetBlockTo(BlockDirection.Top, current.Block);
      //if (previousBlock != null)
      //{
      //  var newCursor = previousBlock.GetCaretFromBottom(movementMode);
      //  cursor.MoveTo(newCursor);

      //  return true;
      //}

      //// we couldn't do it
      //return false;
    }
  }
}