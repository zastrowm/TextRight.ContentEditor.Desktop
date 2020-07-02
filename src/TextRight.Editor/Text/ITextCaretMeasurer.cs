using System;
using System.Collections.Generic;
using System.Linq;
using TextRight.Core.ObjectModel.Blocks.Text;
using TextRight.Core.Utilities;

namespace TextRight.Editor.Text
{
  /// <summary>
  ///   Interface that enables <see cref="TextCaretMeasurerHelper"/> to unify caret measuring for multiple
  ///   editor implementations.
  /// </summary>
  public interface ITextCaretMeasurer
  {
    /// <summary> Gets the bounds of the block if the entire thing was selected. </summary>
    /// <returns> The bounds that encompass the area consumed by the block. </returns>
    MeasuredRectangle MeasureSelectionBounds();

    MeasuredRectangle MeasureTextPosition(TextCaret caret);
  }
}