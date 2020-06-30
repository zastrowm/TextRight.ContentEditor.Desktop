using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.TextFormatting;
using TextRight.Core.ObjectModel.Blocks.Text;
using TextRight.Core.ObjectModel.Blocks.Text.View;
using TextRight.Core.Utilities;

namespace TextRight.Editor.Wpf.View
{
  /// <summary>
  ///  Implementation of <see cref="IVisualLine{TCaret}<TextCaret>"/> for <see cref="CustomStringRenderer"/>.
  /// </summary>
  internal class TextLineRender : IVisualLine<TextCaret>
  {
    private readonly CustomStringRenderer _owner;
    private readonly int _index;

    public TextLineRender(CustomStringRenderer owner, int index)
    {
      _owner = owner;
      _index = index;
    }

    /// <inheritdoc />
    IVisualLine<TextCaret> IVisualLine<TextCaret>.Next
      => Next;

    /// <inheritdoc />
    IVisualLine<TextCaret> IVisualLine<TextCaret>.Previous 
      => Previous;

    /// <summary />
    public TextLineRender Next
    {
      get
      {
        if (_index < _owner.CachedLines.Count - 1)
          return new TextLineRender(_owner, _index + 1);

        return null;
      }
    }

    /// <summary />
    public TextLineRender Previous
    {
      get
      {
        if (_index > 0)
          return new TextLineRender(_owner, _index - 1);

        return null;
      }
    }

    /// <summary />
    internal TextLineContainer GetContainer()
      => _owner.CachedLines[_index];

    /// <inheritdoc />
    public double Height
      => GetContainer().Line.Height;

    /// <inheritdoc />
    public MeasuredRectangle GetBounds()
    {
      var container = GetContainer();
      var offset = _owner.Offset;

      return new MeasuredRectangle()
             {
               X = container.Point.X + offset.X,
               Y = container.Point.Y + offset.Y,
               Height = container.Line.Height,
               Width = container.Line.Width
             };
    }

    private TextBounds[] _cachedLineBounds;

    internal void DebugDraw(DrawingContext drawingContext)
    {
      var container = GetContainer();

      var bounds = GetCachedLineBounds(container);
      for (var index = 0; index < bounds.Length; index++)
      {
        var rect = bounds[index].Rectangle;
        rect.Y += container.Point.Y;
        drawingContext.DrawRectangle(null, DebugColors.CharacterBorders, rect);
      }
    }

    public MeasuredRectangle GetMeasurement(TextCaret caret)
    {
      // TODO is this the right offset to use with fragments?
      var container = GetContainer();

      var lineBounds = GetCachedLineBounds(container);
      int indexIntoLineBounds = caret.Offset.GraphemeOffset - container.Offset.GraphemeOffset;

      Console.WriteLine($"Index: {indexIntoLineBounds}");

      var bounds = lineBounds[indexIntoLineBounds];
      return new MeasuredRectangle()
             {
               X = bounds.Rectangle.X,
               Y = bounds.Rectangle.Y + container.Point.Y,
               Width = bounds.Rectangle.Width,
               Height = bounds.Rectangle.Height
             };
    }

    private TextBounds[] GetCachedLineBounds(TextLineContainer container)
    {
      if (_cachedLineBounds == null)
      {
        var iterator = TextCaret.FromOffset(container.Content, container.Offset.GraphemeOffset);

        var sizes = new List<TextBounds>();

        var desiredCharOffset = container.Offset.CharOffset + container.Line.Length;
        while (iterator.IsValid && iterator.Offset.CharOffset != desiredCharOffset)
        {
          sizes.Add(container.Line.GetTextBounds(iterator.Offset.CharOffset, 1)[0]);

          int lastOffset = iterator.Offset.CharOffset;
          iterator = iterator.GetNextPosition();

          if (lastOffset == iterator.Offset.CharOffset)
          {
            // we're stuck, or we're empty
            break;
          }
        }

        _cachedLineBounds = sizes.ToArray();
      }


      return _cachedLineBounds;
    }

    TextCaret IVisualLine<TextCaret>.FindClosestTo(double xPosition)
    {
      var container = GetContainer();
      var lineBounds = GetCachedLineBounds(container);

      double DistanceTo(TextCaret caretToMeasure)
        => Math.Abs(xPosition - caretToMeasure.Measure().Left);

      var caret = TextCaret.FromOffset(container.Content, container.Offset.GraphemeOffset);

      var closest =
        (caret: caret,
          index: 0,
          distance: DistanceTo(caret)
          );

      for (int i = 1; i < lineBounds.Length; i++)
      {
        caret = caret.GetNextPosition();
        var diff = DistanceTo(caret);

        if (diff <= closest.distance)
        {
          closest = (caret, i, diff);
        }
        else
        {
          // we're getting bigger
          break;
        }
      }

      return closest.caret;
    }
  }
}