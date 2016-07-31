using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TextRight.ContentEditor.Core.ObjectModel.Cursors;
using TextRight.ContentEditor.Core.ObjectModel.Serialization;
using TextRight.ContentEditor.Core.Utilities;

namespace TextRight.ContentEditor.Core.ObjectModel.Blocks
{
  /// <summary> A BlockCollection where the blocks are stacked vertically. </summary>
  public abstract class VerticalBlockCollection : BlockCollection
  {
    /// <summary> Default constructor. </summary>
    internal VerticalBlockCollection()
    {
    }

    /// <inheritdoc />
    public override Block GetBlockTo(BlockDirection direction, Block block)
    {
      switch (direction)
      {
        case BlockDirection.Backward:
        case BlockDirection.Top:
          return block.PreviousBlock;
        case BlockDirection.Forward:
        case BlockDirection.Bottom:
          return block.NextBlock;
        default:
          throw new ArgumentOutOfRangeException(nameof(direction), direction, null);
      }
    }

    /// <inheritdoc />
    public override Block Clone()
    {
      var clone = (VerticalBlockCollection)Descriptor.Descriptor.CreateInstance();

      foreach (var block in Children)
      {
        clone.Append(block.Clone());
      }

      // remove the original first block
      clone.RemoveBlock(clone.FirstBlock);

      return clone;
    }

    /// <inheritdoc />
    protected override void SerializeInto(SerializeNode node)
    {
      foreach (var child in Children)
      {
        node.Children.Add(child.Serialize());
      }
    }

    /// <inheritdoc />
    public override void Deserialize(SerializationContext context, SerializeNode node)
    {
      // TODO should we be removing existing children?
      foreach (var nodeChild in node.Children)
      {
        var childBlock = context.Deserialize(nodeChild);
        Append(childBlock);
      }
    }

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
  }

  /// <summary> Holds the view representation of the BlockCollection. </summary>
  public interface IBlockCollectionView : IDocumentItemView
  {
    /// <summary> Notifies a block inserted. </summary>
    void NotifyBlockInserted(Block previousSibling, Block newBlock, Block nextSibling);

    /// <summary> Invoked when a block has been removed from the collection. </summary>
    void NotifyBlockRemoved(Block oldPreviousSibling, Block blockRemoved, Block oldNextSibiling, int indexOfBlockRemoved);

    /// <summary> Returns the area consumed by the block. </summary>
    /// <returns> A MeasuredRectangle representing the area required to display the block. </returns>
    MeasuredRectangle MeasureBounds();
  }
}