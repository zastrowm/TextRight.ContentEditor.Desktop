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
      var textBlockCursor = (TextBlock.TextBlockCursor)context.Caret.BlockCursor;

      switch (context.CaretMovementMode.CurrentMode)
      {
        case CaretMovementMode.Mode.None:
        {
          double left = textBlockCursor.MeasureCursorPosition().Left;
          context.CaretMovementMode.SetModeToPosition(left);
          goto case CaretMovementMode.Mode.Position;
        }
        case CaretMovementMode.Mode.Position:
        {
          bool didMove;
          MoveToEndOfPreviousLine(textBlockCursor, out didMove);
          if (didMove)
          {
            MoveToPosition(textBlockCursor, context.CaretMovementMode.Position, CursorMover.MoveLeft);
          }
          break;
        }
        case CaretMovementMode.Mode.Home:
          break;
        case CaretMovementMode.Mode.End:
          break;
        default:
          throw new ArgumentOutOfRangeException();
      }
    }

    public static void MoveCaretDownInDocument(DocumentEditorContext context)
    {
      var textBlockCursor = (TextBlock.TextBlockCursor)context.Caret.BlockCursor;

      switch (context.CaretMovementMode.CurrentMode)
      {
        case CaretMovementMode.Mode.None:
        {
          double left = textBlockCursor.MeasureCursorPosition().Left;
          context.CaretMovementMode.SetModeToPosition(left);
          goto case CaretMovementMode.Mode.Position;
        }
        case CaretMovementMode.Mode.Position:
        {
          bool didMove;
          MoveToBeginningOfNextLine(textBlockCursor, out didMove);
          if (didMove)
          {
            MoveToPosition(textBlockCursor, context.CaretMovementMode.Position, CursorMover.MoveRight);
          }
          break;
        }
        case CaretMovementMode.Mode.Home:
          break;
        case CaretMovementMode.Mode.End:
          break;
        default:
          throw new ArgumentOutOfRangeException();
      }
    }

    private static bool MoveToPosition(TextBlock.TextBlockCursor textBlockCursor,
                                       double desiredPosition,
                                       CursorMover cursorMover)
    {
      var lastClosest = textBlockCursor.MeasureCursorPosition();
      double closestDistance = lastClosest.HorizontalDistanceTo(desiredPosition);

      int timesMoved = 0;
      bool didGoTooFar = false;

      do
      {
        var currentPosition = textBlockCursor.MeasureCursorPosition();

        // we may have gone too far, so exit out
        if (!currentPosition.IsInlineTo(lastClosest))
        {
          didGoTooFar = true;
          break;
        }

        // we may have moved past the position, in which case exit out
        var currentDistance = currentPosition.HorizontalDistanceTo(desiredPosition);
        if (!(currentDistance <= closestDistance))
        {
          didGoTooFar = true;
          break;
        }

        closestDistance = currentDistance;
        lastClosest = currentPosition;

        timesMoved++;

        // if we can't move back anymore, then stop trying
        if (!cursorMover.MoveTowards(textBlockCursor))
          break;
      } while (true);

      if (didGoTooFar)
      {
        cursorMover.MoveAway(textBlockCursor);
        timesMoved--;
      }

      return timesMoved == 0;
    }

    private struct CursorMover
    {
      public Func<TextBlock.TextBlockCursor, bool> MoveTowards;
      public Func<TextBlock.TextBlockCursor, bool> MoveAway;

      public static readonly CursorMover MoveLeft = new CursorMover()
                                                    {
                                                      MoveAway = cursor => cursor.MoveForward(),
                                                      MoveTowards = cursor => cursor.MoveBackward(),
                                                    };

      public static readonly CursorMover MoveRight = new CursorMover()
                                                     {
                                                       MoveAway = cursor => cursor.MoveBackward(),
                                                       MoveTowards = cursor => cursor.MoveForward(),
                                                     };
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

    internal static double HorizontalDistanceTo(this MeasuredRectangle rect, double left)
    {
      return Math.Abs(rect.Left - left);
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