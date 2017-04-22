using System;
using System.Collections.Generic;
using System.Linq;

namespace TextRight.Core.ObjectModel.Blocks.Text
{
  /// <summary> A cursor looking in a textblock. </summary>
  public struct TextBlockValueCursor
  {
    /// <summary> A cursor which represents an invalid location. </summary>
    public static readonly TextBlockValueCursor Invalid
      = default(TextBlockValueCursor);

    public TextBlockValueCursor(StyledTextFragment fragment, int offsetIntoSpan)
    {
      Fragment = fragment;

      // TODO verify the offset is valid
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

    /// <summary>
    ///  The offset into <see cref="Fragment"/> where this cursor is pointing.
    /// </summary>
    public int OffsetIntoSpan { get; }

    /// <inheritdoc />
    public bool IsAtBeginning 
      => OffsetIntoSpan == 0;

    /// <inheritdoc />
    public bool IsAtEnd 
      => OffsetIntoSpan >= Fragment.Length && Fragment.Next == null;

    /// <inheritdoc />
    public TextBlockValueCursor MoveForward()
    {
      // we move right to end of the span
      if (OffsetIntoSpan < Fragment.Length)
      {
        return new TextBlockValueCursor(Fragment, OffsetIntoSpan + 1);
      }

      // we're at the end of the span and as long as we can move to the next span,
      // do so. 
      if (Fragment.Next != null)
      {
        // we're never at offset=0 unless we're at the beginning of the first span.
        return new TextBlockValueCursor(Fragment.Next, 1);
      }

      return TextBlockValueCursor.Invalid;
    }

    /// <inheritdoc />
    public TextBlockValueCursor MoveBackward()
    {
      // we're at the beginning of the first span
      if (OffsetIntoSpan == 0)
        return TextBlockValueCursor.Invalid;

      if (OffsetIntoSpan > 2)
      {
        return new TextBlockValueCursor(Fragment, OffsetIntoSpan - 1);
      }

      if (OffsetIntoSpan != 1 || Fragment.Index == 0)
      {
        // at offset 1 of the first span, so go to offset 0 which indicates the
        // beginning of the block. 
        return new TextBlockValueCursor(Fragment, OffsetIntoSpan - 1);
      }

      // we're at the beginning of the current span, so go ahead and move onto
      // previous span. 
      return new TextBlockValueCursor(Fragment.Previous, Fragment.Length);
    }
  }
}