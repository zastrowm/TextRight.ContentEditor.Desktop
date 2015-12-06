using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace TextRight.ContentEditor.Desktop.ObjectModel.Blocks
{
  /// <summary> Holds a collection of blocks. </summary>
  public class BlockCollection : Block, IEnumerable<Block>
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

    public void Append(Block block)
    {
      block.Parent = this;

      _childrenCollection.Add(block);
      block.Index = _childrenCollection.Count - 1;
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

    /// <summary>
    ///   Reset the index of each child in the block collection.
    /// </summary>
    private void ReIndexChildren()
    {
      for (var i = 0; i < _childrenCollection.Count; i++)
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
  }
}