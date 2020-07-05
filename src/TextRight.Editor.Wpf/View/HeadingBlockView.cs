using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using TextRight.Core.Blocks;
using TextRight.Core.ObjectModel;
using TextRight.Core.ObjectModel.Blocks.Text;
using TextRight.Core.Events;

namespace TextRight.Editor.Wpf.View
{
  /// <summary />
  public class HeadingBlockView : BaseTextBlockView,
                                  IChangeListener
  {
    private readonly HeadingBlock _block;

    public HeadingBlockView(DocumentEditorContextView root, HeadingBlock block)
      : base(root, block)
    {
      _block = block;
      _block.Tag = this;
      Padding = new Thickness(10);

      SyncTextSize();
      //Text.TextFont = new Typeface("Segoe UI Semibold");
    }

    private void SyncTextSize()
    {
      FontSize = 30 - _block.HeadingLevel * 2;
      InvalidateMeasure();
    }

    /// <inheritdoc />
    public override IDocumentItem DocumentItem
      => _block;

    public void HandleEvent(ChangeEvent changeEvent)
    {
      if (changeEvent is PropertyChangedEvent<int> propertyChange
          && propertyChange.Descriptor == HeadingBlock.Descriptor.HeadingLevelProperty)
      {
        SyncTextSize();
      }
    }
  }
}