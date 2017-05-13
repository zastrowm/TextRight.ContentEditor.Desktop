using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media.TextFormatting;
using TextRight.Core.ObjectModel.Blocks.Text;
using TextRight.Core.Utilities;
using TextRight.Editor.View.Blocks;

namespace TextRight.Editor.Wpf.View
{
  /// <summary> Container for holding the cached line and point for rendering. </summary>
  internal struct TextLineContainer
  {
    public TextLineContainer(Point point,
                             TextLine line,
                             int characterStartIndex,
                             StyledTextFragment fragment,
                             int fragmentOffset)
    {
      Line = line;
      CharacterStartIndex = characterStartIndex;
      Fragment = fragment;
      FragmentOffset = fragmentOffset;
      Point = point;

      // TODO GRAPHEME
      NumberOfCaretPositions = Line.Length;
    }

    public TextLine Line { get; }
    public int CharacterStartIndex { get; }
    public StyledTextFragment Fragment { get; }
    public int FragmentOffset { get; }
    public Point Point { get; }
    public int NumberOfCaretPositions { get; }

    private class LineImplementation : ILine
    {
      private readonly LineRenderer _owner;
      private readonly int _index;

      public LineImplementation(LineRenderer owner, int index)
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

      /// <inheritdoc />
      public ILine Next
      {
        get
        {
          if (_index <= _owner.Count - 1)
            return new LineImplementation(_owner, _index + 1);

          return null;
        }
      }

      /// <inheritdoc />
      public ILine Previous
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