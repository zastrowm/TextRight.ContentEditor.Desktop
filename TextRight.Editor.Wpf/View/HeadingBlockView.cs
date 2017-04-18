using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using TextRight.Core.Blocks;
using TextRight.Core.ObjectModel;
using TextRight.Core.ObjectModel.Blocks.Text;

namespace TextRight.Editor.Wpf.View
{
  /// <summary />
  public class HeadingBlockView : BaseTextBlockView,
                                  IHeadingBlockView
  {
    private readonly HeadingBlock _block;

    public HeadingBlockView(DocumentEditorContextView root, HeadingBlock block)
      : base(root, block)
    {
      _block = block;
      _block.Target = this;

      Margin = new Thickness(10);

      SyncTextSize();
      //Text.TextFont = new Typeface("Segoe UI Semibold");
    }

    private void SyncTextSize()
    {
      //Text.TextFontSize = 20 - _block.HeadingLevel * 2;

      InvalidateMeasure();
    }

    /// <inheritdoc />
    public override IDocumentItem DocumentItem
      => _block;

    /// <inheritdoc/>
    public void NotifyLevelChanged()
    {
      SyncTextSize();
    }
  }
}