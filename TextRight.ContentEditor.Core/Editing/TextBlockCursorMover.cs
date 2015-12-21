using System;
using System.Collections.Generic;
using System.Linq;
using TextRight.ContentEditor.Core.ObjectModel.Blocks;
using TextRight.ContentEditor.Core.Utilities;

namespace TextRight.ContentEditor.Core.Editing
{
  /// <summary>
  ///  Various algorithms for moving a cursor up/down Home/End.
  /// </summary>
  internal abstract class TextBlockCursorMover
  {
    private const bool DidNotMove = false;

    private const bool DidMove = true;

    /// <summary> A cursor mover whose direction is forward by default. </summary>
    public static readonly TextBlockCursorMover ForwardMover
      = new MoveForwardMover();

    /// <summary> A cursor mover whose direction is backward by default. </summary>
    public static readonly TextBlockCursorMover BackwardMover
      = new MoveBackwardMover();

    /// <summary> Moves in the default direction of the cursor mover. </summary>
    /// <param name="cursor"> The cursor to move. </param>
    /// <returns> True if the cursor was able to move, false otherwise. </returns>
    public abstract bool MoveTowards(TextBlock.TextBlockCursor cursor);

    /// <summary> Moves in the opposite direction of the cursor mover. </summary>
    /// <param name="cursor"> The cursor to move. </param>
    /// <returns> True if the cursor was able to move, false otherwise. </returns>
    public abstract bool MoveAway(TextBlock.TextBlockCursor cursor);

    /// <summary> True if the edge of the block was reached.  </summary>
    /// <param name="cursor"> The cursor to move. </param>
    /// <returns> True if the edge is reached, false otherwise. </returns>
    public abstract bool DidReachEdge(TextBlock.TextBlockCursor cursor);

    /// <summary> Move the caret to the edge of the current line. </summary>
    /// <param name="context"> The context. </param>
    /// <returns> True if the cursor moved, false otherwise. </returns>
    public bool MoveCaretTowardsLineEdge(DocumentEditorContext context)
    {
      var cursor = (TextBlock.TextBlockCursor)context.Caret.BlockCursor;
      bool didMoveToNextLine;
      var result = MoveTowardsLineEdge(cursor, out didMoveToNextLine);

      switch (result)
      {
        case EndMovementState.CouldNotMoveWithinBlock:
          return DidNotMove;
        case EndMovementState.MovedWithOneMove:
          // move back onto the line
          if (didMoveToNextLine && !DidReachEdge(cursor))
          {
            MoveAway(cursor);
          }
          return DidNotMove;
        case EndMovementState.MovedWithMoreThanOneMove:
          // move back onto the line
          if (didMoveToNextLine && !DidReachEdge(cursor))
          {
            MoveAway(cursor);
          }
          return DidMove;
        default:
          throw new ArgumentOutOfRangeException();
      }
    }

