﻿using System;
using System.Collections.Generic;
using System.Linq;
using TextRight.Core;
using TextRight.Core.Commands.Caret;
using TextRight.Core.ObjectModel.Blocks;
using TextRight.Core.ObjectModel.Blocks.Text;
using TextRight.Core.ObjectModel.Cursors;

namespace TextRight.Editor.Text.Commands
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

    public override bool CanActivate(DocumentEditorContext context)
      => context.Selection.Is<TextCaret>();

    /// <inheritdoc />
    public override bool Activate(DocumentSelection cursor, CaretMovementMode movementMode, SelectionMode mode)
    {
      var textCaret = cursor.Start.As<TextCaret>();

      double desiredPosition = UpdateMovementMode(movementMode, textCaret);

      var contentView = textCaret.Block.GetViewOrNull<ITextBlockView>();
      if (contentView == null)
        return false;

      var currentLine = contentView.GetLineFor(textCaret);
      var nextLine = currentLine.Next;

      if (nextLine != null)
      {
        // TODO what if it's invalid (maybe only if the line couldn't be found?) 
        var caret = nextLine.FindClosestTo(desiredPosition);
        if (!caret.IsValid)
          return false;

        cursor.MoveTo(caret, mode);
        return true;
      }

      var currentBlock = textCaret.Block;
      var nextBlock = currentBlock.Parent.GetBlockTo(BlockDirection.Bottom, currentBlock);
      if (nextBlock != null)
      {
        if (nextBlock.GetViewOrNull<IBlockView>() is IBlockView nextBlockView)
        {
          var newCursor = nextBlockView.GetCaretFromTop(movementMode);
          cursor.MoveTo(newCursor, mode);

          return true;
        }
        else
        {
          return false;
        }
      }

      // TODO work the way up the document
      // we couldn't do it
      return false;
    }

    private double UpdateMovementMode(CaretMovementMode movementMode, TextCaret caret)
    {
      switch (movementMode.CurrentMode)
      {
        case CaretMovementMode.Mode.None:
          var position = TextCaretMeasurerHelper.Measure(caret).Left;
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