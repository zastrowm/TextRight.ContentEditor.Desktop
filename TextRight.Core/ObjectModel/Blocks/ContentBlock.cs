using System;
using System.Collections.Generic;
using System.Linq;
using TextRight.Core.Editing;
using TextRight.Core.ObjectModel.Cursors;

namespace TextRight.Core.ObjectModel.Blocks
{
  /// <summary>
  ///  A block that contains content instead of blocks, and thus supports cursors through the
  ///  content.
  /// </summary>
  public abstract class ContentBlock : Block
  {
    /// <summary> The cursor pool for cursors of this block. </summary>
    public abstract ICursorPool CursorPool { get; }

    /// <summary> Gets a block-specific iterator. </summary>
    /// <returns> An iterate that can move through the block. </returns>
    public IBlockContentCursor GetCursor()
    {
      return CursorPool.Borrow(this);
    }

    /// <summary> Shorthand for block.CursorPool.GetCursorCopy(block); </summary>
    public CursorCopy GetCursorCopy()
    {
      return CursorPool.GetCursorCopy(this);
    }

    /// <summary> Gets a block-specific iterator. </summary>
    /// <returns> An iterate that can move through the block. </returns>
    protected abstract IBlockContentCursor CreateCursorOverride();

    /// <summary> Internal method for creating cursors. </summary>
    internal IBlockContentCursor CreateCursor()
    {
      return CreateCursorOverride();
    }

    /// <summary> Retrieves a cursor closest to the given point in the block. </summary>
    /// <param name="point"> The point . </param>
    /// <returns> The cursor for. </returns>
    public virtual IBlockContentCursor GetCursorFor(DocumentPoint point)
    {
      // slow, inefficient mode
      var start = GetCursor().ToBeginning();
      BlockCursorMover.ForwardMover.MoveTowardsPoint(start, point);
      return start;
    }
  }
}