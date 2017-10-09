using System;
using System.Collections.Generic;
using System.Linq;
using TextRight.Core.Utilities;

namespace TextRight.Core.ObjectModel.Blocks.Text.View
{
  /// <summary> A renderer that renders its contents via lines. </summary>
  public interface ILineBasedRenderer
  {
    /// <summary> The first line in the renderer. </summary>
    ITextLine FirstTextLine { get; }  

    /// <summary> The second line in the renderer. </summary>
    ITextLine LastTextLine { get; }

    /// <summary> Gets the line on which the caret appears. </summary>
    /// <param name="caret"> The caret for which the associated line should be retrieved. </param>
    /// <returns> The line for. </returns>
    ITextLine GetLineFor(TextCaret caret);
  }

  public interface ITextLine
  {
    double Height { get; }

    MeasuredRectangle GetBounds();

    ITextLine Next { get; }
    ITextLine Previous { get; }

    TextCaret FindClosestTo(double xPosition);

    MeasuredRectangle GetMeasurement(TextCaret caret);
  }
}