using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace TextRight.ContentEditor.Core.ObjectModel.Blocks
{
  /// <summary> Holds a collection of blocks. </summary>
  public abstract class BlockCollection : Block
  {
    private readonly List<Block> _childrenCollection;

    /// <summary> Default constructor. </summary>
    protected BlockCollection()
    {
      _childrenCollection = new List<Block>();
      // TODO don't append a text block?
      Append(new TextBlock());
    }

    /// <summary> The blocks that exist in the collection. </summary>
    public IEnumerable<Block> Children
      => _childrenCollection;

    public void Append(Block block)
    {
      // TODO what if the existing span is empty, should it be removed?
      block.Parent = this;

      _childrenCollection.Add(block);
      block.Index = _childrenCollection.Count - 1;

      OnBlockInserted(GetPreviousBlock(block), block, null);
    }

    /// <summary> Inserts a block at the given position. </summary>
    /// <exception cref="ArgumentNullException"> Thrown when one or more required
    ///  arguments are null. </exception>
    /// <exception cref="ArgumentException"> Thrown when one or more arguments
    ///  have unsupported or illegal values. </exception>
    /// <param name="beforeBlock"> The block the new block should be placed after. </param>
    /// <param name="newBlock"> The block that should be inserted at the given position. </param>
    private void InsertBlockAfter(Block beforeBlock, Block newBlock)
    {
      if (beforeBlock == null)
        throw new ArgumentNullException(nameof(beforeBlock));
      if (newBlock == null)
        throw new ArgumentNullException(nameof(newBlock));
      if (beforeBlock.Parent != this)
        throw new ArgumentException(nameof(beforeBlock) + " is not a child of the given collection", nameof(newBlock));
      if (newBlock.Parent != null)
        throw new ArgumentException(nameof(newBlock) + " is already parented", nameof(newBlock));

      newBlock.Parent = this;
      _childrenCollection.Insert(beforeBlock.Index + 1, newBlock);
      ReIndexChildren(beforeBlock.Index);

      OnBlockInserted(beforeBlock, newBlock, GetNextBlock(newBlock));
    }

    /// <summary> Inserts a block at the given position. </summary>
    /// <exception cref="ArgumentNullException"> Thrown when one or more required
    ///  arguments are null. </exception>
    /// <exception cref="ArgumentException"> Thrown when one or more arguments
    ///  have unsupported or illegal values. </exception>
    /// <param name="afterBlock"> The block the new block should be placed before. </param>
    /// <param name="newBlock"> The block that should be inserted at the given position. </param>
    private void InsertBlockBefore(Block afterBlock, Block newBlock)
    {
      if (afterBlock == null)
        throw new ArgumentNullException(nameof(afterBlock));
      if (newBlock == null)
        throw new ArgumentNullException(nameof(newBlock));
      if (afterBlock.Parent != this)
        throw new ArgumentException(nameof(afterBlock) + " is not a child of the given collection", nameof(newBlock));
      if (newBlock.Parent != null)
        throw new ArgumentException(nameof(newBlock) + " is already parented", nameof(newBlock));

      newBlock.Parent = this;
      _childrenCollection.Insert(afterBlock.Index, newBlock);
      ReIndexChildren(afterBlock.Index);

      OnBlockInserted(GetPreviousBlock(newBlock), newBlock, afterBlock);
    }

    /// <summary> Removes the given block from the collection. </summary>
    /// <param name="block"> The block to remove from the collection. </param>
    public void RemoveBlock(Block block)
    {
      // TODO what else do we need to do?

      int oldIndex = block.Index;

      var previous = GetPreviousBlock(block);
      var next = GetNextBlock(block);

      block.Parent = null;
      _childrenCollection.Remove(block);
      ReIndexChildren(oldIndex);

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

    /// <summary> Gets the block that follows the given block. </summary>
    /// <param name="block"> The block whose next block should be retrieved. </param>
    /// <returns> The next block in the collection. </returns>
    public virtual Block GetNextBlock(Block block)
    {
      if (block.Parent != this)
        return null;
      if (block.Index >= _childrenCollection.Count - 1)
        return null;

      return _childrenCollection[block.Index + 1];
    }

    /// <summary> Gets the block that precedes the given block. </summary>
    /// <param name="block"> The block whose previous block should be retrieved. </param>
    /// <returns> The previous block in the collection. </returns>
    public Block GetPreviousBlock(Block block)
    {
      if (block.Parent != this)
        return null;
      if (block.Index < 1)
        return null;

      return _childrenCollection[block.Index - 1];
    }

    /// <summary> Reset the index of each child in the block collection. </summary>
    /// <param name="startIndex"> The index at which the children should have their
    ///  indices renumbered. </param>
    private void ReIndexChildren(int startIndex = 0)
    {
      for (var i = startIndex; i < _childrenCollection.Count; i++)
      {
        _childrenCollection[i].Index = i;
      }
    }

    /// <inheritdoc/>
    public override BlockType BlockType
      => BlockType.ContainerBlock;

    /// <summary> Get the first block in the collection. </summary>
    public Block FirstBlock
      => _childrenCollection[0];

    /// <summary> Get the last block in the collection. </summary>
    public Block LastBlock
      => _childrenCollection[_childrenCollection.Count - 1];

    /// <summary> The number of children in the collection. </summary>
    public int ChildCount
      => _childrenCollection.Count;

    /// <summary> Get the block in the hierarchy from the given block path. </summary>
    /// <param name="path"> The path to the block to retrieve. </param>
    /// <returns> The block from path. </returns>
    public Block GetBlockFromPath(BlockPath path)
    {
      BlockCollection collection = this;
      Block block = null;

      for (int i = path.Ids.Length - 1; i >= 0; i--)
      {
        Debug.Assert(collection != null);
        block = collection._childrenCollection[i];
        collection = block as BlockCollection;
      }

      return block;
    }

    /// <inheritdoc/>
    public override IBlockContentCursor GetCursor()
    {
      throw new NotImplementedException();
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
        var textBlockCursor = (TextBlock.TextBlockCursor)cursor;
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
  }
}