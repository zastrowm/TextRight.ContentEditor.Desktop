using System;
using System.Collections.Generic;
using System.Linq;
using TextRight.ContentEditor.Core.ObjectModel.Blocks;
using TextRight.ContentEditor.Core.Utilities;

namespace TextRight.ContentEditor.Core.ObjectModel.Cursors
{
  /// <summary> A temporary copy of a BlockContentCursor. </summary>
  public struct CursorCopy : IDisposable,
                             IReadonlyCursor
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

    /// <inheritdoc />
    public Block Block =>
      _pooledCursor.Cursor.Block;

    /// <inheritdoc />
    public bool IsAtEnd
      => _pooledCursor.Cursor.IsAtEnd;

    /// <inheritdoc />
    public bool IsAtBeginning
      => _pooledCursor.Cursor.IsAtBeginning;

    /// <inheritdoc />
    public CursorCopy Copy()
      => new CursorCopy(_pooledCursor.Cursor);

    /// <inheritdoc />
    public ISerializedBlockCursor Serialize()
      => _pooledCursor.Cursor.Serialize();

    /// <inheritdoc />
    public MeasuredRectangle MeasureCursorPosition()
      => _pooledCursor.Cursor.MeasureCursorPosition();

    /// <inheritdoc />
    public bool Is<T>() where T : IBlockContentCursor
      => _pooledCursor.Cursor is T;
  }
}