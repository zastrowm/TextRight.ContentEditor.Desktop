using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace TextRight.Core.ObjectModel.Blocks.Text
{
  [DebuggerDisplay("Char={CharOffset}, Grapheme={GraphemeOffset}, Length={GraphemeLength}")]
  public struct TextOffset : IEquatable<TextOffset>
  {
    public TextOffset(int charOffset, int graphemeOffset, int graphemeLength)
    {
      CharOffset = charOffset;
      GraphemeOffset = graphemeOffset;
      GraphemeLength = graphemeLength;
    }

    public int CharOffset { get; }
    public int GraphemeOffset { get; }
    public int GraphemeLength { get; }

    public bool HasContent
      => GraphemeLength > 0;

    public bool Equals(TextOffset other) => CharOffset == other.CharOffset && GraphemeOffset == other.GraphemeOffset && GraphemeLength == other.GraphemeLength;

    public override bool Equals(object obj)
    {
      if (ReferenceEquals(null, obj))
        return false;

      return obj is TextOffset && Equals((TextOffset)obj);
    }

    public override int GetHashCode()
    {
      unchecked
      {
        var hashCode = CharOffset;
        hashCode = (hashCode * 397) ^ GraphemeOffset;
        hashCode = (hashCode * 397) ^ GraphemeLength;
        return hashCode;
      }
    }

    public static bool operator ==(TextOffset left, TextOffset right) => left.Equals(right);

    public static bool operator !=(TextOffset left, TextOffset right) => !left.Equals(right);
  }
}