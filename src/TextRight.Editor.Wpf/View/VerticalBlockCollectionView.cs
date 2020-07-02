using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using TextRight.Core.ObjectModel;
using TextRight.Core.ObjectModel.Blocks;
using TextRight.Core.ObjectModel.Blocks.Collections;
using TextRight.Core.ObjectModel.Blocks.Text;
using TextRight.Core.Utilities;

namespace TextRight.Editor.Wpf.View
{
  /// <summary> Implements the functionality for hosting the view of a ChildCollection </summary>
  public class VerticalBlockCollectionView : StackPanel,
                                             IBlockCollectionView
  {
    private readonly RootBlockCollection _blockCollection;
    private readonly DocumentEditorContextView _root;

    public VerticalBlockCollectionView(DocumentEditorContextView root, RootBlockCollection blockCollection)
    {
      _root = root;
      _blockCollection = blockCollection;
      _blockCollection.Tag = this;

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
      var newBlockView = _root.CreateViewFor(newBlock);
      Children.Insert(newBlock.Index, newBlockView);
    }

    /// <inheritdoc />
    public void NotifyBlockRemoved(Block oldPreviousSibling,
                                   Block blockRemoved,
                                   Block oldNextSibling,
                                   int indexOfBlockRemoved)
    {
      var view = (FrameworkElement)((IDocumentItem)blockRemoved).Tag;
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