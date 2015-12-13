using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Documents;
using TextRight.ContentEditor.Core.ObjectModel.Blocks;

namespace TextRight.ContentEditor.Desktop.View
{
  /// <summary> View representation of a <see cref="TextBlock"/> </summary>
  public class TextBlockView : Paragraph, ITextBlockView
  {
    private readonly TextBlock _block;

    /// <summary> Constructor. </summary>
    /// <param name="block"> The block that this view is for. </param>
    public TextBlockView(TextBlock block)
    {
      _block = block;
      _block.Target = this;

      foreach (var span in block)
      {
        Inlines.Add(new StyledStyledTextSpanView(this, span));
      }
    }
  }
}