using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using TextRight.ContentEditor.Core.ObjectModel;
using TextRight.ContentEditor.Core.ObjectModel.Blocks;
using TextRight.ContentEditor.Core.Utilities;

namespace TextRight.ContentEditor.Desktop.View
{
  /// <summary> Implements the functionality for hosting the view of a ChildCollection </summary>
  public class VerticalBlockCollectionView : StackPanel,
                                             IBlockCollectionView
  {
    private readonly VerticalBlockCollection _blockCollection;
    private readonly DocumentEditorContextView _root;

    public VerticalBlockCollectionView(DocumentEditorContextView root, VerticalBlockCollection blockCollection)
    {
      _root = root;
      _blockCollection = blockCollection;
      _blockCollection.Target = this;

      // TODO make this not just for TextBlocks
      foreach (var block in _blockCollection.Children)
      {
        Children.Add(new ParagraphView(_root, (ParagraphBlock)block));
      }
    }

    /// <inheritdoc />
    public IDocumentItem DocumentItem
      => _blockCollection;

    /// <inheritdoc />
    public void NotifyBlockInserted(Block previousSibling, Block newBlock, Block nextSibling)
    {
      var newBlockView = _root.ViewFactory.GetViewFor(_root, newBlock);
      Children.Insert(newBlock.Index, newBlockView);
    }

    /// <inheritdoc />
    public void NotifyBlockRemoved(Block oldPreviousSibling,
                                   Block blockRemoved,
                                   Block oldNextSibiling,
                                   int indexOfBlockRemoved)
    {
      var view = (FrameworkElement)((IDocumentItem)blockRemoved).DocumentItemView;
      Children.Remove(view);
    }

    /// <inheritdoc />
    public MeasuredRectangle MeasureBounds()
    {
      var offset = TransformToAncestor(_root).Transform(new Point(0, 0));

      return new MeasuredRectangle()
             {
               X = offset.X,
               Y = offset.Y,
               Width = ActualWidth,
               Height = ActualHeight
             };
    }
  }
}