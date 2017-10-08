using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Media.TextFormatting;
using TextRight.Core.ObjectModel.Blocks.Text;
using TextRight.Core.ObjectModel.Blocks.Text.View;
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
                             TextOffset offset,
                             StyledTextFragment fragment,
                             int fragmentOffset)
    {
      Line = line;
      CharacterStartIndex = characterStartIndex;
      Fragment = fragment;
      FragmentOffset = fragmentOffset;
      Point = point;

      Debug.Assert(characterStartIndex == offset.CharOffset);
      Offset = offset;

      // TODO GRAPHEME
      NumberOfCaretPositions = Line.Length;
    }

    public TextLine Line { get; }
    public int CharacterStartIndex { get; }
    public TextOffset Offset { get; }

    public StyledTextFragment Fragment { get; }
    public int FragmentOffset { get; }
    public Point Point { get; }
    public int NumberOfCaretPositions { get; }
  }
}