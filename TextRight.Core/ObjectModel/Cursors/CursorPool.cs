using System;
using System.Collections.Generic;
using System.Linq;
using TextRight.Core.ObjectModel.Blocks;

namespace TextRight.Core.ObjectModel.Cursors
{
  /// <summary> Base class for the most common implementation of ICursorPool. </summary>
  public sealed class CursorPool<TCursor, TBlock> : ICursorPool
    where TCursor : class, IBlockContentCursor
    where TBlock : ContentBlock
  {
    private readonly Stack<TCursor> _cursors;

    /// <summary> Default constructor. </summary>
    public CursorPool()
    {
      _cursors = new Stack<TCursor>();
    }

    /// <summary> Gets a cursor from the pool. </summary>
    /// <param name="block"> The block for which the cursor is valid. </param>
    /// <returns> The cursor from the pool. </returns>
    public TCursor Borrow(TBlock block)
    {
      if (block == null)
        throw new ArgumentNullException(nameof(block));

      if (_cursors.Count > 0)
      {
        var pooledCursor = _cursors.Pop();
        pooledCursor.Reset(block);
        return pooledCursor;
      }
      else
      {
        return (TCursor)block.CreateCursor();
      }
    }

    /// <summary> Puts a cursor back into the pool. </summary>
    /// <param name="cursor"> The cursor to put into the pool. </param>
    public void Recycle(TCursor cursor)
    {
      if (cursor == null)
        throw new ArgumentNullException(nameof(cursor));

      _cursors.Push(cursor);
    }

    /// <inheritdoc />
    public CursorCopy GetCursorCopy(TBlock block)
    {
      var fake = Borrow(block);
      var copy = new CursorCopy(fake);
      Recycle(fake);
      return copy;
    }

    /// <inheritdoc />
    IBlockContentCursor ICursorPool.Borrow(Block block)
    {
      return Borrow((TBlock)block);
    }

    /// <inheritdoc />
    void ICursorPool.Recycle(IBlockContentCursor cursor)
    {
      Recycle((TCursor)cursor);
    }

    /// <inheritdoc />
    CursorCopy ICursorPool.GetCursorCopy(Block block)
    {
      return GetCursorCopy((TBlock)block);
    }
  }
}