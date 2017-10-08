using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using TextRight.Core.ObjectModel.Cursors;
using TextRight.Core.Utilities;

namespace TextRight.Core.ObjectModel.Blocks.Text
{
  /// <summary> Iterates a TextBlock. </summary>
  /// <remarks>
  ///  When pointing at a TextBlock, the cursor is really pointing at one of the
  ///  fragments within the TextBlock, in which case there are a couple special
  ///  cases of where the cursor can be looking:
  ///    - At the beginning of the block (also the beginning of the first span).
  ///      In which case, the OffsetIntoSpan will be 0, the only time it should
  ///      ever be zero
  ///    - In the middle of a fragment, thus OffsetIntoSpan is greater than 0  
  ///      and less than or equal to Fragment.Length
  ///    - At the end of a fragment, in which case OffsetIntoSpan is equal to  
  ///      Fragment.Length
  ///    - At the end of the last fragment, which means that OffsetIntoSpan is
  ///      equal to Fragment.Length and we're pointing to the last fragment.
  /// </remarks>
  [DebuggerDisplay("TextBlockCursor(FragmentIndex={Fragment.Index}, Offset={OffsetIntoSpan})")]
  public sealed class TextBlockCursor : BaseBlockContentCursor<TextBlockCursor, TextBlock>,
                                        IEquatable<TextBlockCursor>
  {
    private const char NullCharacter = '\0';

    /// <summary> Constructor. </summary>
    /// <param name="block"> The block for which the cursor is valid. </param>
    public TextBlockCursor(TextBlock block)
      : base(block)
    {
    }

    public TextBlockCursor(TextCaret cursor)
      : base(cursor.Fragment.Parent)
    {
      // TODO verify the offset is correct.
      Fragment = cursor.Fragment;
      OffsetIntoSpan = cursor.Offset.GraphemeOffset;
    }

    /// <summary>
    ///  The span that the cursor is currently pointing towards.
    /// </summary>
    public StyledTextFragment Fragment { get; private set; }

    /// <summary>
    ///  The offset into <see cref="Fragment"/> where this cursor is pointing.
    /// </summary>
    public int OffsetIntoSpan { get; private set; }

    internal SnapshotState State
    {
      get
      {
        return new SnapshotState()
               {
                 OffsetIntoSpan = OffsetIntoSpan,
                 Fragment = Fragment,
               };
      }
      set
      {
        OffsetIntoSpan = value.OffsetIntoSpan;
        Fragment = value.Fragment;
      }
    }

    internal struct SnapshotState
    {
      public int OffsetIntoSpan;
      public StyledTextFragment Fragment;
      public TextBlock Block;
    }

    /// <summary> Get the character after the current cursor position. </summary>
    public char CharacterAfter
      => ToValue().CharacterAfter.Character;

    /// <summary> Get the character before the current cursor position. </summary>
    public char CharacterBefore
    {
      get
      {
        var previousPosition = ToValue().GetPreviousPosition();
        if (previousPosition.IsValid)
          return previousPosition.CharacterAfter.Character;

        return NullCharacter;
      }
    }

    /// <inheritdoc />
    public override void MoveToBeginning()
    {
      Fragment = Block.Content.FirstFragment;
      OffsetIntoSpan = 0;
    }

    /// <inheritdoc />
    public override void MoveToEnd()
    {
      Fragment = Block.Content.LastFragment;
      OffsetIntoSpan = Fragment.NumberOfChars;
    }

    /// <inheritdoc />
    public override bool IsAtEnd
    {
      get
      {
        return OffsetIntoSpan >= Fragment.NumberOfChars
               && Fragment.Next == null;
      }
    }

    /// <inheritdoc />
    public override bool MoveForward()
    {
      // we move right to end of the span
      if (OffsetIntoSpan < Fragment.NumberOfChars)
      {
        OffsetIntoSpan++;
        return true;
      }

      // we're at the end of the span and as long as we can move to the next span,
      // do so. 
      if (Fragment.Next != null)
      {
        Fragment = Fragment.Next;
        // we're never at offset=0 unless we're at the beginning of the first span.
        OffsetIntoSpan = 1;
        return true;
      }

      return false;
    }

    /// <inheritdoc />
    public override bool IsAtBeginning
    {
      get { return OffsetIntoSpan == 0; }
    }

    /// <inheritdoc />
    public override MeasuredRectangle MeasureCursorPosition()
    {
      return ToValue().Measure();
    }

    /// <inheritdoc />
    public override bool MoveBackward()
    {
      // we're at the beginning of the first span
      if (OffsetIntoSpan == 0)
        return false;

      if (OffsetIntoSpan > 2)
      {
        OffsetIntoSpan--;
        return true;
      }

      if (OffsetIntoSpan != 1 || Fragment.Index == 0)
      {
        // at offset 1 of the first span, so go to offset 0 which indicates the
        // beginning of the block. 
        OffsetIntoSpan--;
        return true;
      }

      // we're at the beginning of the current span, so go ahead and move onto
      // previous span. 
      Fragment = Fragment.Previous;
      OffsetIntoSpan = Fragment.NumberOfChars;

      return true;
    }

    /// <inheritdoc/>
    public override ISerializedBlockCursor Serialize()
      => new SerializedCursor(this);

    /// <inheritdoc/>
    public bool CanInsertText()
    {
      return true;
    }

    /// <inheritdoc/>
    public void InsertText(string text)
    {
      Fragment.InsertText(text, OffsetIntoSpan);
      OffsetIntoSpan += text.Length;
    }

    /// <inheritdoc />
    public bool CanDeleteText()
    {
      return true;
    }

    /// <inheritdoc />
    public bool DeleteText(int numberOfCharacters)
    {
      //while (numberOfCharacters > 0)
      {
        int numberOfCharactersRemainingInCurrentFragment = Fragment.NumberOfChars - OffsetIntoSpan;
        int numberOfCharactersToRemove = numberOfCharacters;

        if (numberOfCharactersToRemove > numberOfCharactersRemainingInCurrentFragment)
        {
          numberOfCharactersToRemove = numberOfCharactersRemainingInCurrentFragment;
        }

        // TODO special case when we're deleting the entire fragment
        Fragment.RemoveCharacters(OffsetIntoSpan, numberOfCharactersToRemove);

        numberOfCharacters -= numberOfCharactersToRemove;
        // TODO what happens for multiple fragments
        return numberOfCharactersToRemove > 0;
      }
    }

    /// <summary>
    ///  Extracts the content from the current location to the end of the block.
    /// </summary>
    public StyledTextFragment[] ExtractToEnd()
    {
      return Block.ExtractContentToEnd(this);
    }

    /// <inheritdoc />
    protected override TextBlockCursor CreateInstance(TextBlock block)
    {
      return new TextBlockCursor(block);
    }

    /// <inheritdoc />
    protected override void MoveToOverride(TextBlockCursor cursor)
    {
      State = cursor.State;
    }

    /// <summary> Creates a <see cref="TextCaret"/> from this cursor. </summary>
    public TextCaret ToValue() 
      => TextCaret.FromOffset(Fragment, OffsetIntoSpan);

    internal void MoveTo(TextCaret newPosition)
    {
      State = new SnapshotState()
              {
                Block = newPosition.Fragment.Parent,
                Fragment = newPosition.Fragment,
                OffsetIntoSpan = newPosition.Offset.GraphemeOffset,
              };
    }

    /// <inheritdoc />
    public bool Equals(TextBlockCursor other)
    {
      if (ReferenceEquals(null, other))
        return false;
      if (ReferenceEquals(this, other))
        return true;

      return Equals(Block, other.Block) && Equals(Fragment, other.Fragment) &&
             OffsetIntoSpan == other.OffsetIntoSpan;
    }

    /// <inheritdoc />
    public override bool Equals(object obj)
    {
      if (ReferenceEquals(null, obj))
        return false;
      if (ReferenceEquals(this, obj))
        return true;
      if (obj.GetType() != GetType())
        return false;
      return Equals((TextBlockCursor)obj);
    }

    /// <inheritdoc />
    public override int GetHashCode()
    {
      unchecked
      {
        var hashCode = Block?.GetHashCode() ?? 0;
        hashCode = (hashCode * 397) ^ (Fragment?.GetHashCode() ?? 0);
        hashCode = (hashCode * 397) ^ OffsetIntoSpan;
        return hashCode;
      }
    }

    /// <summary> The cursor's serialized data. </summary>
    public class SerializedCursor : BaseSerializedCursor<TextBlockCursor>
    {
      private readonly int _spanId;
      private readonly int _offset;

      public SerializedCursor(TextBlockCursor cursor)
        : base(cursor)
      {
        _spanId = cursor.Fragment.Index;
        _offset = cursor.OffsetIntoSpan;
      }

      public override void Deserialize(TextBlockCursor cursor)
      {
        cursor.OffsetIntoSpan = _offset;
        cursor.Fragment = cursor.Block.Content.GetSpanAtIndex(_spanId);
      }
    }
  }
}