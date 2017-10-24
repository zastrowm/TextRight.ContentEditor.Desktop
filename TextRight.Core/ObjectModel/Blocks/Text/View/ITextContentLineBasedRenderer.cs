using System;
using System.Collections.Generic;
using System.Linq;
using TextRight.Core.Utilities;

namespace TextRight.Core.ObjectModel.Blocks.Text.View
{
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