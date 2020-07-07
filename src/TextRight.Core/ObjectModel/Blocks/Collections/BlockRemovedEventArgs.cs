using System;
using System.Collections.Generic;
using System.Linq;

namespace TextRight.Core.ObjectModel.Blocks.Collections
{
  /// <summary>
  ///   Event arguments for when a block is removed from a Block Collection.
  /// </summary>
  public class BlockRemovedEventArgs : EventEmitterArgs<IBlockCollectionEventListener>
  {
    public BlockRemovedEventArgs(Block oldPreviousSibling, Block blockRemoved, Block oldNextSibling, int oldIndex)
    {
      OldPreviousSibling = oldPreviousSibling;
      BlockRemoved = blockRemoved;
      OldNextSibling = oldNextSibling;
      OldIndex = oldIndex;
    }

    /// <summary>
    ///  The block that uses to follow the block that was removed.
    /// </summary>
    public Block OldPreviousSibling { get; }

    /// <summary>
    ///   The block that was removed from the collection.
    /// </summary>
    public Block BlockRemoved { get; }

    /// <summary>
    ///  The block that used to follow the block that was removed.
    /// </summary>
    public Block OldNextSibling { get; }

    /// <summary>
    ///   The index of the block before it was removed.
    /// </summary>
    public int OldIndex { get; }

    /// <inheritdoc />
    protected override void Handle(object sender, IBlockCollectionEventListener listener)
      => listener.NotifyBlockRemoved(this);
  }
}