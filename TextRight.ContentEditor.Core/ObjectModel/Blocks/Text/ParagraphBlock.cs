using System;
using System.Collections.Generic;
using System.Linq;

namespace TextRight.ContentEditor.Core.ObjectModel.Blocks
{
  /// <summary> A block that holds text formatted as a paragraph. </summary>
  public class ParagraphBlock : TextBlockBase<ITextBlockView>
  {
    /// <inheritdoc/>
    protected override TextBlock SuperClone()
    {
      return new ParagraphBlock();
    }

    /// <inheritdoc />
    public override string MimeType { get; }
      = "text/plain";
  }
}