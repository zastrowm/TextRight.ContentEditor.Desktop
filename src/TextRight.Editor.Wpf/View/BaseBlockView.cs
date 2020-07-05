using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using TextRight.Core.ObjectModel;

namespace TextRight.Editor.Wpf.View
{
  /// <summary> Base view for blocks in the document. </summary>
  public abstract class BaseBlockView : FrameworkElement, ILayoutable
  {
    private ChangeIndex _layoutIndex;

    /// <inheritdoc/>
    public ChangeIndex LayoutIndex
      => _layoutIndex;

    /// <summary> Updates <see cref="LayoutIndex"/> to the next value. </summary>
    public void MarkLayoutChanged()
    {
      _layoutIndex = _layoutIndex.Next();
    }

    /// <summary> The document item for the view. </summary>
    public abstract IDocumentItem DocumentItem { get; }
  }
}