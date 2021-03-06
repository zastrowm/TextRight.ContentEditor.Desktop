using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using TextRight.Core.Blocks;
using TextRight.Core.ObjectModel.Blocks;
using TextRight.Core.ObjectModel.Blocks.Text;

namespace TextRight.Editor.Wpf.View
{
  /// <summary> Responsible for finding the appropriate view for each Block. </summary>
  public class ViewFactory
  {
    private readonly Dictionary<Type, Func<DocumentEditorContextView, Block, FrameworkElement>> _lookup
      = new Dictionary<Type, Func<DocumentEditorContextView, Block, FrameworkElement>>();

    public ViewFactory()
    {
      Add<ParagraphBlock, ParagraphView>((c, b) => new ParagraphView(c, b));
      Add<HeadingBlock, HeadingBlockView>((c, b) => new HeadingBlockView(c, b));
      Add<ListItemBlock, ListItemBlockView>((c, b) => new ListItemBlockView(c, b));
    }

    /// <summary> Adds a factory function for the given block type. </summary>
    public void Add<TBlock, TView>(Func<DocumentEditorContextView, TBlock, FrameworkElement> creator)
      where TBlock : Block
      where TView : IBlockView
    {
      _lookup.Add(typeof(TBlock), (context, block) => creator.Invoke(context, (TBlock)block));
    }

    /// <summary> Gets a view that can be used for the given block. </summary>
    public FrameworkElement GetViewFor(DocumentEditorContextView context, Block block)
    {
      if (!_lookup.TryGetValue(block.GetType(), out var creator))
        throw new KeyNotFoundException() { Data = { { "Key", block.GetType() } } };

      return creator.Invoke(context, block);
    }
  }
}