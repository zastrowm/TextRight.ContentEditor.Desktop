using System;
using System.Collections.Generic;
using System.Linq;
using TextRight.ContentEditor.Core.ObjectModel.Serialization;

namespace TextRight.ContentEditor.Core.ObjectModel.Blocks
{
  /// <summary> A block that holds text formatted as a paragraph. </summary>
  public class ParagraphBlock : TextBlockBase<ITextBlockView>
  {
    /// <inheritdoc />
    public override string ContentType { get; }
      = "paragraph";

    /// <inheritdoc/>
    protected override TextBlock SuperClone()
    {
      return new ParagraphBlock();
    }

    /// <inheritdoc/>
    protected override void SerializeToNode(SerializeNode node)
    {
      // no-op
    }

    /// <inheritdoc/>
    public override TextBlockAttributes GetAttributes()
    {
      return new Attributes();
    }

    /// <summary />
    private class Attributes : TextBlockAttributes
    {
      public override TextBlock CreateInstance()
      {
        return new ParagraphBlock();
      }
    }
  }
}