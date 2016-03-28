using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using TextRight.ContentEditor.Core.Editing.Commands;

namespace TextRight.ContentEditor.Core.ObjectModel.Blocks
{
  /// <summary> Holds the view representation of the BlockCollection. </summary>
  public interface IBlockCollectionView
  {
    /// <summary> Notifies a block inserted. </summary>
    /// <param name="previousSibling"> The before block. </param>
    /// <param name="newBlock"> The new block. </param>
    /// <param name="nextSibling"> The after block. </param>
    void NotifyBlockInserted(Block previousSibling, Block newBlock, Block nextSibling);
  }

  /// <summary> Holds a collection of blocks. </summary>
  public class BlockCollection : Block,
                                 IEnumerable<Block>,
                                 ICommandProcessorHook
  {
    private readonly List<Block> _childrenCollection;

    /// <summary> Default constructor. </summary>
    public BlockCollection()
    {
      _childrenCollection = new List<Block>();
      Append(new TextBlock());
    }

    /// <summary> The blocks that exist in the collection. </summary>
    public IEnumerable<Block> Children
      => _childrenCollection;

    /// <summary>
    ///  The object that receives all notifications of changes from this instance.
    /// </summary>
    public IBlockCollectionView Target { get; set; }

    public void Append(Block block)
    {
      // TODO what if the existing span is empty, should it be removed?
      block.Parent = this;

      _childrenCollection.Add(block);
      block.Index = _childrenCollection.Count - 1;

      Target?.NotifyBlockInserted(GetPreviousBlock(block), block, null);
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

      Target?.NotifyBlockInserted(beforeBlock, newBlock, GetNextBlock(newBlock));
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

      Target?.NotifyBlockInserted(GetPreviousBlock(newBlock), newBlock, afterBlock);
    }

    public void RemoveBlock(Block block)
    {
      // TODO what else do we need to do?

      int oldIndex = block.Index;

      block.Parent = null;
      _childrenCollection.Remove(block);
      ReIndexChildren(oldIndex);

      // TODO notify
    }

    /// <summary> Gets the block that follows the given block. </summary>
    /// <param name="block"> The block whose next block should be retrieved. </param>
    /// <returns> The next block in the collection. </returns>
    public Block GetNextBlock(Block block)
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

    /// <inheritdoc />
    public override IBlockContentCursor GetCaretFromBottom(CaretMovementMode caretMovementMode)
    {
      return LastBlock.GetCaretFromBottom(caretMovementMode);
    }

    /// <inheritdoc />
    public override IBlockContentCursor GetCaretFromTop(CaretMovementMode caretMovementMode)
    {
      return LastBlock.GetCaretFromTop(caretMovementMode);
    }

    /// <inheritdoc/>
    public IEnumerator<Block> GetEnumerator()
    {
      return _childrenCollection.GetEnumerator();
    }

    /// <inheritdoc/>
    IEnumerator IEnumerable.GetEnumerator()
    {
      return GetEnumerator();
    }

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

    /// <inheritdoc />
    public ICommandProcessor CommandProcessor
      => BlockCollectionCommandProcessor.Instance;
  }
}