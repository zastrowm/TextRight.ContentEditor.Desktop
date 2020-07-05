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
  public interface ITextCaretMeasurer : IBlockView
  {
    // <summary> Measures the bounds of a specific caret position.  </summary>
    MeasuredRectangle MeasureTextPosition(TextCaret caret);
  }
}