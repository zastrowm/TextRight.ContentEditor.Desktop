using System;
using System.Collections.Generic;
using System.Linq;
using TextRight.ContentEditor.Core.ObjectModel.Blocks;

namespace TextRight.ContentEditor.Core.ObjectModel.Cursors
{
  /// <summary>
  ///  A structure which holds a reference to a block cursor and automatically recycles the cursor
  ///  when a new cursor is assigned.
  /// </summary>
  internal struct PooledBlockContentCursor
  {
    /// <summary> The current cursor stored. </summary>
    public IBlockContentCursor Cursor { get; private set; }

    /// <summary> Stores a new cursor, restoring the current cursor if there is one. </summary>
    /// <param name="cursor"> The new cursor to store. </param>
    public void Set(IBlockContentCursor cursor)
    {
      ReturnToPool();

      if (cursor == null)
      {
        Cursor = null;
      }
      else
      {
        var newCursor = cursor.CursorPool.Borrow(cursor.Block);
        newCursor.MoveTo(cursor);
        Cursor = newCursor;
      }
    }

    /// <summary>
    ///  Returns any cursor currently held back to the cursor pool that the cursor was created and
    ///  stores a null cursor.
    /// </summary>
    public void ReturnToPool()
    {
      if (Cursor == null)
        return;

      Cursor.CursorPool.Recycle(Cursor);
      Cursor = null;
    }
  }
}