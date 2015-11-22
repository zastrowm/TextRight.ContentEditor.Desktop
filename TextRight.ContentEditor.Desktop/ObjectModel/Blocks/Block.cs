using System;
using System.Collections.Generic;
using System.Linq;

namespace TextRight.ContentEditor.Desktop.ObjectModel.Blocks
{
  /// <summary>
  ///   Represents a top-level block.
  /// </summary>
  public abstract class Block
  {
    /// <summary>
    ///   The index of the block within the parent's collection.
    /// </summary>
    public int Index { get; set; }

    /// <summary>
    ///   The block that owns this block.
    /// </summary>
    public BlockCollection Parent { get; set; }

    /// <summary> True if the block is the first child of the parent collection. </summary>
    public bool IsFirst
      => Parent == null || Index == 0;

    /// <summary> True if the block is the last child of the parent collection. </summary>
    public bool IsLast
      => Parent == null || GetNextBlock() == null;

    /// <summary> Get the previous block in the block collection. </summary>
    public Block GetPreviousBlock()
      => Parent?.GetPreviousBlock(this);

    /// <summary> Get the next block in the block collection. </summary>
    public Block GetNextBlock()
      => Parent?.GetNextBlock(this);

    /// <summary> The type of the block. </summary>
    public abstract BlockType BlockType { get; }

    /// <summary> Gets a block-specific iterator. </summary>
    /// <returns> An iterate that can move through the block. </returns>
    public abstract IBlockContentCursor GetCursor();
  }
}