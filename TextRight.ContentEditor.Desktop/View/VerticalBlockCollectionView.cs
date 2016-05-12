using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Documents;
using TextRight.ContentEditor.Core.ObjectModel.Blocks;
using Block = TextRight.ContentEditor.Core.ObjectModel.Blocks.Block;

namespace TextRight.ContentEditor.Desktop.View
{
  /// <summary> Implements the functionality for hosting the view of a ChildCollection </summary>
  public class VerticalBlockCollectionView : FlowDocument,
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
        Blocks.Add(new TextBlockView((TextBlock)block));
      }
    }

    /// <inheritdoc />
    public void NotifyBlockInserted(Block previousSibling, Block newBlock, Block nextSibling)
    {
      var newBlockView = new TextBlockView((TextBlock)newBlock);

      if (previousSibling != null)
      {
        var textBlockView = (TextBlockView)((TextBlock)previousSibling).Target;
        Blocks.InsertAfter(textBlockView, newBlockView);
        return;
      }

      if (nextSibling != null)
      {
        var textBlockView = (TextBlockView)((TextBlock)nextSibling).Target;
        Blocks.InsertBefore(textBlockView, newBlockView);
        return;
      }
    }

    /// <inheritdoc />
    public void NotifyBlockRemoved(Block oldPreviousSibling,
                                   Block blockRemoved,
                                   Block oldNextSibiling,
                                   int indexOfBlockRemoved)
    {
      var view = (TextBlockView)((TextBlock)blockRemoved).Target;
      Blocks.Remove(view);
    }
  }
}