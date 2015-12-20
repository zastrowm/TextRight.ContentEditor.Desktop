using System;
using System.Collections.Generic;
using System.Linq;
using TextRight.ContentEditor.Core.ObjectModel.Blocks;
using TextRight.ContentEditor.Core.Utilities;

namespace TextRight.ContentEditor.Core.Editing
{
  /// <summary> Moves the caret up or down. </summary>
  internal static class CaretDirectionalMover
  {
    private const bool DidNotMove = false;
    private const bool DidMove = true;

    /// <summary> Moves the caret to the beginning of the line. </summary>
    /// <param name="context"> The context. </param>
    /// <returns> True if the caret moved, false otherwise. </returns>
    public static bool MoveCaretToBeginningOfLine(DocumentEditorContext context)
    {
      bool didMoveToNextLine;

      var textBlockCursor = (TextBlock.TextBlockCursor)context.Caret.BlockCursor;
      var result = MoveToEndOfPreviousLine(textBlockCursor, out didMoveToNextLine);

      switch (result)
      {
        case EndMovementState.CouldNotMoveWithinBlock:
          return DidNotMove;
        case EndMovementState.MovedWithOneMove:
          // move back onto the line
          if (!textBlockCursor.IsAtBeginning)
          {
            textBlockCursor.MoveForward();
          }
          return DidNotMove;
        case EndMovementState.MovedWithMoreThanOneMove:
          // move back onto the line
          if (!textBlockCursor.IsAtBeginning)
          {
            textBlockCursor.MoveForward();
          }
          return DidMove;
        default:
          throw new ArgumentOutOfRangeException();
      }
    }

    /// <summary> Move the caret to the end of the current line . </summary>
    /// <exception cref="ArgumentOutOfRangeException"> Thrown when one or more
    ///  arguments are outside the required range. </exception>
    /// <param name="context"> The context. </param>
    /// <returns> True if the cursor moved, false otherwise. </returns>
    public static bool MoveCaretToEndOfLine(DocumentEditorContext context)
    {
      var textBlockCursor = (TextBlock.TextBlockCursor)context.Caret.BlockCursor;
      bool didMoveToNextLine;
      var result = MoveToBeginningOfNextLine(textBlockCursor, out didMoveToNextLine);

      switch (result)
      {
        case EndMovementState.CouldNotMoveWithinBlock:
          return DidNotMove;
        case EndMovementState.MovedWithOneMove:
          // move back onto the line
          if (!textBlockCursor.IsAtEnd)
          {
            textBlockCursor.MoveBackward();
          }
          return DidNotMove;
        case EndMovementState.MovedWithMoreThanOneMove:
          // move back onto the line
          if (!textBlockCursor.IsAtEnd)
          {
            textBlockCursor.MoveBackward();
          }
          return DidMove;
        default:
          throw new ArgumentOutOfRangeException();
      }
    }

    /// <selfdoc />
    private static EndMovementState MoveToEndOfPreviousLine(TextBlock.TextBlockCursor textBlockCursor,
                                                            out bool didMoveToNextLine)
    {
      var originalPosition = textBlockCursor.MeasureCursorPosition();
      var numTurns = 0;

      didMoveToNextLine = false;

      // We keep moving until we're not on the line anymore, and then correct
      // back if we go too far.
      while (!didMoveToNextLine && textBlockCursor.MoveBackward())
      {
        numTurns++;
        didMoveToNextLine = !textBlockCursor.MeasureCursorPosition().IsInlineTo(originalPosition);
      }

      switch (numTurns)
      {
        case 0:
          return EndMovementState.CouldNotMoveWithinBlock;
        case 1:
          return EndMovementState.MovedWithOneMove;
        default:
          return EndMovementState.MovedWithMoreThanOneMove;
      }
    }

    /// <selfdoc />
    private static EndMovementState MoveToBeginningOfNextLine(TextBlock.TextBlockCursor textBlockCursor,
                                                              out bool didMoveToNextLine)
    {
      var originalPosition = textBlockCursor.MeasureCursorPosition();
      var numTurns = 0;
      didMoveToNextLine = false;

      // We keep moving until we're not on the line anymore, and then correct
      // back if we go too far.
      while (!didMoveToNextLine && textBlockCursor.MoveForward())
      {
        numTurns++;
        didMoveToNextLine = (!textBlockCursor.MeasureCursorPosition().IsInlineTo(originalPosition));
      }

      switch (numTurns)
      {
        case 0:
          return EndMovementState.CouldNotMoveWithinBlock;
        case 1:
          return EndMovementState.MovedWithOneMove;
        default:
          return EndMovementState.MovedWithMoreThanOneMove;
      }
    }

    public static void MoveCaretUpInDocument(DocumentEditorContext context)
    {
      throw new NotImplementedException();
    }

    /// <summary>
    ///  Check if one rectangle is considered on the same "line" as another.
    /// </summary>
    internal static bool IsInlineTo(this MeasuredRectangle original, MeasuredRectangle comparedTo)
    {
      MeasuredRectangle first, second;

      if (original.Top <= comparedTo.Top)
      {
        first = original;
        second = comparedTo;
      }
      else
      {
        first = comparedTo;
        second = original;
      }

      // if the second point has its top between the top of the first
      // point and the first points bottom, the second point is considered
      // inline with the other
      // TODO do we need some sort of buffer (perhaps subtracting a small number from top?
      return second.Top < first.Bottom;
    }

    /// <summary> How the cursor moved (used by methods in this class). </summary>
    private enum EndMovementState
    {
      CouldNotMoveWithinBlock,
      MovedWithOneMove,
      MovedWithMoreThanOneMove,
    }
  }
}