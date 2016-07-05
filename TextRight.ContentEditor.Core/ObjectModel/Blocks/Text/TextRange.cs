using System;
using System.Collections.Generic;
using System.Linq;

namespace TextRight.ContentEditor.Core.ObjectModel.Blocks
{
  /// <summary> Represents a start index and end index of a specific string. </summary>
  public struct TextRange
  {
    /// <summary> Constructor. </summary>
    /// <param name="startIndex"> The start index within a string. </param>
    /// <param name="endIndex"> The end index within a string. </param>
    public TextRange(int startIndex, int endIndex)
    {
      StartIndex = startIndex;
      EndIndex = endIndex;
    }

    /// <summary> The start index within a string </summary>
    public int StartIndex { get; }

    /// <summary> The end index within a string. </summary>
    public int EndIndex { get; }

    /// <summary> The number of characters between start index and end index. </summary>
    public int Length
      => EndIndex - StartIndex;

    /// <summary> True if the text range has a length of zero </summary>
    public bool IsEmpty
      => StartIndex == EndIndex;

    /// <summary>
    ///  Checks if the given range includes the given index, not including the start and end index
    ///  themselves.
    /// </summary>
    public bool ContainsExclusive(int index)
    {
      return index > StartIndex
             && index < EndIndex;
    }

    /// <summary>
    ///  Checks if the given range includes the given index, including the start and end index
    ///  themselves.
    /// </summary>
    public bool ContainsInclusive(int index)
    {
      return StartIndex <= index
             && index <= EndIndex;
    }

    public bool OverlapsInclusive(TextRange range)
    {
      // http://stackoverflow.com/a/12888920/548304
      int x1 = StartIndex;
      int x2 = EndIndex;
      int y1 = range.StartIndex;
      int y2 = range.EndIndex;

      return Math.Max(x1, y1) <= Math.Min(x2, y2);
    }

    /// <inheritdoc/>
    public override string ToString()
    {
      return $"{StartIndex}:{EndIndex}";
    }
  }
}