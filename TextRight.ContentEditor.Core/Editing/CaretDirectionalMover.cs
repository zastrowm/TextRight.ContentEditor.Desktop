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
      return MoveToLine(textBlockCursor, MoveLeftMover, out didMoveToNextLine);
    }

    /// <selfdoc />
    private static EndMovementState MoveToBeginningOfNextLine(TextBlock.TextBlockCursor textBlockCursor,
                                                              out bool didMoveToNextLine)
    {
      return MoveToLine(textBlockCursor, MoveRightMover, out didMoveToNextLine);
    }

    public static void MoveCaretUpInDocument(DocumentEditorContext context)
    {
      MoveTowardsPositionInNextLine(context, MoveLeftMover);
    }

    public static void MoveCaretDownInDocument(DocumentEditorContext context)
    {
      MoveTowardsPositionInNextLine(context, MoveRightMover);
    }

    /// <selfdoc />
    private static EndMovementState MoveToLine(TextBlock.TextBlockCursor textBlockCursor,
                                               IDirectionalMover mover,
                                               out bool didMoveToNextLine)
    {
      var originalPosition = textBlockCursor.MeasureCursorPosition();
      var numTurns = 0;
      didMoveToNextLine = false;

      // We keep moving until we're not on the line anymore, and then correct
      // back if we go too far.
      while (!didMoveToNextLine && mover.MoveTowards(textBlockCursor))
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

    private static void MoveTowardsPositionInNextLine(DocumentEditorContext context, IDirectionalMover mover)
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
          MoveToLine(textBlockCursor, mover, out didMove);
          if (didMove)
          {
            MoveToPosition(textBlockCursor, context.CaretMovementMode.Position, mover);
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
                                       IDirectionalMover cursorMover)
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
        cursorMover.GetReversedMover().MoveTowards(textBlockCursor);
        timesMoved--;
      }

      return timesMoved == 0;
    }

    private static readonly IDirectionalMover MoveRightMover = MoveForwardMover.Instance;
    private static readonly IDirectionalMover MoveLeftMover = MoveBackwardMover.Instance;

    private interface IDirectionalMover
    {
      bool MoveTowards(TextBlock.TextBlockCursor cursor);
      bool DidReachEdge(TextBlock.TextBlockCursor cursor);
      IDirectionalMover GetReversedMover();
    }

    private class MoveBackwardMover : IDirectionalMover
    {
      public static readonly IDirectionalMover Instance = new MoveBackwardMover();

      private MoveBackwardMover()
      {
      }

      public bool MoveTowards(TextBlock.TextBlockCursor cursor)
        => cursor.MoveBackward();

      public bool DidReachEdge(TextBlock.TextBlockCursor cursor)
        => cursor.IsAtBeginning;

      public IDirectionalMover GetReversedMover()
        => MoveForwardMover.Instance;
    }

    private class MoveForwardMover : IDirectionalMover
    {
      public static readonly IDirectionalMover Instance = new MoveForwardMover();

      private MoveForwardMover()
      {
      }

      public bool MoveTowards(TextBlock.TextBlockCursor cursor)
        => cursor.MoveForward();

      public bool DidReachEdge(TextBlock.TextBlockCursor cursor)
        => cursor.IsAtEnd;

      public IDirectionalMover GetReversedMover()
        => MoveBackwardMover.Instance;
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