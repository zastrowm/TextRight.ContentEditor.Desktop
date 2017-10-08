using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.TextFormatting;
using TextRight.Core.ObjectModel.Blocks.Text;
using TextRight.Core.ObjectModel.Blocks.Text.View;
using TextRight.Core.Utilities;
using TextRight.Editor.View.Blocks;

namespace TextRight.Editor.Wpf.View
{
  internal class LineBasedRenderer : List<TextLineContainer>, ILineBasedRenderer
  {
    private readonly IOffsetBasedItem _parent;

    public LineBasedRenderer(IOffsetBasedItem parent)
    {
      _parent = parent;
    }

    public Point Offset
      => _parent.Offset;

    ITextLine ILineBasedRenderer.FirstTextLine
      => FirstTextLine;

    public LineImplementation FirstTextLine
      => new LineImplementation(this, 0);

    ITextLine ILineBasedRenderer.LastTextLine
      => LastTextLine;

    public LineImplementation LastTextLine
      => new LineImplementation(this, this.Count - 1);

    public ITextLine GetLineFor(TextCaret caret)
    {
      var line = FirstTextLine;

      // NOTE - we need to pass it index into the larger text string.  Not sure if that's the underlying 
      // string or a some other buffer (The TextRun?, the Paragraph?)
      while (line.Next != null && line.Next.GetContainer().Offset.GraphemeOffset <= caret.Offset.GraphemeOffset)
        line = line.Next;

      return line;
    }

    internal class LineImplementation : ITextLine
    {
      private readonly LineBasedRenderer _owner;
      private readonly int _index;

      public LineImplementation(LineBasedRenderer owner, int index)
      {
        _owner = owner;
        _index = index;
      }


      ITextLine ITextLine.Next
        => Next;

      ITextLine ITextLine.Previous 
        => Previous;

      /// <inheritdoc />
      public LineImplementation Next
      {
        get
        {
          if (_index < _owner.Count - 1)
            return new LineImplementation(_owner, _index + 1);

          return null;
        }
      }

      /// <inheritdoc />
      public LineImplementation Previous
      {
        get
        {
          if (_index > 0)
            return new LineImplementation(_owner, _index - 1);

          return null;
        }
      }

      /// <inheritdoc />
      public ILineIterator GetLineStart()
      {
        throw new NotImplementedException();
      }

      /// <inheritdoc />
      public ILineIterator GetLineEnd()
      {
        throw new NotImplementedException();
      }

      internal TextLineContainer GetContainer()
        => _owner[_index];

      /// <inheritdoc />
      public int NumberOfCaretPositions
        => GetContainer().NumberOfCaretPositions;

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

      private static readonly Pen DebugBorderPen
        = new Pen(new SolidColorBrush(Color.FromArgb(126,0,83,252)),.5);

      internal void DebugDraw(DrawingContext drawingContext)
      {
        var container = GetContainer();

        var bounds = GetCachedLineBounds(container);
        for (var index = 0; index < bounds.Length; index++)
        {
          var rect = bounds[index].Rectangle;
          rect.Y += container.Point.Y;
          drawingContext.DrawRectangle(null, DebugBorderPen, rect);
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
          var iterator = TextCaret.FromOffset(container.Fragment, container.Offset.GraphemeOffset);

          var sizes = new List<TextBounds>();

          var desiredCharOffset = container.Offset.CharOffset + container.Line.Length;
          while (iterator.IsValid && iterator.Offset.CharOffset != desiredCharOffset)
          {
            sizes.Add(container.Line.GetTextBounds(iterator.Offset.CharOffset, 1)[0]);

            iterator = iterator.GetNextPosition();
          }

          _cachedLineBounds = sizes.ToArray();
        }


        return _cachedLineBounds;
      }

      TextCaret ITextLine.FindClosestTo(double xPosition)
      {
        var container = GetContainer();
        var lineBounds = GetCachedLineBounds(container);

        int absoluteOffset = container.CharacterStartIndex;

        // account for line indent
        xPosition -= _owner.Offset.X;

        double DistanceTo(TextCaret caretToMeasure)
          => Math.Abs(xPosition - caretToMeasure.Measure().X);

        // TODO grapheme
        var caret = TextCaret.FromOffset(container.Fragment, absoluteOffset);

        var closest = (
            caret: caret,
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
}