using System;
using System.Collections.Generic;
using System.Linq;
using TextRight.ContentEditor.Core.ObjectModel.Blocks;
using TextRight.ContentEditor.Core.Utilities;

namespace TextRight.ContentEditor.Core.ObjectModel.Cursors
{
  /// <summary> Base class for content cursors. </summary>
  /// <typeparam name="TCursor"> Type of the cursor. </typeparam>
  /// <typeparam name="TBlock"> The type of block for which the cursor is valid. </typeparam>
  public abstract class BaseBlockContentCursor<TCursor, TBlock> : IBlockContentCursor
    where TCursor : BaseBlockContentCursor<TCursor, TBlock>
    where TBlock : Block
  {
    /// <summary> Pool of cursors for this content-cursor type. </summary>
    public static CursorPool<TCursor, TBlock> CursorPool { get; }
      = new CursorPool<TCursor, TBlock>();

    /// <summary> Specialised constructor for use only by derived class. </summary>
    /// <param name="block"> The block for which this cursor is valid. </param>
    protected BaseBlockContentCursor(TBlock block)
    {
      Block = block;
    }

    /// <summary> The block for which the cursor is valid. </summary>
    public TBlock Block { get; private set; }

    /// <inheritdoc />
    Block IBlockContentCursor.Block
      => Block;

    /// <inheritdoc />
    public abstract void MoveToBeginning();

    /// <inheritdoc />
    public abstract void MoveToEnd();

    /// <inheritdoc />
    public abstract bool MoveForward();

    /// <inheritdoc />
    public abstract bool MoveBackward();

    /// <inheritdoc />
    void IBlockContentCursor.Reset(Block block)
    {
      Block = (TBlock)block;
    }

    /// <inheritdoc />
    public abstract ISerializedBlockCursor Serialize();

    /// <inheritdoc />
    public abstract bool IsAtEnd { get; }

    /// <inheritdoc />
    public abstract bool IsAtBeginning { get; }

    /// <inheritdoc />
    public abstract MeasuredRectangle MeasureCursorPosition();

    /// <inheritdoc />
    ICursorPool IBlockContentCursor.CursorPool
      => CursorPool;

    /// <inheritdoc />
    void IBlockContentCursor.MoveTo(IBlockContentCursor cursor)
    {
      MoveTo((TCursor)cursor);
    }

    /// <summary> Sets this instance to be equal to the given cursor. </summary>
    /// <param name="cursor"> The cursor to set this cursor equal to. </param>
    public void MoveTo(TCursor cursor)
    {
      cursor.Block = Block;
      MoveToOverride(cursor);
    }

    /// <summary> Sets this instance to be equal to the given cursor. </summary>
    /// <param name="cursor"> The cursor to set this cursor equal to. </param>
    protected abstract void MoveToOverride(TCursor cursor);

    /// <summary> Creates a new instance of the cursor for the given block. </summary>
    /// <param name="block"> The block for which this cursor is valid. </param>
    protected abstract TCursor CreateInstance(TBlock block);
  }
}