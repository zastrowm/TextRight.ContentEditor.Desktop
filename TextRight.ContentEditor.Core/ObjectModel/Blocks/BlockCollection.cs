using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using TextRight.ContentEditor.Core.ObjectModel.Serialization;

namespace TextRight.ContentEditor.Core.ObjectModel.Blocks
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
      _blockList = new BlockLinkedList(this, new TextBlock());
    }

    /// <summary> The blocks that exist in the collection. </summary>
    public IEnumerable<Block> Children
      => _blockList;

    /// <summary> Adds a block to the end of the collection. </summary>
    /// <param name="block"> The block to remove from the collection. </param>
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
    private void InsertBlockAfter([NotNull] Block beforeBlock, [NotNull] Block newBlock)
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
    private void InsertBlockBefore([NotNull] Block afterBlock, [NotNull] Block newBlock)
    {
      _blockList.InsertBefore(afterBlock, newBlock);
      OnBlockInserted(newBlock.PreviousBlock, newBlock, afterBlock);
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

    /// <inheritdoc/>
    public override BlockType BlockType
      => BlockType.ContainerBlock;

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

    /// <inheritdoc/>
    public override IBlockContentCursor GetCursor()
    {
      throw new NotImplementedException();
    }

    /// <summary> Merges the given block with the previous block. </summary>
    /// <param name="block"> The block to remove from the collection and whose content should be merged with the previous block. </param>
    /// <returns> True if it was merged, false if it was not. </returns>
    public virtual bool MergeWithPrevious(Block block)
    {
      // TODO support more than just text blocks?
      var textBlock = block as TextBlock;
      if (textBlock == null)
        return false;

      // TODO handle parent-blocks
      if (textBlock.IsFirst)
        return false;

      var cursor = textBlock.GetTextCursor();
      cursor.MoveToBeginning();

      var previous = textBlock.PreviousBlock as TextBlock;
      if (previous == null)
        return false;

      var fragments = cursor.ExtractToEnd();
      previous.AppendAll(fragments);

      RemoveBlock(textBlock);

      return true;
    }

    /// <inheritdoc />
    public override string MimeType { get; }
      = null;

    /// <summary>
    ///  True if the block can break into two at the given position.
    /// </summary>
    /// <param name="cursor"> The caret that specified the position. </param>
    /// <returns> true if we can break, false if not. </returns>
    public bool CanBreak(IBlockContentCursor cursor)
      => true;

    /// <summary> Breaks the block into two at the given location. </summary>
    /// <param name="cursor"> The caret at which the block should be split. </param>
    /// <returns>
    ///  The block that is the next sibling of the original block that was split
    ///  into two.
    /// </returns>
    public Block TryBreakBlock(IBlockContentCursor cursor)
    {
      if (cursor == null)
        throw new ArgumentNullException(nameof(cursor));

      if (!CanBreak(cursor))
        return null;

      var targetBlock = cursor.Block;

      Block secondaryBlock = null;

      if (cursor.IsAtEnd)
      {
        secondaryBlock = new TextBlock();
        InsertBlockAfter(targetBlock, secondaryBlock);
      }
      else if (cursor.IsAtBeginning)
      {
        secondaryBlock = targetBlock;
        InsertBlockBefore(targetBlock, new TextBlock());
      }
      else
      {
        var textBlockCursor = (TextBlockCursor)cursor;
        var fragments = textBlockCursor.ExtractToEnd();

        var newTextBlock = new TextBlock();
        secondaryBlock = newTextBlock;

        // TODO should this be done by AppendSpan automatically?
        newTextBlock.RemoveSpan(newTextBlock.First());

        foreach (var fragment in fragments)
        {
          newTextBlock.AppendSpan(fragment);
        }

        InsertBlockAfter(targetBlock, secondaryBlock);
      }

      return secondaryBlock;
    }

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
      if (obj.GetType() != this.GetType())
        return false;
      return Equals((BlockCollection)obj);
    }

    public override int GetHashCode()
    {
      return _blockList.GetHashCode();
    }

  }
}