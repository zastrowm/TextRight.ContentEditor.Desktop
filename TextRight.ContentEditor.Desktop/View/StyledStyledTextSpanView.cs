using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows.Documents;
using TextRight.ContentEditor.Core.ObjectModel.Blocks;
using TextRight.ContentEditor.Core.Utilities;

namespace TextRight.ContentEditor.Desktop.View
{
  /// <summary>
  ///  Associates a WPF Run with a TextSpan and keeps them in sync.
  /// </summary>
  public class StyledStyledTextSpanView : Run, IStyledTextSpanView
  {
    private readonly TextBlockView _textBlockView;
    private readonly StyledTextFragment _fragment;

    /// <summary> Constructor. </summary>
    /// <param name="textBlockView"></param>
    /// <param name="fragment"> The span to keep synchronized. </param>
    public StyledStyledTextSpanView(TextBlockView textBlockView, StyledTextFragment fragment)
    {
      _textBlockView = textBlockView;
      _fragment = fragment;
      _fragment.Target = this;

      TextUpdated(_fragment);
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

      // GetPositionAtOffset always returns zero width, so to get the width of the full character, 
      // we need to measure the left side and then measure the right
      var position = ContentStart.GetPositionAtOffset(offset);
      Debug.Assert(position != null);

      var nextPosition = position.GetPositionAtOffset(1);
      Debug.Assert(nextPosition != null);

      var leftRect = position.GetCharacterRect(LogicalDirection.Forward);
      var rightRect = nextPosition.GetCharacterRect(LogicalDirection.Backward);

      return new MeasuredRectangle()
             {
               X = leftRect.X,
               Y = leftRect.Y,
               Width = rightRect.X - leftRect.X,
               Height = leftRect.Height
             };
    }

    public void Detach()
    {
      // TODO we should recycle this view (pool it)?
      _fragment.Target = null;
      _textBlockView.Inlines.Remove(this);
    }
  }
}