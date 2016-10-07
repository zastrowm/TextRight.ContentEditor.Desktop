using System;
using System.Collections.Generic;
using System.Linq;
using TextRight.Core.ObjectModel.Blocks;
using TextRight.Core.ObjectModel.Cursors;
using TextRight.Core.Utilities;

namespace TextRight.Core.Editing
{
  /// <summary>
  ///  Various algorithms for moving a cursor up/down Home/End.
  /// </summary>
  /// <remarks>
  ///  The algorithms for moving in one direction is almost always implemented
  ///  the same as moving in it's opposite direction.  So, in order to only have
  ///  on copy of the algorithms, each algorithm is implemented abstractly
  ///  moving Towards a direction (forward for example) and Away (backwards for
  ///  example).  When moving the opposite direction the same algorithm is used
  ///  except the meaning of Towards if flipped (so it becomes backwards) as is
  ///  Away (it becomes forward).
  ///  
  ///  Thus why we have <see cref="ForwardMover"/> and
  ///  <see cref="BackwardMover"/>.
  /// </remarks>
  internal abstract class BlockCursorMover
  {
    private const bool DidNotMove = false;

    private const bool DidMove = true;

    /// <summary> A cursor mover whose direction is forward by default. </summary>
    public static readonly BlockCursorMover ForwardMover
      = new MoveForwardMover();

    /// <summary> A cursor mover whose direction is backward by default. </summary>
    public static readonly BlockCursorMover BackwardMover
      = new MoveBackwardMover();

    /// <summary> Moves in the default direction of the cursor mover. </summary>
    /// <param name="cursor"> The cursor to move. </param>
    /// <returns> True if the cursor was able to move, false otherwise. </returns>
    public abstract bool MoveTowards(IBlockContentCursor cursor);

    /// <summary> Moves in the opposite direction of the cursor mover. </summary>
    /// <param name="cursor"> The cursor to move. </param>
    /// <returns> True if the cursor was able to move, false otherwise. </returns>
    public abstract bool MoveAway(IBlockContentCursor cursor);

    /// <summary> True if the edge of the block was reached.  </summary>
    /// <param name="cursor"> The cursor to move. </param>
    /// <returns> True if the edge is reached, false otherwise. </returns>
    public abstract bool DidReachEdge(IBlockContentCursor cursor);

