using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using TextRight.ContentEditor.Core.ObjectModel;
using TextRight.ContentEditor.Core.ObjectModel.Blocks;

namespace TextRight.ContentEditor.Desktop.View
{
  /// <summary> Holds the view for TextBlock. </summary>
  public class ParagraphView : BaseTextBlockView
  {
    private readonly ParagraphBlock _block;

    public ParagraphView(DocumentEditorContextView root, ParagraphBlock block)
      : base(root, block)
    {
      Margin = new Thickness(10);

      _block = block;
      _block.Target = this;
    }

    /// <inheritdoc />
    public override IDocumentItem DocumentItem
      => _block;
  }
}