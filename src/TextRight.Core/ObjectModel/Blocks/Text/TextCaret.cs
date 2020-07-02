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
  [DebuggerDisplay("Offset={Offset}, Content={Content}")]
  public readonly struct TextCaret : ISimpleCaret<TextCaret, TextBlock>,
                                     IEquatable<TextCaret>,
                                     IBlockCaret
  {
    /// <summary> A cursor which represents an invalid location. </summary>
    public static readonly TextCaret Invalid
      = default(TextCaret);

    public static ICaretMover<TextCaret> Factory
      => TextCaretMover.Instance;

    private TextCaret(TextBlockContent content, TextOffset offset)
    {
      Content = content;
      Offset = offset;
    }

    /// <summary> The text-content object that this caret is pointing to. </summary>
    public TextBlockContent Content { get; }
    
    internal StringFragmentBuffer Buffer
      => Content.Buffer;
    
    /// <summary>
    ///  True if the cursor represents a location in a fragment, false if there is no fragment
    ///  associated with the cursor.
    /// </summary>
    public bool IsValid
      => Content != null;

    /// <summary> The block that this cursor is associated with. </summary>
    public TextBlock Block
      => Content.Owner;

    /// <summary> The offset into <see cref="Span"/> that this cursor is pointing. </summary>
    public TextOffset Offset { get; }

    /// <inheritdoc />
    public bool IsAtBlockStart
      => Offset.GraphemeOffset == 0;

    /// <inheritdoc />
    public bool IsAtBlockEnd
      => Offset.GraphemeOffset >= Content.GraphemeLength;

    /// <summary> Get the character after the current cursor position. </summary>
    public TextUnit CharacterAfter
    {
      get
      {
        if (!IsAtBlockEnd)
          return Buffer.GetCharacterAt(Offset);

        return TextUnit.Default;
      }
    }

    /// <inheritdoc />
    public TextCaret GetNextPosition()
    {
      var maybeNextOffset = Buffer.GetNextOffset(Offset);
      if (maybeNextOffset != null)
      {
        var nextOffset = maybeNextOffset.GetValueOrDefault();
        return new TextCaret(Content, nextOffset);
      }

      // point to just after the last character (unless we're empty in which case we're already doing that)
      if (Buffer.GetLastOffset().GraphemeOffset == Offset.GraphemeOffset
          &&Buffer.GraphemeLength != 0)
      {
        return new TextCaret(Content, TextOffsetHelpers.CreateAfterTextOffset(Buffer));
      }

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
        return new TextCaret(Content, Buffer.GetPreviousOffset(Offset).GetValueOrDefault());
      }

      Debug.Fail("How did we get here?");
      return Invalid;
    }

    /// <inheritdoc />
    public bool Equals(TextCaret other)
      => Equals(Content, Content) && Offset == other.Offset;

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
        return ((Content?.GetHashCode() ?? 0) * 397) ^ Offset.GetHashCode();
      }
    }

    /// <summary> Converts the current instance into a non-specific block caret. </summary>
    public BlockCaret ToBlockCaret()
    {
      if (!IsValid)
        return BlockCaret.Invalid;

      return new BlockCaret(TextCaretMover.Instance,
                            Content,
                            Offset.CharOffset,
                            Offset.GraphemeOffset,
                            Offset.GraphemeLength);
    }

    /// <summary> Gets a cursor that is looking at the beginning of the content. </summary>
    public static TextCaret FromBeginning(TextBlockContent content)
    {
      return new TextCaret(content, content.Buffer.GetFirstOffset());
    }

    /// <summary> Gets a cursor that is looking at the end of the content. </summary>
    public static TextCaret FromEnd(TextBlockContent content)
    {
      return new TextCaret(content,
                           TextOffsetHelpers.CreateAfterTextOffset(content.Buffer));
    }

    /// <summary>
    ///  Gets a cursor that is looking at the character at the given index.
    ///  
    ///  Potentially very expensive, and should be avoided in favor of
    ///  <see cref="FromOffset(TextSpan,int)"/> instead.
    /// </summary>
    /// <param name="span"> The span that the cursor is currently pointing towards. </param>
    public static TextCaret FromCharacterIndex(TextBlockContent content, int characterIndex)
    {
      return FromOffset(content, content.Buffer.GetOffsetToCharacterIndex(characterIndex).GetValueOrDefault());
    }

    /// <summary> Gets a cursor that is looking at the grapheme at the given index. </summary>
    public static TextCaret FromOffset(TextBlockContent content, int graphemeIndex)
    {
      // TODO validate
      return FromOffset(content, content.Buffer.GetOffsetToGraphemeIndex(graphemeIndex).GetValueOrDefault());
    }

    // <summary> Gets a cursor that is looking at the grapheme at the given index. </summary>
    public static TextCaret FromOffset(TextBlockContent span, TextOffset offset)
    {
      // TODO validate
      return new TextCaret(span, offset);
    }

    /// <summary> Gets an object that holds the serialized data for this caret. </summary>
    public ISerializedBlockCaret Serialize()
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
      => TextCaretMover.Instance.FromBlockCaret(caret);

    private class TextCaretMover : SimpleCaretMover<TextCaret, TextBlock>
    {
      internal static readonly TextCaretMover Instance
        = new TextCaretMover();

      /// <inheritdoc />
      public override TextCaret FromBlockCaret(BlockCaret caret)
      {
        if (caret.Mover != Instance)
          throw new ArgumentException("Caret does not represent the content of a TextCaret", nameof(caret));

        var offset = new TextOffset(caret.InstanceOffset1, caret.InstanceOffset2, caret.InstanceOffset3);
        return new TextCaret((TextBlockContent)caret.InstanceDatum, offset);
      }
    }

    private class SerializedData : ISerializedBlockCaret
    {
      private readonly int _graphemeOffset;
      private readonly BlockPath _pathToBlock;

      public SerializedData(TextCaret caret)
      {
        _graphemeOffset = caret.Offset.GraphemeOffset;
        _pathToBlock = caret.Block.GetBlockPath();
      }

      public BlockCaret Deserialize(DocumentEditorContext context)
      {
        var block = _pathToBlock.Get(context.Document);
        var content = ((TextBlock)block).Content;
        return FromOffset(content, _graphemeOffset);
      }
    }
  }
}