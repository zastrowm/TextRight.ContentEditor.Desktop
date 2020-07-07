using System;
using System.Collections.Generic;
using System.Linq;

namespace TextRight.Core.ObjectModel.Blocks.Collections
{
  /// <summary>
  ///   Event arguments for when a block is inserted into a Block Collection.
  /// </summary>
  public class BlockInsertedEventArgs : EventEmitterArgs<IBlockCollectionEventListener>
  {
    public BlockInsertedEventArgs(Block previousSibling, Block newBlock, Block nextSibling)
    {
      PreviousSibling = previousSibling;
      NewBlock = newBlock;
      NextSibling = nextSibling;
    }

    /// <summary>
    ///   The block that precedes the new block that was inserted.
    /// </summary>
    public Block PreviousSibling { get; }

    /// <summary>
    ///   The new block that was inserted.
    /// </summary>
    public Block NewBlock { get; }

    /// <summary>
    ///   The block that follows the new block that was inserted.
    /// </summary>
    public Block NextSibling { get; }

    /// <inheritdoc />
    protected override void Handle(object sender, IBlockCollectionEventListener listener)
      => listener.NotifyBlockInserted(this);
  }
}