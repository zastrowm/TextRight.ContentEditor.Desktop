using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using TextRight.Core.Cursors;
using TextRight.Core.Utilities;

namespace TextRight.Core.ObjectModel.Blocks.Text
{
  /// <summary>
  ///  A position within a <see cref="TextSpan"/> where text can be inserted.
  /// </summary>
  public struct TextCaret : IEquatable<TextCaret>,
                            IBlockCaret
  {
    /// <summary> A cursor which represents an invalid location. </summary>
    public static readonly TextCaret Invalid
      = default(TextCaret);

    public static ICaretMover<TextCaret> Factory
      => TextCaretMover.Instance;

    private TextCaret(TextSpan span, TextOffset offset)
    {
      Span = span;
      Offset = offset;
    }

    /// <summary>
    ///  True if the cursor represents a location in a fragment, false if there is no fragment
    ///  associated with the cursor.
    /// </summary>
    public bool IsValid
      => Span != null;

    /// <summary>
    ///  The span that the cursor is currently pointing towards.
    /// </summary>
    public TextSpan Span { get; }

    /// <summary> The block that this cursor is associated with. </summary>
    public TextBlock Block
      => Span.Parent;

    /// <summary> The offset into <see cref="Span"/> that this cursor is pointing. </summary>
    public TextOffset Offset { get; }

    /// <summary> True if the cursor is pointing at the beginning of the current fragment. </summary>
    public bool IsAtFragmentStart
      => Offset.GraphemeOffset == 0;

    /// <summary> True if the cursor is pointing at the end of the current fragment. </summary>
    public bool IsAtFragmentEnd
    {
      get
      {
        if (Span.Next == null)
          return Offset.GraphemeOffset >= Span.GraphemeLength;
        else
          return Offset.GraphemeOffset >= Span.GraphemeLength - 1;
      }
    }

    /// <inheritdoc />
    public bool IsAtBlockStart
      => Span.Previous == null && IsAtFragmentStart;

    /// <inheritdoc />
    public bool IsAtBlockEnd
      => Span.Next == null && IsAtFragmentEnd;

    /// <summary> Get the character after the current cursor position. </summary>
    public TextUnit CharacterAfter
    {
      get
      {
        if (!IsAtBlockEnd)
          return Span.Buffer.GetCharacterAt(Offset);

        return TextUnit.Default;
      }
    }

    /// <inheritdoc />
    public TextCaret GetNextPosition()
    {
      var maybeNextOffset = Span.Buffer.GetNextOffset(Offset);
      if (maybeNextOffset != null)
      {
        var nextOffset = maybeNextOffset.GetValueOrDefault();
        return new TextCaret(Span, nextOffset);
      }

      var nextFragment = Span.Next;

      // we're at the end of the span and as long as we can move to the next span,
      // do so. 
      if (nextFragment != null)
      {
        return new TextCaret(nextFragment, nextFragment.Buffer.GetFirstOffset());
      }

      if (Span.Buffer.GetLastOffset().GraphemeOffset == Offset.GraphemeOffset)
        return new TextCaret(Span, TextOffsetHelpers.CreateAfterTextOffset(Span.Buffer));

      return Invalid;
    }

    /// <inheritdoc />
    public TextCaret GetPreviousPosition()
    {
      // we're at the beginning of the first span
      if (IsAtBlockStart)
        return Invalid;

      if (Offset.GraphemeOffset > 0)
      {
        return new TextCaret(Span, Span.Buffer.GetPreviousOffset(Offset).GetValueOrDefault());
      }

      if (Offset.GraphemeOffset == 0 && Span.Previous != null)
      {
        // we're at the beginning of the current span, so go ahead and move into
        // previous span. 
        return new TextCaret(Span.Previous, Span.Previous.Buffer.GetLastOffset());
      }

      Debug.Fail("How did we get here?");
      return Invalid;
    }

    /// <inheritdoc />
    public bool Equals(TextCaret other)
      => Equals(Span, other.Span) && Offset == other.Offset;

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
        return ((Span != null ? Span.GetHashCode() : 0) * 397) ^ Offset.GetHashCode();
      }
    }

    /// <summary> Converts the current instance into a non-specific block caret. </summary>
    public BlockCaret ToBlockCaret()
    {
      if (!IsValid)
        return BlockCaret.Invalid;

      return new BlockCaret(TextCaretMover.Instance, Span, Offset.CharOffset, Offset.GraphemeOffset, Offset.GraphemeLength);
    }

    /// <inheritdoc />
    public MeasuredRectangle Measure()
    {
      if (IsAtBlockStart && IsAtBlockEnd)
      {
        // if it's empty, there is no character to measure
        return Block.GetBounds().FlattenLeft();
      }

      // we want to measure the next character unless the previous character was
      // a space (as the text will most likely appear on the next line anyways) 
      bool shouldMeasureNext = IsAtBlockStart
                               || (!IsAtBlockStart && GetPreviousPosition().CharacterAfter.Character == ' ');

      return shouldMeasureNext
        ? MeasureForward().FlattenLeft()
        : MeasureBackward().FlattenRight();
    }

    private MeasuredRectangle MeasureForward()
    {
      if (IsAtBlockEnd || Span.Owner?.Target == null)
        return MeasuredRectangle.Invalid;

      return Span.Owner.Target.Measure(this);
    }

    private MeasuredRectangle MeasureBackward()
    {
      if (IsAtBlockStart || Span.Owner?.Target == null)
        return MeasuredRectangle.Invalid;

      return Span.Owner.Target.Measure(GetPreviousPosition());
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
      return new TextCaret((TextSpan)caret.InstanceDatum, offset);
    }

    /// <summary> Gets a cursor that is looking at the beginning of the content. </summary>
    public static TextCaret FromBeginning(TextBlockContent content)
    {
      return new TextCaret(content.FirstSpan, content.FirstSpan.Buffer.GetFirstOffset());
    }

    /// <summary> Gets a cursor that is looking at the end of the content. </summary>
    public static TextCaret FromEnd(TextBlockContent content)
    {
      return new TextCaret(content.LastSpan,
                           TextOffsetHelpers.CreateAfterTextOffset(content.LastSpan.Buffer));
    }

    /// <summary> Gets a cursor that is looking at the grapheme at the given index. </summary>
    public static TextCaret FromOffset(TextSpan span, int graphemeIndex)
    {
      // TODO validate
      return FromOffset(span, span.Buffer.GetOffsetToGraphemeIndex(graphemeIndex).GetValueOrDefault());
    }

    // <summary> Gets a cursor that is looking at the grapheme at the given index. </summary>
    public static TextCaret FromOffset(TextSpan span, TextOffset offset)
    {
      // TODO validate
      return new TextCaret(span, offset);
    }

    /// <summary> Gets an object that holds the serialized data for this caret. </summary>
    private ISerializedBlockCaret Serialize()
      => new SerializedData(this);

    /// <summary />
    public static bool operator ==(TextCaret left, TextCaret right)
      => left.Equals(right);

    /// <summary />
    public static bool operator !=(TextCaret left, TextCaret right)
      => !left.Equals(right);

    /// <summary> Implicit cast that converts the given TextCaret to a BlockCaret. </summary>
    public static implicit operator BlockCaret(TextCaret caret)
      => caret.ToBlockCaret();

    /// <summary> Explicit cast that converts the given BlockCaret to a TextCaret. </summary>
    public static explicit operator TextCaret(BlockCaret caret)
      => FromBlockCaret(caret);

    private class SerializedData : ISerializedBlockCaret
    {
      private readonly int _spanIndex;
      private readonly int _graphemeOffset;
      private readonly BlockPath _pathToBlock;

      public SerializedData(TextCaret caret)
      {
        _spanIndex = caret.Span.Index;
        _graphemeOffset = caret.Offset.GraphemeOffset;
        _pathToBlock = caret.Block.GetBlockPath();
      }

      public BlockCaret Deserialize(DocumentEditorContext context)
      {
        var block = _pathToBlock.Get(context.Document);
        var fragment = ((TextBlock)block).Content.GetSpanAtIndex(_spanIndex);
        return TextCaret.FromOffset(fragment, _graphemeOffset);
      }
    }

    private class TextCaretMover : ICaretMover<TextCaret>
    {
      internal static readonly TextCaretMover Instance
        = new TextCaretMover();

      /// <inheritdoc />
      public TextCaret Convert(BlockCaret caret)
        => FromBlockCaret(caret);

      public BlockCaret MoveForward(BlockCaret caret)
        => FromBlockCaret(caret).GetNextPosition().ToBlockCaret();

      public BlockCaret MoveBackward(BlockCaret caret)
        => FromBlockCaret(caret).GetPreviousPosition().ToBlockCaret();

      public bool IsAtBlockEnd(BlockCaret caret)
        => FromBlockCaret(caret).IsAtBlockEnd;

      public ContentBlock GetBlock(BlockCaret blockCaret) 
        => FromBlockCaret(blockCaret).Block;

      public bool IsAtBlockStart(BlockCaret caret)
        => FromBlockCaret(caret).IsAtBlockStart;

      public MeasuredRectangle Measure(BlockCaret blockCaret)
        => FromBlockCaret(blockCaret).Measure();

      public ISerializedBlockCaret Serialize(BlockCaret caret)
        => FromBlockCaret(caret).Serialize();
    }
  }
}