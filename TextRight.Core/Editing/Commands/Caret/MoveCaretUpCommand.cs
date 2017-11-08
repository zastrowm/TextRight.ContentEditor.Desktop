using System;
using System.Collections.Generic;
using System.Linq;
using TextRight.Core.ObjectModel.Blocks;
using TextRight.Core.ObjectModel.Blocks.Text;
using TextRight.Core.ObjectModel.Cursors;

namespace TextRight.Core.Commands.Caret
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
      => context.Selection.Is<TextCaret>();

    /// <inheritdoc />
    public override bool Activate(DocumentSelection cursor, CaretMovementMode movementMode)
    {
      var textCaret = cursor.Start.As<TextCaret>();

      double desiredPosition = UpdateMovementMode(movementMode, textCaret);

      if (textCaret.Fragment.Owner.Target == null)
        return false;

      var currentLine = textCaret.Fragment.Owner.Target.GetLineFor(textCaret);
      var previousLine = currentLine.Previous;

      if (previousLine != null)
      {
        // TODO what if it's invalid (maybe only if the line couldn't be found?) 
        var caret = previousLine.FindClosestTo(desiredPosition);
        if (!caret.IsValid)
          return false;

        cursor.MoveTo(caret);
        return true;
      }

      var currentBlock = textCaret.Block;
      var previousBlock = currentBlock.Parent.GetBlockTo(BlockDirection.Top, currentBlock);
      if (previousBlock != null)
      {
        var newCursor = previousBlock.GetCaretFromBottom(movementMode);
        cursor.MoveTo(newCursor);

        return true;
      }

      // TODO work the way up the document
      return false;
    }

    private double UpdateMovementMode(CaretMovementMode movementMode, TextCaret caret)
    {
      switch (movementMode.CurrentMode)
      {
        case CaretMovementMode.Mode.None:
          var position = caret.Measure().Left;
          movementMode.SetModeToPosition(position);
          return position;
        case CaretMovementMode.Mode.Position:
          return movementMode.Position;
        case CaretMovementMode.Mode.Home:
          return 0;
        case CaretMovementMode.Mode.End:
          return double.MaxValue;
        default:
          throw new ArgumentOutOfRangeException();
      }
    }
  }
}