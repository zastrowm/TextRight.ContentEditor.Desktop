using System;
using System.Collections.Generic;
using System.Linq;
using TextRight.Core.Utilities;

namespace TextRight.Core.ObjectModel.Blocks.Text
{
  /// <summary> Base class for a block that presents text. </summary>
  /// <typeparam name="TBlockView"> The block-specific view that should knows how to present the
  ///  block. </typeparam>
  public abstract class TextBlockBase<TBlockView> : TextBlock,
                                                    IDocumentItem<TBlockView>
    where TBlockView : IDocumentItemView, ITextBlockView
  {
    /// <inheritdoc/>
    public TBlockView Target { get; set; }

    /// <inheritdoc />
    IDocumentItemView IDocumentItem.DocumentItemView
      => Target;

    /// <inheritdoc />
    public override MeasuredRectangle GetBounds()
    {
      return Target?.MeasureBounds() ?? MeasuredRectangle.Invalid;
    }

    /// <inheritdoc/>
    protected internal override void OnFragmentInserted(StyledTextFragment previous,
                                               StyledTextFragment fragment,
                                               StyledTextFragment next)
    {
      Target?.NotifyBlockInserted(fragment.Previous, fragment, fragment.Next);
    }
  }
}