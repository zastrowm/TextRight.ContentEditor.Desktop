using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using TextRight.ContentEditor.Core.ObjectModel.Blocks;

namespace TextRight.ContentEditor.Desktop.View
{
  /// <summary> Responsible for finding the appropriate view for each Block. </summary>
  public class ViewFactory
  {
    private readonly Dictionary<Type, Func<DocumentEditorContextView, Block, FrameworkElement>> _lookup
      = new Dictionary<Type, Func<DocumentEditorContextView, Block, FrameworkElement>>();

    public ViewFactory()
    {
      Add<ParagraphBlock>((c, b) => new ParagraphView(c, b));
      Add<HeadingBlock>((c, b) => new HeadingBlockView(c, b));
    }

    /// <summary> Adds a factory function for the given block type. </summary>
    public void Add<TBlock>(Func<DocumentEditorContextView, TBlock, FrameworkElement> creator)
      where TBlock : Block
    {
      _lookup.Add(typeof(TBlock), (context, block) => creator.Invoke(context, (TBlock)block));
    }

    /// <summary> Gets a view that can be used for the given block. </summary>
    public FrameworkElement GetViewFor(DocumentEditorContextView context, Block block)
    {
      Func<DocumentEditorContextView, Block, FrameworkElement> creator;
      if (!_lookup.TryGetValue(block.GetType(), out creator))
        throw new KeyNotFoundException();

      return creator.Invoke(context, block);
    }
  }
}