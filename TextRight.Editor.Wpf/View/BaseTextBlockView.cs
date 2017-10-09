using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using TextRight.Core;
using TextRight.Core.ObjectModel;
using TextRight.Core.ObjectModel.Blocks.Text;
using TextRight.Core.ObjectModel.Blocks.Text.View;
using TextRight.Core.Utilities;

namespace TextRight.Editor.Wpf.View
{
  /// <summary> Shared view representation for subclasses of <see cref="TextBlock"/> </summary>
  public abstract class BaseTextBlockView : FrameworkElement,
                                            IOffsetBasedItem,
                                            ITextBlockView,
                                            ILineBasedRenderer
  {
    private readonly DocumentEditorContextView _root;
    private readonly TextBlock _block;
    private readonly List<StyledStyledTextSpanView> _spans;
    private readonly CustomStringRenderer _renderer;

    private ChangeIndex _cachedIndex;
    private Point _cachedOffset;

    /// <summary> Constructor. </summary>
    /// <param name="root"> The root view that this TextBox is ultimately a part of. </param>
    /// <param name="block"> The block that this view is for. </param>
    protected internal BaseTextBlockView(DocumentEditorContextView root, TextBlock block)
    {
      _root = root;
      _block = block;
      _spans = new List<StyledStyledTextSpanView>();
      _renderer = new CustomStringRenderer(this, block, _spans);

      foreach (var span in block.Content.Fragments)
      {
        _spans.Add(new StyledStyledTextSpanView(this, span));
      }

      RecreateText();
    }

    // TODO what if this isn't valid yet
    /// <inheritdoc />
    public Point Offset
      => _cachedOffset;

    /// <summary> The document item for the view. </summary>
    public abstract IDocumentItem DocumentItem { get; }

    /// <inheritdoc/>
    protected override Size MeasureOverride(Size constraint)
    {
      if (_renderer.SetMaxWidth(constraint.Width))
      {
        _root.MarkChanged();
      }

      return new Size(_renderer.MaxWidth, _renderer.GetHeight());
    }

    /// <inheritdoc />
    protected override void OnRender(DrawingContext drawingContext)
    {
      base.OnRender(drawingContext);

      if (_block == null)
        return;

      _renderer.Render(drawingContext);
    }

    /// <summary>
    ///  True if <see cref="MeasureCharacter"/> and <see cref="MeasureBounds"/> will return valid data,
    ///  false otherwise.
    /// </summary>
    public bool IsValidForMeasuring
      => IsMeasureValid && _root.IsLayoutValid;

    /// <summary> Measures the character at the given index for the given fragment. </summary>
    /// <param name="fragment"> The fragment that owns the character. </param>
    /// <param name="characterIndex"> The index of the character to measure. </param>
    /// <returns> The size of the character. </returns>
    public MeasuredRectangle MeasureCharacter(StyledStyledTextSpanView fragment, int characterIndex)
    {
      Revalidate();

      if (!IsValidForMeasuring)
        return MeasuredRectangle.Invalid;

      var cursor = TextCaret.FromOffset((StyledTextFragment)fragment.DocumentItem, characterIndex);
      var rect = _renderer.MeasureCharacter(cursor);
      if (!rect.IsValid)
        return rect;

      rect.X += _cachedOffset.X;
      rect.Y += _cachedOffset.Y;

      return rect;
    }

    /// <summary> Measures the character at the given index for the given fragment. </summary>
    /// <returns> The size of the character. </returns>
    public MeasuredRectangle MeasureCharacter(TextCaret caret)
    {
      Revalidate();

      if (!IsValidForMeasuring)
        return MeasuredRectangle.Invalid;

      var rect = _renderer.MeasureCharacter(caret);
      if (!rect.IsValid)
        return rect;

      rect.X += _cachedOffset.X;
      rect.Y += _cachedOffset.Y;

      return rect;
    }

    /// <inheritdoc />
    public MeasuredRectangle MeasureBounds()
    {
      Revalidate();

      if (!IsValidForMeasuring)
        return MeasuredRectangle.Invalid;

      return new MeasuredRectangle()
             {
               X = _cachedOffset.X,
               Y = _cachedOffset.Y,
               Width = ActualWidth,
               Height = ActualHeight
             };
    }

    public MeasuredRectangle Measure(TextCaret caret) 
      => MeasureCharacter(caret);

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

    public TextCaret GetCursor(DocumentPoint point)
    {
      point.X -= _cachedOffset.X;
      point.Y -= _cachedOffset.Y;

      return _renderer.GetCursor(point);
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
      _renderer.Invalidate();

      // TODO do we have to do both 
      InvalidateMeasure();
      InvalidateVisual();
    }

    private ILineBasedRenderer RevalidateAndGetRenderer()
    {
      Revalidate();
      return _renderer;
    }

    /// <inheritdoc />
    public ITextLine FirstTextLine 
      => RevalidateAndGetRenderer().FirstTextLine;

    /// <inheritdoc />
    public ITextLine LastTextLine 
      => RevalidateAndGetRenderer().LastTextLine;

    /// <inheritdoc />
    public ITextLine GetLineFor(TextCaret caret) 
      => RevalidateAndGetRenderer().GetLineFor(caret);
  }
}