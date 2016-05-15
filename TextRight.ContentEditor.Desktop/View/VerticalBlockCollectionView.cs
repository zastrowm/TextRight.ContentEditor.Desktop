using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Controls;
using TextRight.ContentEditor.Core.ObjectModel;
using TextRight.ContentEditor.Core.ObjectModel.Blocks;
using TextBlock = TextRight.ContentEditor.Core.ObjectModel.Blocks.TextBlock;

namespace TextRight.ContentEditor.Desktop.View
{
  /// <summary> Implements the functionality for hosting the view of a ChildCollection </summary>
  public class VerticalBlockCollectionView : StackPanel,
                                             IBlockCollectionView
  {
    private readonly VerticalBlockCollection _blockCollection;

    public VerticalBlockCollectionView(VerticalBlockCollection blockCollection)
    {
      _blockCollection = blockCollection;
      _blockCollection.Target = this;

      // TODO make this not just for TextBlocks
      foreach (var block in _blockCollection.Children)
      {
        Children.Add(new TextBlockView((TextBlock)block));
      }
    }

    /// <inheritdoc />
    public IDocumentItem DocumentItem
      => _blockCollection;

    /// <inheritdoc />
    public void NotifyBlockInserted(Block previousSibling, Block newBlock, Block nextSibling)
    {
      var newBlockView = new TextBlockView((TextBlock)newBlock);
      Children.Insert(newBlock.Index, newBlockView);
    }

    /// <inheritdoc />
    public void NotifyBlockRemoved(Block oldPreviousSibling,
                                   Block blockRemoved,
                                   Block oldNextSibiling,
                                   int indexOfBlockRemoved)
    {
      var view = (TextBlockView)((TextBlock)blockRemoved).Target;
      Children.Remove(view);
    }
  }
}