using System;
using System.Collections.Generic;
using System.Linq;

namespace TextRight.ContentEditor.Core.ObjectModel.Blocks
{
  /// <summary> Walks a document navigating between blocks. </summary>
  public static class BlockTreeWalker
  {
    /// <summary> Get the next block in the tree that is not a container block. </summary>
    /// <param name="block"> The block from which the search should start. </param>
    /// <returns> The next block that is not a container block. </returns>
    public static Block GetNextNonContainerBlock(Block block)
    {
      while (true)
      {
        var nextBlock = block.GetNextBlock();

        if (nextBlock != null)
        {
          return GetFirstDescendantNonContainerBlock(nextBlock);
        }

        if (block.Parent == null)
        {
          return null;
        }

        block = block.Parent;
      }
    }

    /// <summary> Get the previous block in the tree that is not a container block. </summary>
    /// <param name="block"> The block from which the search should start. </param>
    /// <returns> The previous block that is not a container block. </returns>
    public static Block GetPreviousNonContainerBlock(Block block)
    {
      while (true)
      {
        var previousBlock = block.GetPreviousBlock();

        if (previousBlock != null)
        {
          return GetLastDescendantNonContainerBlock(previousBlock);
        }

        if (block.Parent == null)
        {
          return null;
        }

        block = block.Parent;
      }
    }

    /// <summary> Get the first descendant block that is not a container, inclusive search. </summary>
    /// <param name="block"> The block from which the search should start. </param>
    /// <returns> The first non container block starting at block (and possibly returning block). </returns>
    private static Block GetFirstDescendantNonContainerBlock(Block block)
    {
      var collectionBlock = block as BlockCollection;
      while (collectionBlock != null)
      {
        block = collectionBlock.FirstBlock;
        collectionBlock = block as BlockCollection;
      }

      return block;
    }

    /// <summary> Gets the last descendant block that is not a container, inclusive search. </summary>
    /// <param name="block"> The block from which the search should start. </param>
    /// <returns> The last non-container block starting at block 9and possibly returning block) </returns>
    private static Block GetLastDescendantNonContainerBlock(Block block)
    {
      var collectionBlock = block as BlockCollection;
      while (collectionBlock != null)
      {
        block = collectionBlock.LastBlock;
        collectionBlock = block as BlockCollection;
      }

      return block;
    }
  }
}