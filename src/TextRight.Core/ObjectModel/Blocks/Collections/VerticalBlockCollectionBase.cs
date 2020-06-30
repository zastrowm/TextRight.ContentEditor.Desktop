using System;
using System.Collections.Generic;
using System.Linq;
using TextRight.Core.Utilities;

namespace TextRight.Core.ObjectModel.Blocks.Collections
{
  /// <summary> Base class for Vertical Block Collections. </summary>
  /// <typeparam name="TBlockView"> The type of view that observes the given collection. </typeparam>
  public abstract class VerticalBlockCollectionBase<TBlockView> : VerticalBlockCollection,
                                                                  IDocumentItem<TBlockView>
    where TBlockView : IDocumentItemView, IBlockCollectionView
  {
    protected VerticalBlockCollectionBase(Block firstChild)
      : base(firstChild)
    {
      
    }

    /// <summary>
    ///  The object that receives all notifications of changes from this instance.
    /// </summary>
    public TBlockView Target { get; set; }

    /// <inheritdoc />
    IDocumentItemView IDocumentItem.DocumentItemView
      => Target;

    /// <inheritdoc />
    protected override void OnBlockInserted(Block previousBlock, Block newBlock, Block nextBlock)
    {
      Target?.NotifyBlockInserted(previousBlock, newBlock, nextBlock);
    }

    /// <inheritdoc />
    protected override void OnBlockRemoved(Block previousBlock,
                                           Block removedBlock,
                                           Block nextBlock,
                                           int indexOfRemovedBlock)
    {
      Target?.NotifyBlockRemoved(previousBlock, removedBlock, nextBlock, indexOfRemovedBlock);
    }

    /// <inheritdoc />
    public override MeasuredRectangle GetSelectionBounds()
    {
      return Target?.MeasureBounds() ?? MeasuredRectangle.Invalid;
    }
  }
}