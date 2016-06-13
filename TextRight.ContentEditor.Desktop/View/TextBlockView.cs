using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using TextRight.ContentEditor.Core.ObjectModel;
using TextRight.ContentEditor.Core.ObjectModel.Blocks;
using TextRight.ContentEditor.Core.Utilities;

namespace TextRight.ContentEditor.Desktop.View
{
  /// <summary> View representation of a <see cref="Core.ObjectModel.Blocks.TextBlock"/> </summary>
  public class TextBlockView : FrameworkElement,
                               ITextBlockView
  {
    private readonly DocumentEditorContextView _root;
    private readonly TextBlock _block;
    private readonly List<StyledStyledTextSpanView> _spans;

    private FormattedText _formattedText;
    private Size _lastMeasureSize;

    /// <summary> Constructor. </summary>
    /// <param name="root"> The root view that this TextBox is ultimately a part of. </param>
    /// <param name="block"> The block that this view is for. </param>
    public TextBlockView(DocumentEditorContextView root, TextBlock block)
    {
      Margin = new Thickness(10);

      _root = root;
      _block = block;
      _block.Target = this;

      _spans = new List<StyledStyledTextSpanView>();

      foreach (var span in block)
      {
        _spans.Add(new StyledStyledTextSpanView(this, span));
      }

      RecreateText();
    }

    /// <summary> Recreates the FormmatedText due to a text-change event. </summary>
    private void RecreateText()
    {
      int startIndex = 0;
      var builder = new StringBuilder();

      foreach (var span in _spans)
      {
        span.CharacterOffsetIntoTextView = startIndex;
        builder.Append(span.Text);
        startIndex += span.Text.Length;
      }

      _formattedText = new FormattedText(builder.ToString(),
                                         CultureInfo.CurrentCulture,
                                         FlowDirection.LeftToRight,
                                         new Typeface("Times New Roman"),
                                         16,
                                         Brushes.Black,
                                         null,
                                         TextFormattingMode.Display)
                       {
                         MaxTextWidth = _lastMeasureSize.Width
                       };

      InvalidateMeasure();
      InvalidateArrange();
      InvalidateVisual();
    }

    /// <inheritdoc />
    public IDocumentItem DocumentItem
      => _block;

    protected override Size MeasureOverride(Size constraint)
    {
      _lastMeasureSize = constraint;
      UpdateFormattedTextWithConstraints();

      return new Size(_formattedText.Width, _formattedText.Height);
    }

    /// <inheritdoc />
    protected override void OnRender(DrawingContext drawingContext)
    {
      base.OnRender(drawingContext);
      drawingContext.DrawText(_formattedText, new Point(0, 0));
    }

    /// <summary> Measures the character at the given index for the given fragment. </summary>
    /// <param name="fragment"> The fragment that owns the character. </param>
    /// <param name="characterIndex"> The index of the character to measure. </param>
    /// <returns> The size of the character. </returns>
    public MeasuredRectangle MeasureCharacter(StyledStyledTextSpanView fragment, int characterIndex)
    {
      int offset = fragment.CharacterOffsetIntoTextView + characterIndex;
      var geometry = _formattedText.BuildHighlightGeometry(default(Point), offset, 1);
      Debug.Assert(geometry != null);

      // TODO see if there is a faster way of doing this
      var absoluteOffset = TransformToAncestor(_root).Transform(new Point(0, 0));

      return new MeasuredRectangle()
             {
               X = absoluteOffset.X + geometry.Bounds.X,
               Y = absoluteOffset.Y + geometry.Bounds.Y,
               Height = geometry.Bounds.Height,
               Width = geometry.Bounds.Width,
             };
    }

    /// <summary> Removes the given span from the TextBlockView. </summary>
    public void MarkRemoved(StyledStyledTextSpanView toRemove)
    {
      _spans.Remove(toRemove);

      RecreateText();
    }

    /// <summary> Invoked by a child fragment when the fragment's text has changed. </summary>
    /// <param name="fragment"> The fragment that changed. </param>
    public void MarkTextChanged(StyledTextFragment fragment)
    {
      RecreateText();
    }

    /// <inheritdoc />
    public void NotifyBlockInserted(StyledTextFragment previousSibling,
                                    StyledTextFragment newFragment,
                                    StyledTextFragment nextSibling)
    {
      var newView = new StyledStyledTextSpanView(this, newFragment);

      var previousSpanView = previousSibling?.Target as StyledStyledTextSpanView;
      var nextSpanView = nextSibling?.Target as StyledStyledTextSpanView;
      if (previousSpanView != null)
      {
        _spans.Insert(previousSibling.Index + 1, newView);
      }
      else if (nextSpanView != null)
      {
        _spans.Insert(nextSibling.Index, newView);
      }
      else
      {
        _spans.Add(newView);
      }

      RecreateText();
    }

    /// <inheritdoc />
    public MeasuredRectangle MeasureBounds()
    {
      var offset = TransformToAncestor(_root).Transform(new Point(0, 0));

      return new MeasuredRectangle()
             {
               X = offset.X,
               Y = offset.Y,
               Width = ActualWidth,
               Height = ActualHeight
             };
    }

    /// <summary> Synchronizes the properties of FormattedText with the last measure size. </summary>
    private void UpdateFormattedTextWithConstraints()
    {
      // ReSharper disable once CompareOfFloatsByEqualityOperator
      _formattedText.MaxTextWidth = _lastMeasureSize.Width == double.PositiveInfinity
        ? 0
        : _lastMeasureSize.Width;
    }
  }
}