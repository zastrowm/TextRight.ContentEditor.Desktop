using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using TextRight.Core.Blocks;
using TextRight.Core.Cursors;
using TextRight.Core.ObjectModel;
using TextRight.Core.ObjectModel.Blocks;
using TextRight.Core.ObjectModel.Blocks.Collections;
using TextRight.Core.ObjectModel.Cursors;
using TextRight.Core.Utilities;

using TextBlock = System.Windows.Controls.TextBlock;

namespace TextRight.Editor.Wpf.View
{
  public class ListItemBlockView : Grid,
                                   IBlockCollectionView
  {
    private readonly DocumentEditorContextView _root;
    private readonly ListItemBlock _listItemBlock;
    private readonly StackPanel _childContents;
    private readonly TextBlock _listSymbol;

    public ListItemBlockView(DocumentEditorContextView root, ListItemBlock listItemBlock)
    {
      _root = root;
      _listItemBlock = listItemBlock;
      _childContents = new StackPanel();
      SetRow(_childContents, 0);
      SetColumn(_childContents, 1);

      _listSymbol = new TextBlock();
      _listSymbol.Text = " - ";
      SetRow(_listSymbol, 0);
      SetColumn(_listSymbol, 0);

      Children.Add(_listSymbol);
      Children.Add(_childContents);

      ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Auto });
      ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });

      foreach (var block in listItemBlock.Children)
      {
        var view = _root.CreateViewFor(block);
        Children.Add(view);
      }
    }

    /// <inheritdoc />
    public IDocumentItem DocumentItem
      => _listItemBlock;

    /// <inheritdoc />
    public void NotifyBlockInserted(BlockInsertedEventArgs args)
    {
      var newBlock = args.NewBlock;
      _childContents.Children.Insert(newBlock.Index, _root.CreateViewFor(newBlock));
    }

    /// <inheritdoc />
    public void NotifyBlockRemoved(BlockRemovedEventArgs args)
    {
      var view = (FrameworkElement)((IDocumentItem)args.BlockRemoved).Tag;
      _childContents.Children.Remove(view);
    }

    /// <inheritdoc />
    public MeasuredRectangle MeasureSelectionBounds()
      => CollectionViewHelper.MeasureSelectionBounds(_root, this);

    /// <inheritdoc />
    public BlockCaret GetCaretFromBottom(CaretMovementMode movementMode)
      => CollectionViewHelper.GetCaretFromBottom(_childContents.Children, movementMode);

    /// <inheritdoc />
    public BlockCaret GetCaretFromTop(CaretMovementMode movementMode)
      => CollectionViewHelper.GetCaretFromTop(_childContents.Children, movementMode);
  }
}