using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using TextRight.Core.Cursors;
using TextRight.Core.ObjectModel;
using TextRight.Core.ObjectModel.Blocks;
using TextRight.Core.ObjectModel.Blocks.Collections;
using TextRight.Core.ObjectModel.Blocks.Text;
using TextRight.Core.ObjectModel.Cursors;
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

      foreach (var block in _blockCollection.Children)
      {
        Children.Add(_root.CreateViewFor(block));
      }
    }

    /// <inheritdoc />
    public IDocumentItem DocumentItem
      => _blockCollection;

    /// <inheritdoc />
    public void NotifyBlockInserted(BlockInsertedEventArgs args)
    {
      var newBlockView = _root.CreateViewFor(args.NewBlock);
      Children.Insert(args.NewBlock.Index, newBlockView);
    }

    /// <inheritdoc />
    public void NotifyBlockRemoved(BlockRemovedEventArgs args)
    {
      var view = (FrameworkElement)args.BlockRemoved.Tag;
      Debug.Assert(Children.Contains(view));
      Children.Remove(view);
    }

    /// <inheritdoc />
    public MeasuredRectangle MeasureSelectionBounds()
      => CollectionViewHelper.MeasureSelectionBounds(_root, this);

    /// <inheritdoc />
    public BlockCaret GetCaretFromBottom(CaretMovementMode movementMode)
      => CollectionViewHelper.GetCaretFromBottom(Children, movementMode);

    /// <inheritdoc />
    public BlockCaret GetCaretFromTop(CaretMovementMode movementMode)
      => CollectionViewHelper.GetCaretFromTop(Children, movementMode);
  }
}