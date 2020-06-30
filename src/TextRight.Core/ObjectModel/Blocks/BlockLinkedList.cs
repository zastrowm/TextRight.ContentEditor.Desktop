using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using JetBrains.Annotations;
using TextRight.Core.ObjectModel.Blocks.Collections;

namespace TextRight.Core.ObjectModel.Blocks
{
  /// <summary> Maintains a linked-list of blocks. </summary>
  internal class BlockLinkedList : IEnumerable<Block>,
                                   IEquatable<BlockLinkedList>
  {
    [NotNull]
    private readonly BlockCollection _parentCollection;

    public BlockLinkedList(BlockCollection parentCollection, Block block)
    {
      if (parentCollection == null)
        throw new ArgumentNullException(nameof(parentCollection));
      if (block == null)
        throw new ArgumentNullException(nameof(block));

      _parentCollection = parentCollection;

      block.Parent = _parentCollection;
      block.Index = 0;
      Head = block;
      Tail = block;

      Count = 1;
    }

    /// <summary> The number of blocks in the list.  </summary>
    public int Count { get; private set; }

    /// <summary> The first block in the list. </summary>
    [NotNull]
    public Block Head { get; private set; }

    /// <summary> The last block in the list. </summary>
    [NotNull]
    public Block Tail { get; private set; }

    /// <summary> Inserts a new block to be before an existing block. </summary>
    /// <exception cref="ArgumentNullException"> Thrown when one or more required
    ///  arguments are null. </exception>
    /// <exception cref="ArgumentException"> Thrown when one or more arguments
    ///  have unsupported or illegal values. </exception>
    /// <param name="beforeBlock"> The block that the new block will be inserted before. </param>
    /// <param name="newBlock"> The new block to insert into the list. </param>
    public void InsertBefore(Block beforeBlock, Block newBlock)
    {
      if (beforeBlock == null)
        throw new ArgumentNullException(nameof(beforeBlock));
      if (newBlock == null)
        throw new ArgumentNullException(nameof(newBlock));
      if (beforeBlock.Parent != _parentCollection)
        throw new ArgumentException(nameof(beforeBlock) + " is not a child of the given collection", nameof(beforeBlock));
      if (newBlock.Parent != null)
        throw new ArgumentException(nameof(newBlock) + " is already parented", nameof(newBlock));

      var oldPrevious = beforeBlock.PreviousBlock;

      beforeBlock.PreviousBlock = newBlock;
      newBlock.NextBlock = beforeBlock;

      if (oldPrevious != null)
      {
        oldPrevious.NextBlock = newBlock;
        newBlock.PreviousBlock = oldPrevious;
      }
      else
      {
        // only way that oldPrevious could be null is if the old block was the Head
        Head = newBlock;
      }

      newBlock.Parent = _parentCollection;

      Count += 1;

      Reindex(oldPrevious);
    }

    /// <summary> Inserts a block after an existing block. </summary>
    /// <exception cref="ArgumentNullException"> Thrown when one or more required
    ///  arguments are null. </exception>
    /// <exception cref="ArgumentException"> Thrown when one or more arguments
    ///  have unsupported or illegal values. </exception>
    /// <param name="afterBlock"> The block after which the new block should be inserted. </param>
    /// <param name="newBlock"> The new block to insert into the list. </param>
    public void InsertAfter(Block afterBlock, Block newBlock)
    {
      if (afterBlock == null)
        throw new ArgumentNullException(nameof(afterBlock));
      if (newBlock == null)
        throw new ArgumentNullException(nameof(newBlock));
      if (afterBlock.Parent != _parentCollection)
        throw new ArgumentException(nameof(afterBlock) + " is not a child of the given collection", nameof(afterBlock));
      if (newBlock.Parent != null)
        throw new ArgumentException(nameof(newBlock) + " is already parented", nameof(newBlock));

      var oldNext = afterBlock.NextBlock;

      afterBlock.NextBlock = newBlock;
      newBlock.PreviousBlock = afterBlock;

      if (oldNext != null)
      {
        oldNext.PreviousBlock = newBlock;
        newBlock.NextBlock = oldNext;
      }
      else
      {
        // only way that oldNext could be null is if the old block was the Tail
        Tail = newBlock;
      }

      newBlock.Parent = _parentCollection;

      Count += 1;

      Reindex(afterBlock);
    }

    /// <summary> Removes teh given block from the collection. </summary>
    /// <exception cref="ArgumentNullException"> Thrown when one or more required
    ///  arguments are null. </exception>
    /// <exception cref="ArgumentException"> Thrown when one or more arguments
    ///  have unsupported or illegal values. </exception>
    /// <param name="block"> The block to remove. </param>
    public void Remove(Block block)
    {
      if (block == null)
        throw new ArgumentNullException(nameof(block));
      if (block.Parent != _parentCollection)
        throw new ArgumentException(nameof(block) + " is not a child of the given collection", nameof(block));

      var previous = block.PreviousBlock;
      var next = block.NextBlock;

      if (previous != null)
      {
        previous.NextBlock = next;
      }
      else
      {
        Debug.Assert(next != null, "We're removing a block from the end and there is no new last element");
        // if the previous element is null, then we're replacing the head
        Head = next;
      }

      if (next != null)
      {
        next.PreviousBlock = previous;
      }
      else
      {
        Debug.Assert(previous != null, "We're removing a block from the beginning and there is no new last element");
        // if the next element is null, then we're replacing the tail
        Tail = previous;
      }

      block.Parent = null;
      block.Index = -1;

      Count -= 1;

      Reindex(previous);
    }

    /// <summary> Resets the indices of the given blocks. </summary>
    /// <param name="startingBlock"> The block at which the re-indexing should occur. </param>
    private void Reindex(Block startingBlock)
    {
      Block currentBlock;
      int currentIndex;
      if (startingBlock != null)
      {
        currentBlock = startingBlock;
        currentIndex = startingBlock.Index;
      }
      else
      {
        currentBlock = Head;
        currentIndex = 0;
      }

      while (currentBlock != null)
      {
        currentBlock.Index = currentIndex;
        currentIndex++;

        currentBlock = currentBlock.NextBlock;
      }

      Verify();
    }

    [Conditional("DEBUG")]
    private void Verify()
    {
      Debug.Assert(Tail.Index == Count - 1, "Count is incorrect");

      int index = 0;

      foreach (var block in this)
      {
        Debug.Assert(block != null, $"Block at {index} is null");
        Debug.Assert(block.Parent == _parentCollection, $"Block at {block.Index} is not correctly parented");
        index++;
      }
    }

    /// <summary> Gets the block at the specified index.  Possibly O(n). </summary>
    /// <param name="index"> The index of the block to retrieve. </param>
    /// <returns> The block at the given index. </returns>
    public Block GetAtIndex(int index)
    {
      var current = Head;
      while (index > 0 && current != null)
      {
        index--;
        current = current.NextBlock;
      }

      return current;
    }

    /// <nodoc />
    public bool Equals(BlockLinkedList other)
    {
      if (ReferenceEquals(null, other))
        return false;
      if (ReferenceEquals(this, other))
        return true;

      if (Count != other.Count)
        return false;

      return this.SequenceEqual(other);
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
      return Equals((BlockLinkedList)obj);
    }

    /// <inheritdoc />
    public override int GetHashCode()
    {
      return _parentCollection.GetHashCode();
    }

    /// <inheritdoc />
    public IEnumerator<Block> GetEnumerator()
    {
      return new BlockIterator(this);
    }

    /// <inheritdoc />
    IEnumerator IEnumerable.GetEnumerator()
    {
      return GetEnumerator();
    }

    /// <summary> Custom implementation of a block iterator. </summary>
    public class BlockIterator : IEnumerator<Block>
    {
      [NotNull]
      private readonly BlockLinkedList _blockLinkedList;

      private bool _hasStarted;

      public BlockIterator(BlockLinkedList blockLinkedList)
      {
        if (blockLinkedList == null)
          throw new ArgumentNullException(nameof(blockLinkedList));

        _blockLinkedList = blockLinkedList;
      }

      /// <inheritdoc />
      public void Dispose()
      {
      }

      /// <inheritdoc />
      public bool MoveNext()
      {
        if (!_hasStarted)
        {
          _hasStarted = true;
          Current = _blockLinkedList.Head;
        }
        else if (Current != null)
        {
          Current = Current.NextBlock;
        }

        return Current != null;
      }

      /// <inheritdoc />
      public void Reset()
      {
        Current = null;
      }

      /// <inheritdoc />
      public Block Current { get; private set; }

      /// <inheritdoc />
      object IEnumerator.Current
      {
        get { return Current; }
      }
    }
  }
}