using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JetBrains.Annotations;
using TextRight.ContentEditor.Core.Editing.Commands;
using TextRight.ContentEditor.Core.ObjectModel.Serialization;

namespace TextRight.ContentEditor.Core.ObjectModel.Blocks
{
  /// <summary> A BlockCollection where the blocks are stacked vertically. </summary>
  public class VerticalBlockCollection : BlockCollection,
                                         ICommandProcessorHook,
                                         IDocumentItem<IBlockCollectionView>
  {
    /// <summary>
    ///  The object that receives all notifications of changes from this instance.
    /// </summary>
    [CanBeNull]
    public IBlockCollectionView Target { get; set; }

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
    public override ICursorPool CursorPool
    {
      get { throw new NotImplementedException(); }
    }

    /// <inheritdoc />
    protected override IBlockContentCursor CreateCursorOverride()
    {
      throw new NotImplementedException();
    }

    /// <inheritdoc />
    public override Block Clone()
    {
      var clone = new VerticalBlockCollection();

      foreach (var block in Children)
      {
        clone.Append(block.Clone());
      }

      // remove the original first block
      clone.RemoveBlock(clone.FirstBlock);

      return clone;
    }

    /// <inheritdoc />
    public override SerializeNode SerializeAsNode()
    {
      var node = new SerializeNode(typeof(VerticalBlockCollection));
      foreach (var child in Children)
      {
        node.Children.Add(child.SerializeAsNode());
      }
      return node;
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

    /// <inheritdoc />
    public ICommandProcessor CommandProcessor
      => VerticalBlockCollectionCommandProcessor.Instance;
  }

  /// <summary> Holds the view representation of the BlockCollection. </summary>
  public interface IBlockCollectionView : IDocumentItemView
  {
    /// <summary> Notifies a block inserted. </summary>
    void NotifyBlockInserted(Block previousSibling, Block newBlock, Block nextSibling);

    /// <summary> Invoked when a block has been removed from the collection. </summary>
    void NotifyBlockRemoved(Block oldPreviousSibling, Block blockRemoved, Block oldNextSibiling, int indexOfBlockRemoved);
  }
}