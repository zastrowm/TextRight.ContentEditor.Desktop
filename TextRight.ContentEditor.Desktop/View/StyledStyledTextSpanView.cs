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
  public class StyledStyledTextSpanView : Run, IStyledTextSpanView
  {
    private readonly StyledTextFragment _fragment;

    /// <summary> Constructor. </summary>
    /// <param name="fragment"> The span to keep synchronized. </param>
    public StyledStyledTextSpanView(StyledTextFragment fragment)
    {
      _fragment = fragment;
      _fragment.Target = this;
    }

    /// <inheritdoc/>
    public void TextUpdated(StyledTextFragment fragment)
    {
      Text = _fragment.Text;
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