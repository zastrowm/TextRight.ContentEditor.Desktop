using System;
using System.Collections.Generic;
using System.Linq;
using TextRight.ContentEditor.Core.ObjectModel.Blocks;

namespace TextRight.ContentEditor.Core.ObjectModel.Cursors
{
  /// <summary> A temporary copy of a BlockContentCursor. </summary>
  public struct CursorCopy : IDisposable
  {
    private PooledBlockContentCursor _pooledCursor;

    /// <summary> Constructor. </summary>
    /// <param name="cursor"> The cursor to make a copy of. </param>
    public CursorCopy(IBlockContentCursor cursor)
    {
      _pooledCursor = new PooledBlockContentCursor();
      _pooledCursor.Set(cursor);
    }

    /// <summary> The current cursor. </summary>
    public IBlockContentCursor Cursor
      => _pooledCursor.Cursor;

    /// <summary>
    ///  Returns any cursor currently held back to the cursor pool that the cursor was created
    /// </summary>
    public void Dispose()
    {
      _pooledCursor.ReturnToPool();
    }
  }
}