using System;
using System.Collections.Generic;
using System.Linq;
using TextRight.ContentEditor.Core.Utilities;

namespace TextRight.ContentEditor.Core.ObjectModel.Blocks
{
  /// <summary> Base class for Vertical Block Collections. </summary>
  /// <typeparam name="TBlockView"> The type of view that observes the given collection. </typeparam>
  public abstract class VerticalBlockCollectionBase<TBlockView> : VerticalBlockCollection,
                                                                  IDocumentItem<TBlockView>
    where TBlockView : IDocumentItemView, IBlockCollectionView
  {
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
    public override MeasuredRectangle GetBounds()
    {
      return Target?.MeasureBounds() ?? MeasuredRectangle.Invalid;
    }
  }
}