using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;
using TextRight.ContentEditor.Core.ObjectModel.Blocks;
using TextRight.ContentEditor.Core.Utilities;

namespace TextRight.ContentEditor.Desktop.View
{
  /// <summary>
  ///  Associates a WPF Run with a TextSpan and keeps them in sync.
  /// </summary>
  public class StyledStyledTextSpanView : Run,
                                          IStyledTextSpanView
  {
    private readonly TextBlockView _textBlockView;
    private readonly StyledTextFragment _fragment;
    private GeneralTransform _transform;

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

    /// <summary>
    ///  The transform to convert points into the coordinate space for the control that the document
    ///  is hosted for, lazily initialized.
    /// </summary>
    private GeneralTransform LazyTransform
    {
      get
      {
        if (_transform == null)
        {
          _transform = _textBlockView.TransformToAncestor((Visual)_textBlockView.Parent);
        }

        return _transform;
      }
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

      // TODO investigate if there's another/faster way to do this
      var offsetToBlock = LazyTransform.Transform(default(Point));

      return new MeasuredRectangle()
             {
               X = leftRect.X + offsetToBlock.X,
               Y = leftRect.Y + offsetToBlock.Y,
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