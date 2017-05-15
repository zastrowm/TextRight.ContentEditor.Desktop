using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using TextRight.Core.Cursors;

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

    private TextCaret(StyledTextFragment fragment, TextOffset offset)
    {
      Fragment = fragment;
      Offset = offset;
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

    /// <summary> The offset into <see cref="Fragment"/> that this cursor is pointing. </summary>
    public TextOffset Offset { get; }

    /// <summary> True if the cursor is pointing at the beginning of the current fragment. </summary>
    public bool IsAtBeginningOfFragment
      => Offset.GraphemeOffset == 0;

    /// <summary> True if the cursor is pointing at the end of the current fragment. </summary>
    public bool IsAtEndOfFragment
    {
      get
      {
        if (Fragment.Next == null)
          return Offset.GraphemeOffset >= Fragment.GraphemeLength;
        else
          return Offset.GraphemeOffset >= Fragment.GraphemeLength - 1;
      }
    }

    /// <inheritdoc />
    public bool IsAtBeginningOfBlock
      => Fragment.Previous == null && IsAtBeginningOfFragment;

    /// <inheritdoc />
    public bool IsAtEndOfBlock
      => Fragment.Next == null && IsAtEndOfFragment;

    /// <summary> Get the character after the current cursor position. </summary>
    public TextUnit CharacterAfter
    {
      get
      {
        if (!IsAtEndOfBlock)
          return Fragment.Buffer.GetCharacterAt(Offset.GraphemeOffset);

        return TextUnit.Default;
      }
    }

    /// <inheritdoc />
    public TextCaret GetNextPosition()
    {
      var maybeNextOffset = Fragment.Buffer.GetNextOffset(Offset);
      if (maybeNextOffset != null)
      {
        var nextOffset = maybeNextOffset.GetValueOrDefault();
        return new TextCaret(Fragment, nextOffset);
      }

      var nextFragment = Fragment.Next;

      // we're at the end of the span and as long as we can move to the next span,
      // do so. 
      if (nextFragment != null)
      {
        return new TextCaret(nextFragment, nextFragment.Buffer.GetFirstOffset());
      }

      if (Fragment.Buffer.GetLastOffset() == Offset)
        return new TextCaret(Fragment, TextOffsetHelpers.CreateAfterTextOffset(Fragment.Buffer));

      return Invalid;
    }

    /// <inheritdoc />
    public TextCaret GetPreviousPosition()
    {
      // we're at the beginning of the first span
      if (IsAtBeginningOfBlock)
        return Invalid;

      if (Offset.GraphemeOffset > 0)
      {
        return new TextCaret(Fragment, Fragment.Buffer.GetPreviousOffset(Offset).GetValueOrDefault());
      }

      if (Offset.GraphemeOffset == 0 && Fragment.Previous != null)
      {
        // we're at the beginning of the current span, so go ahead and move into
        // previous span. 
        return new TextCaret(Fragment.Previous, Fragment.Previous.Buffer.GetLastOffset());
      }

      Debug.Fail("How did we get here?");
      return Invalid;
    }

    /// <inheritdoc />
    public bool Equals(TextCaret other)
      => Equals(Fragment, other.Fragment) && Offset == other.Offset;

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
        return ((Fragment != null ? Fragment.GetHashCode() : 0) * 397) ^ Offset.GetHashCode();
      }
    }

    /// <summary> Converts the current instance into a non-specific block caret. </summary>
    public BlockCaret ToBlockCaret()
    {
      if (!IsValid)
        return BlockCaret.Invalid;

      return new BlockCaret(TextCaretMover.Instance, Offset.CharOffset, Offset.GraphemeOffset, Offset.GraphemeLength);
    }

    /// <summary>
    ///  Converts the given caret into a TextCaret, assuming that the BlockCaret is pointing at text
    ///  content.
    /// </summary>
    public static TextCaret FromBlockCaret(BlockCaret caret)
    {
      if (caret.Mover != TextCaretMover.Instance)
        throw new ArgumentException("Caret does not represent the content of a TextCaret", nameof(caret));

      var offset = new TextOffset(caret.InstanceOffset1, caret.InstanceOffset2, caret.InstanceOffset3);
      return new TextCaret((StyledTextFragment)caret.InstanceDatum, offset);
    }

    /// <summary> Gets a cursor that is looking at the beginning of the content. </summary>
    public static TextCaret FromBeginning(TextBlockContent content)
    {
      return new TextCaret(content.FirstFragment, content.FirstFragment.Buffer.GetFirstOffset());
    }

    /// <summary> Gets a cursor that is looking at the end of the content. </summary>
    public static TextCaret FromEnd(TextBlockContent content)
    {
      return new TextCaret(content.LastFragment,
                           TextOffsetHelpers.CreateAfterTextOffset(content.LastFragment.Buffer));
    }

    /// <summary> Gets a cursor that is looking at the grapheme at the given index. </summary>
    public static TextCaret FromOffset(StyledTextFragment fragment, int graphemeIndex)
    {
      // TODO validate
      var offset = fragment.Buffer.GetOffsetToGraphemeIndex(graphemeIndex);
      return new TextCaret(fragment, offset.GetValueOrDefault());
    }

    /// <inheritdoc />
    public static bool operator ==(TextCaret left, TextCaret right)
      => left.Equals(right);

    /// <inheritdoc />
    public static bool operator !=(TextCaret left, TextCaret right)
      => !left.Equals(right);

    /// <summary> Implicit cast that converts the given TextCaret to a BlockCaret. </summary>
    public static implicit operator BlockCaret(TextCaret caret) 
      => caret.ToBlockCaret();

    /// <summary> Implicit cast that converts the given BlockCaret to a TextCaret. </summary>
    public static implicit operator TextCaret(BlockCaret caret)
      => FromBlockCaret(caret);

    private class TextCaretMover : ICaretMover
    {
      internal static readonly TextCaretMover Instance
        = new TextCaretMover();

      public BlockCaret MoveForward(BlockCaret caret)
        => FromBlockCaret(caret).GetNextPosition().ToBlockCaret();

      public BlockCaret MoveBackward(BlockCaret caret)
        => FromBlockCaret(caret).GetNextPosition().ToBlockCaret();

      public bool IsAtBlockEnd(BlockCaret caret)
        => FromBlockCaret(caret).IsAtEndOfBlock;

      public bool IsAtBlockStart(BlockCaret caret)
        => FromBlockCaret(caret).IsAtBeginningOfBlock;
    }
  }
}