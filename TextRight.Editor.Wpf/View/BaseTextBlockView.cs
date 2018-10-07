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
  public abstract class BaseTextBlockView : BaseBlockView,
                                            IOffsetBasedItem,
                                            ITextBlockContentView,
                                            ITextBlockContentEventListener,
                                            TextBlock.ITextBlockListener
  {
    private readonly DocumentEditorContextView _root;
    private readonly TextBlock _block;
    private readonly CustomStringRenderer _renderer;

    private ChangeIndex _parentIndex;

    /// <summary> Constructor. </summary>
    /// <param name="root"> The root view that this TextBox is ultimately a part of. </param>
    /// <param name="block"> The block that this view is for. </param>
    protected internal BaseTextBlockView(DocumentEditorContextView root, TextBlock block)
    {
      _root = root;
      _block = block;
      _renderer = new CustomStringRenderer(this, block);

      _block.SubscribeListener(this);

      TextBlockChanged(null, _block.Content);

      RecreateText();
    }

    /// <inheritdoc />
    public Point Offset
      => GetOffset();

    /// <summary> The document item for the view. </summary>
    public abstract IDocumentItem DocumentItem { get; }

    public int FontSize
    {
      get => _renderer.FontSize;
      set
      {
        _renderer.FontSize = value;
        RecreateText();
      }
    }

    /// <inheritdoc/>
    protected override Size MeasureOverride(Size constraint)
    {
      if (_renderer.SetMaxWidth(constraint.Width))
      {
        _root.MarkChanged();
      }

      return new Size(_renderer.MaxWidth, _renderer.GetHeight());
    }

    /// <summary>
    ///  Calculates the offset from the upper left of this block to the upper left of the root.
    /// </summary>
    private Point GetOffset()
    {
      FrameworkElement instance = this;

      var offset = new Point();

      while (instance != null && instance != _root.RootVisual)
      {
        var next = VisualTreeHelper.GetOffset(instance);
        offset.X += next.X;
        offset.Y += next.Y;

        instance = instance.Parent as FrameworkElement;
      }

      return offset;
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
    /// <returns> The size of the character. </returns>
    public MeasuredRectangle MeasureCharacter(TextCaret caret)
    {
      Revalidate();

      if (!IsValidForMeasuring || !IsArrangeValid)
        return MeasuredRectangle.Invalid;

      var rect = _renderer.MeasureCharacter(caret);
      if (!rect.IsValid)
        return rect;

      var offset = GetOffset();

      rect.X += offset.X;
      rect.Y += offset.Y;

      return rect;
    }

    /// <inheritdoc />
    public MeasuredRectangle MeasureBounds()
    {
      Revalidate();

      if (!IsValidForMeasuring)
        return MeasuredRectangle.Invalid;

      var offset = GetOffset();

      return new MeasuredRectangle()
             {
               X = offset.X,
               Y = offset.Y,
               Width = ActualWidth,
               Height = ActualHeight
             };
    }

    public MeasuredRectangle Measure(TextCaret caret)
      => MeasureCharacter(caret);

    /// <inheritdoc />
    public void NotifyFragmentInserted(TextSpan previousSibling,
                                       TextSpan newSpan,
                                       TextSpan nextSibling)
    {
      RecreateText();
    }

    public TextCaret GetCursor(DocumentPoint point)
    {
      var offset = GetOffset();

      point.X -= offset.X;
      point.Y -= offset.Y;

      return _renderer.GetCaret(point);
    }

    /// <summary>
    ///  Check if the root view has changed and if so, re-evaluate any cached data that would now be
    ///  invalid.
    /// </summary>
    private void Revalidate()
    {
      // n/a at moment

      if (!_root.LayoutChangeIndex.HasChanged(ref _parentIndex))
        return;
    }

    /// <summary> Recreates the FormmatedText due to a text-change event. </summary>
    private void RecreateText()
    {
      MarkLayoutChanged();

      _root.MarkChanged();
      _renderer.Invalidate();

      // TODO do we have to do both 
      InvalidateMeasure();
      InvalidateVisual();
    }

    private CustomStringRenderer RevalidateAndGetRenderer()
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

    void ITextBlockContentEventListener.NotifyFragmentRemoved(TextSpan previousSibling,
                                                              TextSpan removedSpan,
                                                              TextSpan nextSibling)
    {
      RecreateText();
    }

    void ITextBlockContentEventListener.NotifyTextChanged(TextSpan changedSpan)
    {
      RecreateText();
    }

    public void TextBlockChanged(TextBlockContent oldContent, TextBlockContent newContent)
    {
      if (oldContent?.Target == this)
      {
        oldContent.Target = null;
      }

      newContent.Target = this;
    }
  }
}