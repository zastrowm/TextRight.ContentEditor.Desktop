using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Documents;
using TextRight.ContentEditor.Desktop.ObjectModel.Blocks;
using TextRight.ContentEditor.Desktop.Utilities;

namespace TextRight.ContentEditor.Desktop.View
{
  /// <summary>
  ///  Associates a WPF Run with a TextSpan and keeps them in sync.
  /// </summary>
  public class StyledTextSpanView : Run, ITextSpanResponder
  {
    private readonly StyledTextSpan _span;

    /// <summary> Constructor. </summary>
    /// <param name="span"> The span to keep synchronized. </param>
    public StyledTextSpanView(StyledTextSpan span)
    {
      _span = span;
      _span.Target = this;
    }

    /// <inheritdoc/>
    public void TextUpdated(StyledTextSpan span)
    {
      Text = _span.Text;
    }

    /// <inheritdoc/>
    public MeasuredRectangle Measure(int offset)
    {
      // TODO Debug/Assert not null
      var rect = ContentStart.GetPositionAtOffset(offset).GetCharacterRect(LogicalDirection.Forward);
      return new MeasuredRectangle()
             {
               X = rect.X,
               Y = rect.Y,
               Width = rect.Width,
               Height = rect.Height
             };
    }
  }
}