﻿using System;
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
    protected internal static readonly Thickness BoxedEmptyThickness 
      = new Thickness(0);

    private readonly DocumentEditorContextView _root;
    private readonly TextBlock _block;
    private readonly CustomStringRenderer _renderer;
    private ChangeIndex _parentIndex;

    /// <summary> Static constructor. </summary>
    static BaseTextBlockView()
    {
      // Prevent the base.Margin from ever taking effect in blocks
      MarginProperty.AddOwner(typeof(BaseTextBlockView),
                              new FrameworkPropertyMetadata()
                              {
                                CoerceValueCallback = (instance, value) => BoxedEmptyThickness
                              });
    }

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

    /// <summary> The area around the element that should be free of space. </summary>
    public Thickness Padding { get; set; }

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
      var width = Padding.Left + Padding.Right;
      var height = Padding.Top + Padding.Bottom;

      if (_renderer.SetMaxWidth(constraint.Width - width))
      {
        _root.MarkChanged();
      }

      return new Size(_renderer.MaxWidth + width, _renderer.GetHeight() + height);
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

      _renderer.Render(drawingContext, new Point(Padding.Left, Padding.Top));
    }

    /// <summary>
    ///  True if <see cref="MeasureCharacter"/> and <see cref="MeasureSelectionBounds"/> will return valid data,
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

      rect.X += offset.X + Padding.Left;
      rect.Y += offset.Y + Padding.Top;

      return rect;
    }

    /// <inheritdoc />
    public MeasuredRectangle MeasureSelectionBounds()
    {
      Revalidate();

      if (!IsValidForMeasuring)
        return MeasuredRectangle.Invalid;

      var offset = GetOffset();

      return new MeasuredRectangle()
             {
               X = offset.X + Padding.Left,
               Y = offset.Y + Padding.Top,
               Width = ActualWidth - Padding.Left - Padding.Right,
               Height = ActualHeight - Padding.Top - Padding.Bottom
             };
    }

    public MeasuredRectangle Measure(TextCaret caret)
      => MeasureCharacter(caret);

    /// <summary> Gets a caret that represents the given position in the Text. </summary>
    public TextCaret GetCursor(DocumentPoint point)
    {
      var offset = GetOffset();

      point.X -= offset.X + Padding.Left;
      point.Y -= offset.Y + Padding.Top;

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
    public IVisualLine<TextCaret> FirstTextLine
      => RevalidateAndGetRenderer().FirstTextLine;

    /// <inheritdoc />
    public IVisualLine<TextCaret> LastTextLine
      => RevalidateAndGetRenderer().LastTextLine;

    /// <inheritdoc />
    public IVisualLine<TextCaret> GetLineFor(TextCaret caret)
      => RevalidateAndGetRenderer().GetLineFor(caret);

    /// <inheritdoc />
    void ITextBlockContentEventListener.NotifyTextChanged(TextBlockContent changedContent)
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