using System;
using System.Collections.Generic;
using System.Linq;
using TextRight.Core.Utilities;

namespace TextRight.Core.ObjectModel.Blocks.Text.View
{
  /// <summary> View for a TextBlockContent. </summary>
  public interface ITextBlockContentView : IContentBlockView, IDocumentItemView
  {
    /// <summary> Measures the given caret position. </summary>
    /// <param name="caret"> The caret position to measure. </param>
    /// <returns> A MeasuredRectangle representing the caret position. </returns>
    MeasuredRectangle Measure(TextCaret caret);

    /// <summary> The first line in the renderer. </summary>
    ITextLine FirstTextLine { get; }

    /// <summary> The second line in the renderer. </summary>
    ITextLine LastTextLine { get; }

    /// <summary> Gets the line on which the caret appears. </summary>
    /// <param name="caret"> The caret for which the associated line should be retrieved. </param>
    /// <returns> The line for. </returns>
    ITextLine GetLineFor(TextCaret caret);
  }
}