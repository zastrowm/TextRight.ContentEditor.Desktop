using System;
using System.Collections.Generic;
using System.Linq;

namespace TextRight.Core.ObjectModel.Blocks.Text
{
  /// <summary>
  ///  A position within a <see cref="StyledTextFragment"/> where text can be inserted.
  /// </summary>
  public struct TextCaret : IEquatable<TextCaret>
  {
    /// <summary> A cursor which represents an invalid location. </summary>
    public static readonly TextCaret Invalid
      = default(TextCaret);

    public TextCaret(StyledTextFragment fragment, int offsetIntoSpan)
    {
      if (offsetIntoSpan < 0)
        throw new ArgumentException("Offset must be zero or a positive number",nameof(offsetIntoSpan));
      if (offsetIntoSpan > fragment.Length)
        throw new ArgumentException(
          $"Offset must be <= fragment.Length ({fragment.Length}) but was ({offsetIntoSpan})",
          nameof(offsetIntoSpan));

      if (offsetIntoSpan == 0 && fragment.Previous != null)
      {
        Fragment = fragment.Previous;
        OffsetIntoSpan = fragment.Previous.Length;
      }
      else
      {
        Fragment = fragment;
        OffsetIntoSpan = offsetIntoSpan;
      }
    }

    // ReSharper disable once UnusedParameter.Local
    internal TextCaret(StyledTextFragment fragment, int offsetIntoSpan, object unused)
    {
      // constructor used solely to avoid the checks in the other constructors when we can guarantee
      // those are true. 

      Fragment = fragment;
      OffsetIntoSpan = offsetIntoSpan;
    }

    /// <summary>
    ///  True if the cursor represents a location in a fragment, false if there is no fragment
    ///  associated with the cursor.
    /// </summary>
    public bool IsValid
      => Fragment != null;

    /// <summary>
    ///  The span that the cursor is currently pointing towards.
    /// </summary>
    public StyledTextFragment Fragment { get; }

    /// <summary> The block that this cursor is associated with. </summary>
    public Block Block
      => Fragment.Parent;

    /// <summary>
    ///  The offset into <see cref="Fragment"/> where this cursor is pointing.
    /// </summary>
    public int OffsetIntoSpan { get; }

    /// <inheritdoc />
    public bool IsAtBeginningOfBlock
      => OffsetIntoSpan == 0;

    /// <inheritdoc />
    public bool IsAtEndOfBlock
      => IsAtEndOfFragment && Fragment.Next == null;

    /// <summary> True if the cursor is pointing at the beginning of the current fragment. </summary>
    public bool IsAtBeginningOfFragment
      => OffsetIntoSpan == 0 || (Fragment.Previous != null && OffsetIntoSpan == 1);

    /// <summary> True if the cursor is pointing at the end of the current fragment. </summary>
    public bool IsAtEndOfFragment
      => OffsetIntoSpan >= Fragment.Length;

    /// <summary> Get the character after the current cursor position. </summary>
    public TextUnit CharacterAfter
    {
      get
      {
        if (!IsAtEndOfFragment)
          return Fragment.GetCharacterAt(OffsetIntoSpan);
        if (Fragment.Next == null)
          return TextUnit.Default;
        return Fragment.Next.GetCharacterAt(0);
      }
    }

    /// <inheritdoc />
    public TextCaret GetNextPosition()
    {
      // we move right to end of the span
      if (OffsetIntoSpan < Fragment.Length)
      {
        return new TextCaret(Fragment, OffsetIntoSpan + 1, null);
      }

      // we're at the end of the span and as long as we can move to the next span,
      // do so. 
      if (Fragment.Next != null)
      {
        // we're never at offset=0 unless we're at the beginning of the first span.
        return new TextCaret(Fragment.Next, 1, null);
      }

      return Invalid;
    }

    /// <inheritdoc />
    public TextCaret GetPreviousPosition()
    {
      // we're at the beginning of the first span
      if (OffsetIntoSpan == 0)
        return Invalid;

      if (OffsetIntoSpan > 2)
      {
        return new TextCaret(Fragment, OffsetIntoSpan - 1, null);
      }

      if (OffsetIntoSpan != 1 || Fragment.Index == 0)
      {
        // at offset 1 of the first span, so go to offset 0 which indicates the
        // beginning of the block. 
        return new TextCaret(Fragment, OffsetIntoSpan - 1, null);
      }

      // we're at the beginning of the current span, so go ahead and move onto
      // previous span. 
      return new TextCaret(Fragment.Previous, Fragment.Length, null);
    }

    /// <inheritdoc />
    public bool Equals(TextCaret other) 
      => Equals(Fragment, other.Fragment) && OffsetIntoSpan == other.OffsetIntoSpan;

    /// <inheritdoc />
    public override bool Equals(object obj)
    {
      if (ReferenceEquals(null, obj))
        return false;

      return obj is TextCaret && Equals((TextCaret)obj);
    }

    /// <inheritdoc />
    public override int GetHashCode()
    {
      unchecked
      {
        return ((Fragment != null ? Fragment.GetHashCode() : 0) * 397) ^ OffsetIntoSpan;
      }
    }

    /// <inheritdoc />
    public static bool operator ==(TextCaret left, TextCaret right) 
      => left.Equals(right);

    /// <inheritdoc />
    public static bool operator !=(TextCaret left, TextCaret right) 
      => !left.Equals(right);
  }
}