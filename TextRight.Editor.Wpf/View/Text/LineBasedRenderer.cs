using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media.TextFormatting;
using TextRight.Core.ObjectModel.Blocks.Text;
using TextRight.Core.ObjectModel.Blocks.Text.View;
using TextRight.Core.Utilities;

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

    public ITextLine FirstTextLine
      => new LineImplementation(this, 0);

    public ITextLine LastTextLine
      => new LineImplementation(this, this.Count - 1);

    public ITextLine GetLineFor(TextCaret caret)
    {
      // TODO
      throw new NotImplementedException();
    }

    private class LineImplementation : ITextLine
    {
      private readonly LineBasedRenderer _owner;
      private readonly int _index;

      public LineImplementation(LineBasedRenderer owner, int index)
      {
        _owner = owner;
        _index = index;
      }

      private TextLineContainer GetContainer()
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

      public MeasuredRectangle GetMeasurement(TextCaret caret)
      {
        // TODO is this the right offset to use with fragments?
        
        // TODO graphemes
        int offset = caret.Offset.CharOffset;

        var container = GetContainer();

        if (_cachedLineBounds == null)
        {
          int startIndex = container.CharacterStartIndex;
          int length = container.Line.Length;

          _cachedLineBounds = new TextBounds[length];

          for (int i = 0; i < length; i++)
          {
            _cachedLineBounds[i] = container.Line.GetTextBounds(startIndex + i, 1)[0];
          }
        }

        var bounds = _cachedLineBounds[offset - container.CharacterStartIndex];
        return new MeasuredRectangle()
               {
                 X = bounds.Rectangle.X,
                 Y = bounds.Rectangle.Y + container.Point.Y,
                 Width = bounds.Rectangle.Width,
                 Height = bounds.Rectangle.Height
               };
      }

      /// <inheritdoc />
      public ITextLine Next
      {
        get
        {
          if (_index <= _owner.Count - 1)
            return new LineImplementation(_owner, _index + 1);

          return null;
        }
      }

      /// <inheritdoc />
      public ITextLine Previous
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
    }
  }
}