    /// <summary> Move the caret to the edge of the current line. </summary>
    /// <param name="cursor"> The caret to move. </param>
    /// <returns> True if the cursor moved, false otherwise. </returns>
    public bool MoveCaretTowardsLineEdge(IBlockContentCursor cursor)
    {
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
    ///  Moves the caret to the next/previous line and attempts to move the caret as close to the
    ///  desired position as possible.  Updates the context to indicate that if the caret is
    ///  subsequently moved "up" or "down", the caret should attempt to stay at the given position in
    ///  the line.
    /// </summary>
    /// <exception cref="ArgumentOutOfRangeException"> Thrown when one or more arguments are outside
    ///  the required range. </exception>
    /// <param name="cursor"> The cursor to move. </param>
    /// <param name="caretMovementMode"> How the caret should move. </param>
    /// <returns>
    ///  True if the caret was moved to the next line, false if it was not able to be moved (reached
    ///  edge of block).
    /// </returns>
    public bool MoveCaretTowardsPositionInNextLine(IBlockContentCursor cursor,
                                                   CaretMovementMode caretMovementMode)
    {
      using (var snapshot = CursorSnapshot.From(cursor))
      {
        switch (caretMovementMode.CurrentMode)
        {
          case CaretMovementMode.Mode.None:
          {
            goto case CaretMovementMode.Mode.Position;
          }
          case CaretMovementMode.Mode.Position:
          {
            bool didMove;
            MoveTowardsLineEdge(cursor, out didMove);
            if (didMove)
            {
              MoveTowardsLineOffset(cursor, caretMovementMode.Position);
            }
            else
            {
              snapshot.Restore(cursor);
            }
            return didMove;
          }
          case CaretMovementMode.Mode.Home:
          {
            bool didMove;
            var state = MoveTowardsLineEdge(cursor, out didMove);
            if (!didMove)
            {
              snapshot.Restore(cursor);
              return false;
            }

            // move it back to the correct line
            BackwardMover.MoveCaretTowardsLineEdge(cursor);
            return state != EndMovementState.CouldNotMoveWithinBlock;
          }
          case CaretMovementMode.Mode.End:
          {
            bool didMove;
            var state = MoveTowardsLineEdge(cursor, out didMove);
            if (!didMove)
            {
              snapshot.Restore(cursor);
              return false;
            }
            // move it back to the correct line
            ForwardMover.MoveCaretTowardsLineEdge(cursor);
            return state != EndMovementState.CouldNotMoveWithinBlock;
          }
          default:
            throw new ArgumentOutOfRangeException();
        }
      }
    }

    /// <summary>
    ///  Moves the given cursor forward towards the line's edge usually continuing onto the next line
    ///  if <paramref name="didMoveToNextLine"/> is true.
    /// </summary>
    /// <remarks>
    ///  It should be expected that this method overshoots and continues onto the next line, indicated
    ///  by <paramref name="didMoveToNextLine"/> being true. If this is not desired, the caller should
    ///  check
    ///  <paramref name="didMoveToNextLine"/> and call <see cref="MoveAway"/> if
    ///  it is true.
    /// </remarks>
    /// <param name="cursor"> The cursor to move. </param>
    /// <param name="didMoveToNextLine"> [out] True if the caret overshot and was moved to the next
    ///  line. </param>
    /// <returns> An EndMovementState representing how the caret was moved. </returns>
    private EndMovementState MoveTowardsLineEdge(IBlockContentCursor cursor,
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
        didMoveToNextLine = !MeasuredRectangle.AreInline(cursor.MeasureCursorPosition(), originalPosition);
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
    ///  Moves the caret forward as close to the <paramref name="point"/> while staying in the block.
    /// </summary>
    /// <param name="cursor"> The cursor to move. </param>
    /// <param name="point"> The point to move towards. </param>
    public void MoveTowardsPoint(IBlockContentCursor cursor, DocumentPoint point)
    {
      MoveTowardsLine(cursor, point.Y);
      MoveTowardsLineOffset(cursor, point.X);
    }

    /// <summary>
    ///  Moves the cursor forward to the line closest to the given <paramref name="y"/> value.
    /// </summary>
    /// <param name="cursor"> The cursor to move. </param>
    /// <param name="y"> The y coordinate whose line we would like to be on. </param>
    public void MoveTowardsLine(IBlockContentCursor cursor, double y)
    {
      using (var closestLine = CursorSnapshot.From(cursor))
      {
        closestLine.Snapshot(cursor);

        var lastPosition = cursor.MeasureCursorPosition();

        while (MoveTowards(cursor))
        {
          var newPosition = cursor.MeasureCursorPosition();

          if (MeasuredRectangle.AreInline(newPosition, lastPosition))
          {
            // widen the last position so that it will be the full height of all of the characters
            lastPosition.Y = Math.Min(newPosition.Y, lastPosition.Y);
            lastPosition.Height = Math.Max(newPosition.Height, lastPosition.Height);
          }
          else if (VerticalDistanceTo(lastPosition, y) <= VerticalDistanceTo(newPosition, y))
          {
            // we're not inline, and the point we're trying to get to is now going to get further away, so
            // break out. 
            break;
          }
          else
          {
            // save the beginning of the line
            closestLine.Snapshot(cursor);
            lastPosition = newPosition;
          }
        }

        // always restore the beginning of the line
        closestLine.Restore(cursor);
      }
    }

    /// <summary>
    ///  Moves the caret as close to the <see cref="desiredPosition"/> while staying on the current
    ///  line.
    /// </summary>
    /// <param name="cursor"> The cursor to move. </param>
    /// <param name="desiredPosition"> The desired horizontal position of the caret. </param>
    /// <returns> True if the caret moved, false otherwise. </returns>
    public bool MoveTowardsLineOffset(IBlockContentCursor cursor, double desiredPosition)
    {
      var lastClosest = cursor.MeasureCursorPosition();
      double closestDistance = HorizontalDistanceTo(lastClosest, desiredPosition);

      int timesMoved = 0;
      bool didGoTooFar = false;

      do
      {
        var currentPosition = cursor.MeasureCursorPosition();

        // we may have gone too far, so exit out
        if (!MeasuredRectangle.AreInline(currentPosition, lastClosest))
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
    private class MoveBackwardMover : BlockCursorMover
    {
      public override bool MoveTowards(IBlockContentCursor cursor)
        => cursor.MoveBackward();

      public override bool MoveAway(IBlockContentCursor cursor)
        => cursor.MoveForward();

      public override bool DidReachEdge(IBlockContentCursor cursor)
        => cursor.IsAtBeginning;
    }

    /// <summary> A cursor mover whose direction is forward by default. </summary>
    private class MoveForwardMover : BlockCursorMover
    {
      public override bool MoveTowards(IBlockContentCursor cursor)
        => cursor.MoveForward();

      public override bool MoveAway(IBlockContentCursor cursor)
        => cursor.MoveBackward();

      public override bool DidReachEdge(IBlockContentCursor cursor)
        => cursor.IsAtEnd;
    }

    /// <summary> How the cursor moved (used by methods in this class). </summary>
    private enum EndMovementState
    {
      CouldNotMoveWithinBlock,
      MovedWithOneMove,
      MovedWithMoreThanOneMove,
    }

    /// <summary> Get the distance to the given position </summary>
    private static double HorizontalDistanceTo(MeasuredRectangle rect, double left)
    {
      return Math.Abs(rect.Center.X - left);
    }

    /// <summary> Get the distance to the given position </summary>
    private static double VerticalDistanceTo(MeasuredRectangle rect, double bottom)
    {
      return Math.Abs(rect.Center.Y - bottom);
    }
  }
}