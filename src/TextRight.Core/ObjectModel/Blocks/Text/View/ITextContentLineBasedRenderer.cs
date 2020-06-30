using System;
using System.Collections.Generic;
using System.Linq;
using TextRight.Core.Utilities;

namespace TextRight.Core.ObjectModel.Blocks.Text.View
{
  /// <summary>
  ///  Represents a visual line that can handle measuring a specific type of caret.
  /// </summary>
  /// <typeparam name="TCaret"> The type of caret that can be measured. </typeparam>
  public interface IVisualLine<TCaret>
  {
    /// <summary> The height of the line. </summary>
    double Height { get; }

    /// <summary> The bounds of the line, including the height and width. </summary>
    MeasuredRectangle GetBounds();

    /// <summary> Retrieves the next line in the view, or null if this is the last line. </summary>
    IVisualLine<TCaret> Next { get; }

    /// <summary> Retrieves the previous line in the view, or null if this is the first line. </summary>
    IVisualLine<TCaret> Previous { get; }

    /// <summary> Finds the caret closest to the given xPosition. </summary>
    /// <param name="xPosition"> The position for which a caret should be returned. </param>
    TCaret FindClosestTo(double xPosition);

    /// <summary> Measures the specified caret position. </summary>
    /// <param name="caret"> The caret position to measure. </param>
    /// <returns> The measured size of the caret position. </returns>
    MeasuredRectangle GetMeasurement(TCaret caret);
  }
}