    /// <summary>
    ///  Moves the caret to the next/previous line and attempts to move the caret
    ///  as close to the caret as possible.  Updates the context to indicate that
    ///  if the caret is subsequently moved "up" or "down", the caret should
    ///  attempt to stay at the given position in the line.
    /// </summary>
    /// <exception cref="ArgumentOutOfRangeException"> Thrown when one or more
    ///  arguments are outside the required range. </exception>
    /// <param name="context"> The context's whose caret should be updated. </param>
    public void MoveCaretTowardsPositionInNextLine(DocumentEditorContext context)
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
          MoveTowardsLineEdge(textBlockCursor, out didMove);
          if (didMove)
          {
            MoveToPosition(textBlockCursor, context.CaretMovementMode.Position);
          }
          break;
        }
        case CaretMovementMode.Mode.Home:
        {
          bool didMove;
          MoveTowardsLineEdge(textBlockCursor, out didMove);
          if (didMove)
          {
            BackwardMover.MoveCaretTowardsLineEdge(context);
          }
          break;
        }
        case CaretMovementMode.Mode.End:
        {
          bool didMove;
          MoveTowardsLineEdge(textBlockCursor, out didMove);
          if (didMove)
          {
            ForwardMover.MoveCaretTowardsLineEdge(context);
          }
          break;
        }
        default:
          throw new ArgumentOutOfRangeException();
      }
    }

    /// <summary>
    ///  Moves the given cursor towards the line's edge (right or left) usually
    ///  continuing onto the next line if <paramref name="didMoveToNextLine"/> is
    ///  true.
    /// </summary>
    /// <remarks>
    ///  It should be expected that this method overshoots and continues onto the
    ///  next line, indicated by <paramref name="didMoveToNextLine"/> being true.
    ///  If this is not desired, the caller should check
    ///  <paramref name="didMoveToNextLine"/> and call <see cref="MoveAway"/> if
    ///  it is true.
    /// </remarks>
    /// <param name="cursor"> The cursor to move. </param>
    /// <param name="didMoveToNextLine"> [out] True if the caret overshot and was
    ///  moved to the next line. </param>
    /// <returns> An EndMovementState representing how the caret was moved. </returns>
    private EndMovementState MoveTowardsLineEdge(TextBlock.TextBlockCursor cursor,
                                                 out bool didMoveToNextLine)
    {
      var originalPosition = cursor.MeasureCursorPosition();
      var numTurns = 0;
      didMoveToNextLine = false;

      // We keep moving until we're not on the line anymore, and then correct
      // back if we go too far.
      while (!didMoveToNextLine && MoveTowards(cursor))
      {
        numTurns++;
        didMoveToNextLine = !AreInline(cursor.MeasureCursorPosition(), originalPosition);
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

    /// <summary>
    ///  Moves the caret as close to the <see cref="desiredPosition"/> in the line.
    /// </summary>
    /// <param name="cursor"> The cursor to move. </param>
    /// <param name="desiredPosition"> The desired position of the caret. </param>
    /// <returns> True if the caret moved, false otherwise. </returns>
    // ReSharper disable once UnusedMethodReturnValue.Local
    private bool MoveToPosition(TextBlock.TextBlockCursor cursor, double desiredPosition)
    {
      var lastClosest = cursor.MeasureCursorPosition();
      double closestDistance = HorizontalDistanceTo(lastClosest, desiredPosition);

      int timesMoved = 0;
      bool didGoTooFar = false;

      do
      {
        var currentPosition = cursor.MeasureCursorPosition();

        // we may have gone too far, so exit out
        if (!AreInline(currentPosition, lastClosest))
        {
          didGoTooFar = true;
          break;
        }

        // we may have moved past the position, in which case exit out
        var currentDistance = HorizontalDistanceTo(currentPosition, desiredPosition);
        if (!(currentDistance <= closestDistance))
        {
          didGoTooFar = true;
          break;
        }

        closestDistance = currentDistance;
        lastClosest = currentPosition;

        timesMoved++;

        // if we can't move back anymore, then stop trying
        if (!MoveTowards(cursor))
          break;
      } while (true);

      if (didGoTooFar)
      {
        MoveAway(cursor);
        timesMoved--;
      }

      return timesMoved == 0;
    }

    /// <summary> A cursor mover whose direction is backward by default. </summary>
    private class MoveBackwardMover : TextBlockCursorMover
    {
      public override bool MoveTowards(TextBlock.TextBlockCursor cursor)
        => cursor.MoveBackward();

      public override bool MoveAway(TextBlock.TextBlockCursor cursor)
        => cursor.MoveForward();

      public override bool DidReachEdge(TextBlock.TextBlockCursor cursor)
        => cursor.IsAtBeginning;
    }

    /// <summary> A cursor mover whose direction is forward by default. </summary>
    private class MoveForwardMover : TextBlockCursorMover
    {
      public override bool MoveTowards(TextBlock.TextBlockCursor cursor)
        => cursor.MoveForward();

      public override bool MoveAway(TextBlock.TextBlockCursor cursor)
        => cursor.MoveBackward();

      public override bool DidReachEdge(TextBlock.TextBlockCursor cursor)
        => cursor.IsAtEnd;
    }

    /// <summary> How the cursor moved (used by methods in this class). </summary>
    internal enum EndMovementState
    {
      CouldNotMoveWithinBlock,
      MovedWithOneMove,
      MovedWithMoreThanOneMove,
    }

    /// <summary>
    ///  Check if one rectangle is considered on the same "line" as another.
    /// </summary>
    private static bool AreInline(MeasuredRectangle original, MeasuredRectangle comparedTo)
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

    /// <summary> Get the distance to the given position </summary>
    private static double HorizontalDistanceTo(MeasuredRectangle rect, double left)
    {
      return Math.Abs(rect.Left - left);
    }
  }
}