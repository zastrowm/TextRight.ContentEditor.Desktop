﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TextRight.ContentEditor.Core.ObjectModel.Blocks;

namespace TextRight.ContentEditor.Core.ObjectModel.Cursors
{
  /// <summary> Maintains the state of a cursor so that it can be restored later. </summary>
  public struct CursorSnapshot : IDisposable
  {
    private PooledBlockContentCursor _cursor;

    /// <summary> Creates a new snapshot that stores the state of the given cursor </summary>
    /// <param name="cursor"> The cursor whose state should be remembered. </param>
    /// <returns> A CursorSnapshot. </returns>
    public static CursorSnapshot From(IBlockContentCursor cursor)
    {
      var snapshot = new CursorSnapshot();
      snapshot.Snapshot(cursor);
      return snapshot;
    }

    /// <summary> Save the current state of the given cursor so that it can be restored later. </summary>
    /// <param name="cursor"> The cursor whose state should be remembered. </param>
    public void Snapshot(IBlockContentCursor cursor)
    {
      if (cursor == null)
        throw new ArgumentNullException(nameof(cursor));

      _cursor.Set(cursor);
    }

    /// <summary> Sets the given cursor to the state represented by this cursor snapshot. </summary>
    /// <exception cref="InvalidOperationException"> Thrown when the requested operation is invalid. </exception>
    /// <param name="cursor"> The cursor whose state should be set equal to the state in this snapshot. </param>
    public void Restore(IBlockContentCursor cursor)
    {
      if (_cursor.Cursor == null)
        throw new InvalidOperationException("No cursor is currently remembered in the snapshot");

      cursor.MoveTo(_cursor.Cursor);
    }

    /// <summary>
    ///  Returns any cursor currently held back to the cursor pool that the cursor was created
    /// </summary>
    public void Dispose()
    {
      _cursor.ReturnToPool();
    }
  }
}