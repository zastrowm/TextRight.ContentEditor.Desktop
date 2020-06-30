using System;
using System.Collections.Generic;
using System.Linq;

namespace TextRight.Core.ObjectModel.Blocks.Text
{
  /// <summary>
  ///   The difference between two <see cref="TextOffset"/>s.
  /// </summary>
  public readonly struct TextOffsetDifference : IEquatable<TextOffsetDifference>
  {
    public TextOffsetDifference(int charSize, int graphemeSize)
    {
      CharSize = charSize;
      GraphemeSize = graphemeSize;
    }

    public int CharSize { get; }

    public int GraphemeSize { get; }

    public bool Equals(TextOffsetDifference other)
      => CharSize == other.CharSize && GraphemeSize == other.GraphemeSize;

    public override bool Equals(object obj)
      => obj is TextOffsetDifference other && Equals(other);

    public override int GetHashCode()
    {
      unchecked
      {
        return (CharSize * 397) ^ GraphemeSize;
      }
    }

    public static bool operator ==(TextOffsetDifference left, TextOffsetDifference right)
      => left.Equals(right);

    public static bool operator !=(TextOffsetDifference left, TextOffsetDifference right)
      => !left.Equals(right);
  }
}