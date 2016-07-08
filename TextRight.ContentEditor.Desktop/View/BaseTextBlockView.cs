using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;
using TextRight.ContentEditor.Core.ObjectModel;
using TextRight.ContentEditor.Core.ObjectModel.Blocks;
using TextRight.ContentEditor.Core.Utilities;

namespace TextRight.ContentEditor.Desktop.View
{
  /// <summary> Shared view representation for subclasses of <see cref="TextBlock"/> </summary>
  public abstract class BaseTextBlockView : FrameworkElement,
                                            ITextBlockView
  {
    private readonly DocumentEditorContextView _root;
    private readonly List<StyledStyledTextSpanView> _spans;

    private FormattedText _formattedText;
    private Size _lastMeasureSize;

    private ChangeIndex _cachedIndex;
    private Point _cachedOffset;
    private MeasuredRectangle[] _cachedSizes;
    private Typeface _textFont;
    private int _textFontSize;

    /// <summary> Constructor. </summary>
    /// <param name="root"> The root view that this TextBox is ultimately a part of. </param>
    /// <param name="block"> The block that this view is for. </param>
    protected internal BaseTextBlockView(DocumentEditorContextView root, TextBlock block)
    {
      _root = root;
      _spans = new List<StyledStyledTextSpanView>();
      _textFont = new Typeface("Times New Roman");
      _textFontSize = 16;

      foreach (var span in block)
      {
        _spans.Add(new StyledStyledTextSpanView(this, span));
      }

      RecreateText();
    }

    protected Typeface TextFont
    {
      get { return _textFont; }
      set
      {
        _textFont = value;
        RecreateText();
      }
    }

    protected int TextFontSize
    {
      get { return _textFontSize; }
      set
      {
        _textFontSize = value;
        RecreateText();
      }
    }

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
      Revalidate();

      int offset = fragment.CharacterOffsetIntoTextView + characterIndex;
      var geometry = _formattedText.BuildHighlightGeometry(default(Point), offset, 1);
      Debug.Assert(geometry != null);

      var absoluteOffset = _cachedOffset;

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
      Revalidate();

      return new MeasuredRectangle()
             {
               X = _cachedOffset.X,
               Y = _cachedOffset.Y,
               Width = ActualWidth,
               Height = ActualHeight
             };
    }

    /// <summary>
    ///  Check if the root view has changed and if so, re-evaluate any cached data that would now be
    ///  invalid.
    /// </summary>
    private void Revalidate()
    {
      if (!_root.LayoutChangeIndex.HasChanged(ref _cachedIndex))
        return;

      // TODO see if there is a faster way of doing this
      _cachedOffset = TransformToAncestor(_root).Transform(new Point(0, 0));
      _cachedSizes = null;
    }

    /// <summary> Recreates the FormmatedText due to a text-change event. </summary>
    private void RecreateText()
    {
      _root.MarkChanged();

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
                                         TextFont,
                                         _textFontSize,
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

    /// <summary> Synchronizes the properties of FormattedText with the last measure size. </summary>
    private void UpdateFormattedTextWithConstraints()
    {
      // ReSharper disable once CompareOfFloatsByEqualityOperator
      var newMaxWidth = _lastMeasureSize.Width == double.PositiveInfinity
        ? 0
        : _lastMeasureSize.Width;

      // ReSharper disable once CompareOfFloatsByEqualityOperator
      // ReSharper disable once RedundantCheckBeforeAssignment
      if (newMaxWidth == _formattedText.MaxTextWidth)
        return;

      _root.MarkChanged();
      _formattedText.MaxTextWidth = newMaxWidth;
    }

    public abstract IDocumentItem DocumentItem { get; }
  }
}