using System;
using System.Collections.Generic;
using System.Linq;
using TextRight.Core.Utilities;

namespace TextRight.Core.ObjectModel.Blocks.Collections
{
  /// <summary> Base class for Vertical Block Collections. </summary>
  /// <typeparam name="TBlockView"> The type of view that observes the given collection. </typeparam>
  public abstract class VerticalBlockCollectionBase<TBlockView> : VerticalBlockCollection,
                                                                  IDocumentItem
    where TBlockView : class, IBlockCollectionView
  {
    protected VerticalBlockCollectionBase(Block firstChild)
      : base(firstChild)
    {
      
    }

    private TBlockView Target
      => Tag as TBlockView;

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
  }
}