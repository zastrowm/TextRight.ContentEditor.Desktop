using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace TextRight.Core.ObjectModel.Blocks.Text
{
  [DebuggerDisplay("Char={CharOffset}, Grapheme={GraphemeOffset}, Length={GraphemeLength}")]
  public readonly struct TextOffset : IEquatable<TextOffset>
  {
    public TextOffset(int charOffset, int graphemeOffset, int graphemeLength)
    {
      CharOffset = charOffset;
      GraphemeOffset = graphemeOffset;
      GraphemeLength = graphemeLength;
    }

    /// <summary> The offset to a character in a buffer that this offset is pointing to. </summary>
    public int CharOffset { get; }
    
    /// <summary> The offset to a grapheme in a buffer that this offset is pointing to. </summary>
    public int GraphemeOffset { get; }
    
    /// <summary> The size of the grapheme being pointed to by this offset. </summary>
    public int GraphemeLength { get; }

    /// <summary> True if this offset is pointing to a valid grapheme. </summary>
    public bool HasContent
      => GraphemeLength > 0;

    /// <summary />
    public bool Equals(TextOffset other) 
      => CharOffset == other.CharOffset && GraphemeOffset == other.GraphemeOffset && GraphemeLength == other.GraphemeLength;

    /// <inheritdoc />
    public override bool Equals(object obj)
    {
      if (ReferenceEquals(null, obj))
        return false;

      return obj is TextOffset offset && Equals(offset);
    }

    /// <inheritdoc />
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

    /// <summary />
    public static bool operator ==(TextOffset left, TextOffset right) 
      => left.Equals(right);

    /// <summary />
    public static bool operator !=(TextOffset left, TextOffset right) 
      => !left.Equals(right);
  }
}