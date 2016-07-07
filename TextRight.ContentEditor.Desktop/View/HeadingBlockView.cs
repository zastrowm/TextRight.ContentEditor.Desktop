using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;
using TextRight.ContentEditor.Core.ObjectModel;
using TextRight.ContentEditor.Core.ObjectModel.Blocks;

namespace TextRight.ContentEditor.Desktop.View
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

      TextFont = new Typeface("Calibre");
    }

    /// <inheritdoc />
    public override IDocumentItem DocumentItem
      => _block;

    /// <inheritdoc/>
    public void NotifyLevelChanged()
    {
      // TODO
    }
  }
}