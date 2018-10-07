using System;
using System.Collections.Generic;
using System.Linq;
using TextRight.Core.Cursors;
using TextRight.Core.ObjectModel.Blocks.Collections;
using TextRight.Core.ObjectModel.Cursors;
using TextRight.Core.Utilities;

namespace TextRight.Core.ObjectModel.Blocks
{
  public interface IContentBlockView : IDocumentItemView
  {
    /// <summary> Returns the area consumed by the block. </summary>
    /// <returns>
    ///  A MeasuredRectangle representing the area that would be selected if the entire block was
    ///  selected.
    /// </returns>
    MeasuredRectangle MeasureSelectionBounds();
  }

  /// <summary>
  ///  A block that contains content instead of blocks, and thus supports cursors through the
  ///  content.
  /// </summary>
  public abstract class ContentBlock : Block
  {
    public abstract BlockCaret GetCaretAtStart();

    public abstract BlockCaret GetCaretAtEnd();

    /// <summary> Retrieves the IContentBlockView associated with the block. </summary>
    protected abstract IContentBlockView ContentBlockView { get; }

    /// <inheritdoc />
    public override MeasuredRectangle GetSelectionBounds()
      => ContentBlockView?.MeasureSelectionBounds() ?? MeasuredRectangle.Invalid;
  }
}