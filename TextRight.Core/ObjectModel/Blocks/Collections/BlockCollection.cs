using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using TextRight.Core.ObjectModel.Blocks.Text;

namespace TextRight.Core.ObjectModel.Blocks.Collections
{
  /// <summary> A block that holds a collection of child blocks. </summary>
  public abstract class BlockCollection : Block,
                                          IEquatable<BlockCollection>
  {
    [NotNull]
    private readonly BlockLinkedList _blockList;

    /// <summary> Default constructor. </summary>
    protected BlockCollection()
    {
      // TODO don't append a text block?
      _blockList = new BlockLinkedList(this, new ParagraphBlock());
    }

    /// <summary> The blocks that exist in the collection. </summary>
    public IEnumerable<Block> Children
      => _blockList;

    /// <summary> Adds a block to the end of the collection. </summary>
    /// <param name="block"> The block to add to the collection. </param>
    public void Append([NotNull] Block block)
    {
      // TODO what if the existing span is empty, should it be removed?
      _blockList.InsertAfter(_blockList.Tail, block);
      OnBlockInserted(block.PreviousBlock, block, null);
    }

    /// <summary> Inserts a block at the given position. </summary>
    /// <exception cref="ArgumentNullException"> Thrown when one or more required
    ///  arguments are null. </exception>
    /// <exception cref="ArgumentException"> Thrown when one or more arguments
    ///  have unsupported or illegal values. </exception>
    /// <param name="beforeBlock"> The block the new block should be placed after. </param>
    /// <param name="newBlock"> The block that should be inserted at the given position. </param>
    public void InsertBlockAfter([NotNull] Block beforeBlock, [NotNull] Block newBlock)
    {
      _blockList.InsertAfter(beforeBlock, newBlock);
      OnBlockInserted(beforeBlock, newBlock, newBlock.NextBlock);
    }

    /// <summary> Inserts a block at the given position. </summary>
    /// <exception cref="ArgumentNullException"> Thrown when one or more required
    ///  arguments are null. </exception>
    /// <exception cref="ArgumentException"> Thrown when one or more arguments
    ///  have unsupported or illegal values. </exception>
    /// <param name="afterBlock"> The block the new block should be placed before. </param>
    /// <param name="newBlock"> The block that should be inserted at the given position. </param>
    public void InsertBlockBefore([NotNull] Block afterBlock, [NotNull] Block newBlock)
    {
      _blockList.InsertBefore(afterBlock, newBlock);
      OnBlockInserted(newBlock.PreviousBlock, newBlock, afterBlock);
    }

    /// <summary> Replaces an existing block with the new block. </summary>
    /// <param name="targetBlock"> The block that is being replaced. </param>
    /// <param name="newBlock"> The new block that is replacing the original block. </param>
    public void Replace(Block targetBlock, Block newBlock)
    {
      // TODO this isn't safe for all collections
      InsertBlockAfter(targetBlock, newBlock);
      RemoveBlock(targetBlock);
    }

    /// <summary> Removes the given block from the collection. </summary>
    /// <param name="block"> The block to remove from the collection. </param>
    public void RemoveBlock(Block block)
    {
      if (block == null)
        throw new ArgumentNullException(nameof(block));

      // TODO what else do we need to do?
      int oldIndex = block.Index;

      var previous = block.PreviousBlock;
      var next = block.NextBlock;

      _blockList.Remove(block);

      OnBlockRemoved(previous, block, next, oldIndex);
    }

    /// <summary>
    ///  Invoked after a block has been inserted into the collection.
    /// </summary>
    /// <param name="previousBlock"> The block before the newBlock. </param>
    /// <param name="newBlock"> The new block that was inserted. </param>
    /// <param name="nextBlock"> The block that is now after the new block. </param>
    protected virtual void OnBlockInserted(Block previousBlock,
                                           Block newBlock,
                                           Block nextBlock)
    {
    }

    /// <summary>
    ///  Invoked after a block has been removed from the collection.
    /// </summary>
    /// <param name="previousBlock"> The block that used to come before the
    ///  removed block. </param>
    /// <param name="removedBlock"> The block that was removed. </param>
    /// <param name="nextBlock"> The block that used to come after the removed
    ///  block. </param>
    /// <param name="indexOfRemovedBlock"> The old index of the removed block. </param>
    protected virtual void OnBlockRemoved(Block previousBlock,
                                          Block removedBlock,
                                          Block nextBlock,
                                          int indexOfRemovedBlock)
    {
    }

    /// <summary> Get the first block in the collection. </summary>
    public Block FirstBlock
      => _blockList.Head;

    /// <summary> Get the last block in the collection. </summary>
    public Block LastBlock
      => _blockList.Tail;

    /// <summary> The number of children in the collection. </summary>
    public int ChildCount
      => _blockList.Count;

    /// <summary> Gets the child block at the given index. </summary>
    /// <param name="blockIndex"> Zero-based index of the block to retrieve. </param>
    /// <returns> The block child that has the given index </returns>
    internal Block GetNthBlock(int blockIndex)
    {
      return _blockList.GetAtIndex(blockIndex);
    }

    /// <summary>
    ///  Determines the block that would be encountered if a cursor was navigating in
    ///  <paramref name="direction"/> from <paramref name="block"/>.
    /// </summary>
    /// <param name="direction"> The theoretical direction a cursor is moving. </param>
    /// <param name="block"> The block from which the cursor is moving. </param>
    /// <returns>
    ///  The block that is closest in the given direct from <paramref name="block"/>
    /// </returns>
    public abstract Block GetBlockTo(BlockDirection direction, Block block);

    /// <nodoc />
    public bool Equals(BlockCollection other)
    {
      if (ReferenceEquals(null, other))
        return false;
      if (ReferenceEquals(this, other))
        return true;
      return _blockList.Equals(other._blockList);
    }

    /// <inheritdoc />
    public override bool Equals(object obj)
    {
      if (ReferenceEquals(null, obj))
        return false;
      if (ReferenceEquals(this, obj))
        return true;
      if (obj.GetType() != GetType())
        return false;
      return Equals((BlockCollection)obj);
    }

    public override int GetHashCode()
    {
      return _blockList.GetHashCode();
    }
  }
}