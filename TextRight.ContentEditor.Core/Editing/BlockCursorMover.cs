using System;
using System.Collections.Generic;
using System.Linq;
using TextRight.ContentEditor.Core.Editing.Commands;
using TextRight.ContentEditor.Core.ObjectModel.Blocks;
using TextRight.ContentEditor.Core.ObjectModel.Cursors;
using TextRight.ContentEditor.Core.Utilities;

namespace TextRight.ContentEditor.Core.Editing
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
      var textCursor = (IBlockContentCursor)cursor;
      bool didMoveToNextLine;
      var result = MoveTowardsLineEdge(textCursor, out didMoveToNextLine);

      switch (result)
      {
        case EndMovementState.CouldNotMoveWithinBlock:
          return DidNotMove;
        case EndMovementState.MovedWithOneMove:
          // move back onto the line
          if (didMoveToNextLine && !DidReachEdge(textCursor))
          {
            MoveAway(textCursor);
          }
          return DidNotMove;
        case EndMovementState.MovedWithMoreThanOneMove:
          // move back onto the line
          if (didMoveToNextLine && !DidReachEdge(textCursor))
          {
            MoveAway(textCursor);
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
    /// <param name="cursor"></param>
    /// <param name="caretMovementMode"></param>
    /// <returns>
    ///  True if the caret was moved to the next line, false if it was not able to
    ///  be moved (reached edge of block).
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
              MoveToPosition(cursor, caretMovementMode.Position);
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
    ///  Moves the caret as close to the <see cref="desiredPosition"/> while staying on the current
    ///  line.
    /// </summary>
    /// <param name="cursor"> The cursor to move. </param>
    /// <param name="desiredPosition"> The desired position of the caret. </param>
    /// <returns> True if the caret moved, false otherwise. </returns>
    public bool MoveToPosition(IBlockContentCursor cursor, double desiredPosition)
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

    /// <summary>
    ///  Check if one rectangle is considered on the same "line" as another.
    /// </summary>
    private static bool AreInline(MeasuredRectangle original, MeasuredRectangle comparedTo)
    {
      MeasuredRectangle first,
                        second;

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