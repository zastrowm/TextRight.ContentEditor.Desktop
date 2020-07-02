using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using TextRight.Core.Blocks;
using TextRight.Core.ObjectModel;
using TextRight.Core.ObjectModel.Blocks;
using TextRight.Core.Utilities;

using TextBlock = System.Windows.Controls.TextBlock;

namespace TextRight.Editor.Wpf.View
{
  public class ListItemBlockView : Grid,
                                   IListItemBlockView
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

      // TODO make this not just for TextBlocks
      foreach (var block in listItemBlock.Children)
      {
        var view = _root.CreateViewFor(block);
        Children.Add(view);
      }
    }

    /// <inheritdoc />
    public IDocumentItem DocumentItem
      => _listItemBlock;

    public void NotifyBlockInserted(Block previousSibling, Block newBlock, Block nextSibling)
    {
      var newBlockView = _root.CreateViewFor(newBlock);
      _childContents.Children.Insert(newBlock.Index, newBlockView);
    }

    public void NotifyBlockRemoved(Block oldPreviousSibling,
                                   Block blockRemoved,
                                   Block oldNextSibling,
                                   int indexOfBlockRemoved)
    {
      var view = (FrameworkElement)((IDocumentItem)blockRemoved).Tag;
      _childContents.Children.Remove(view);
    }

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