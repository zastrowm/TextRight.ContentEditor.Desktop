using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
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

    private ChangeIndex _cachedIndex;
    private Point _cachedOffset;

    /// <summary> Constructor. </summary>
    /// <param name="root"> The root view that this TextBox is ultimately a part of. </param>
    /// <param name="block"> The block that this view is for. </param>
    protected internal BaseTextBlockView(DocumentEditorContextView root, TextBlock block)
    {
      _root = root;
      _spans = new List<StyledStyledTextSpanView>();
      Text = new FormattedTextHelper(_spans)
             {
               TextFont = new Typeface("Times New Roman"),
               TextFontSize = 16
             };

      foreach (var span in block)
      {
        _spans.Add(new StyledStyledTextSpanView(this, span));
      }

      RecreateText();
    }

    /// <summary> The formatted text for this view.. </summary>
    protected FormattedTextHelper Text { get; }

    /// <summary> The document item for the view. </summary>
    public abstract IDocumentItem DocumentItem { get; }

    /// <inheritdoc/>
    protected override Size MeasureOverride(Size constraint)
    {
      var formattedText = Text.GetFormattedText();

      if (Text.SetSizeConstraint(constraint.Width))
      {
        _root.MarkChanged();
      }

      return new Size(formattedText.Width, formattedText.Height);
    }

    /// <inheritdoc />
    protected override void OnRender(DrawingContext drawingContext)
    {
      base.OnRender(drawingContext);

      var formattedText = Text.GetFormattedText();
      drawingContext.DrawText(formattedText, new Point(0, 0));
    }

    /// <summary>
    ///  True if <see cref="MeasureCharacter"/> and <see cref="MeasureBounds"/> will return valid data,
    ///  false otherwise.
    /// </summary>
    public bool IsValidForMeasuring
      => IsArrangeValid;

    /// <summary> Measures the character at the given index for the given fragment. </summary>
    /// <param name="fragment"> The fragment that owns the character. </param>
    /// <param name="characterIndex"> The index of the character to measure. </param>
    /// <returns> The size of the character. </returns>
    public MeasuredRectangle MeasureCharacter(StyledStyledTextSpanView fragment, int characterIndex)
    {
      Revalidate();

      if (!IsArrangeValid)
        return MeasuredRectangle.Invalid;

      var formattedText = Text.GetFormattedText();

      int offset = fragment.CharacterOffsetIntoTextView + characterIndex;
      var geometry = formattedText.BuildHighlightGeometry(default(Point), offset, 1);
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

    /// <inheritdoc />
    public MeasuredRectangle MeasureBounds()
    {
      Revalidate();

      if (!IsArrangeValid)
        return MeasuredRectangle.Invalid;

      return new MeasuredRectangle()
             {
               X = _cachedOffset.X,
               Y = _cachedOffset.Y,
               Width = ActualWidth,
               Height = ActualHeight
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

    /// <summary>
    ///  Check if the root view has changed and if so, re-evaluate any cached data that would now be
    ///  invalid.
    /// </summary>
    private void Revalidate()
    {
      if (!_root.LayoutChangeIndex.HasChanged(ref _cachedIndex))
        return;

      // TODO see if there is a faster way of doing this
      // TODO do we have to mark changed in RecreateText()?
      _cachedOffset = TransformToAncestor(_root).Transform(new Point(0, 0));
    }

    /// <summary> Recreates the FormmatedText due to a text-change event. </summary>
    private void RecreateText()
    {
      _root.MarkChanged();
      Text.Invalidate();

      // TODO do we have to do all 3? 
      InvalidateMeasure();
      InvalidateArrange();
      InvalidateVisual();
    }
  }
}