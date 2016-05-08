﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using TextRight.ContentEditor.Core.Editing.Commands;
using TextRight.ContentEditor.Core.Utilities;

namespace TextRight.ContentEditor.Core.ObjectModel.Blocks
{
  /// <summary> A text block. </summary>
  public partial class TextBlock
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
    public class TextBlockCursor : IBlockContentCursor,
                                   ITextContentCursor,
                                   ICommandProcessorHook,
                                   IEquatable<TextBlockCursor>
    {
      private const char NullCharacter = '\0';

      private readonly TextBlock _block;

      /// <summary> Constructor. </summary>
      /// <param name="block"> The block for which the cursor is valid. </param>
      public TextBlockCursor(TextBlock block)
      {
        _block = block;
      }

      /// <summary>
      ///  The span that the cursor is currently pointing towards.
      /// </summary>
      public StyledTextFragment Fragment { get; private set; }

      /// <summary>
      ///  The offset into <see cref="Fragment"/> where this cursor is pointing.
      /// </summary>
      public int OffsetIntoSpan { get; private set; }

      /// <inheritdoc />
      public Block Block
        => _block;

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
        => OffsetIntoSpan != Fragment.Length ? Fragment.Text[OffsetIntoSpan] : NullCharacter;

      /// <summary> Get the character before the current cursor position. </summary>
      public char CharacterBefore
        => OffsetIntoSpan != 0 ? Fragment.Text[OffsetIntoSpan - 1] : NullCharacter;

      /// <inheritdoc />
      public void MoveToBeginning()
      {
        Fragment = _block.FirstFragment;
        OffsetIntoSpan = 0;
      }

      /// <inheritdoc />
      public void MoveToEnd()
      {
        Fragment = _block.LastFragment;
        OffsetIntoSpan = Fragment.Length;
      }

      /// <inheritdoc />
      public bool IsAtEnd
      {
        get
        {
          return OffsetIntoSpan >= Fragment.Length
                 && Fragment.Next == null;
        }
      }

      /// <inheritdoc />
      public bool MoveForward()
      {
        // we move right to end of the span
        if (OffsetIntoSpan < Fragment.Length)
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
      public bool IsAtBeginning
      {
        get { return OffsetIntoSpan == 0; }
      }

      private MeasuredRectangle MeasureForward()
      {
        if (IsAtEnd || Fragment.Target == null)
          return MeasuredRectangle.Invalid;

        return Fragment.Target.Measure(OffsetIntoSpan);
      }

      private MeasuredRectangle MeasureBackward()
      {
        if (IsAtBeginning || Fragment.Target == null)
          return MeasuredRectangle.Invalid;

        return Fragment.Target.Measure(OffsetIntoSpan - 1);
      }

      /// <inheritdoc />
      public MeasuredRectangle MeasureCursorPosition()
      {
        // we want to measure the next character unless the previous character was
        // a space (as the text will most likely appear on the next line anyways) 
        bool shouldMeasureNext = IsAtBeginning
                                 || (!IsAtEnd && CharacterBefore == ' ');

        return shouldMeasureNext
          ? MeasureForward().FlattenLeft()
          : MeasureBackward().FlattenRight();
      }

      /// <summary> Attempts to process incoming commands. </summary>
      public ICommandProcessor CommandProcessor
        => TextBlockCursorCommandProcessor.Instance;

      /// <inheritdoc />
      public bool MoveBackward()
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
        OffsetIntoSpan = Fragment.Length;

        return true;
      }

      /// <inheritdoc/>
      public ISerializedBlockCursor Serialize()
        => new SerializedData(this);

      /// <inheritdoc/>
      public bool CanInsertText()
      {
        return true;
      }

      /// <inheritdoc/>
      public void InsertText(string text)
      {
        Fragment.Text = Fragment.Text.Insert(OffsetIntoSpan, text);
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
          int numberOfCharactersRemainingInCurrentFragment = Fragment.Length - OffsetIntoSpan;
          int numberOfCharactersToRemove = numberOfCharacters;

          if (numberOfCharactersToRemove > numberOfCharactersRemainingInCurrentFragment)
          {
            numberOfCharactersToRemove = numberOfCharactersRemainingInCurrentFragment;
          }

          // TODO special case when we're deleting the entire fragment
          Fragment.Text = Fragment.Text.Remove(OffsetIntoSpan, numberOfCharactersToRemove);

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
        return _block.ExtractContentToEnd(this);
      }

      /// <summary> Makes a deep copy of this instance. </summary>
      /// <returns> A copy of this instance. </returns>
      public TextBlockCursor Clone()
      {
        return new TextBlockCursor(_block)
               {
                 State = State
               };
      }

      /// <inheritdoc />
      public bool Equals(TextBlockCursor other)
      {
        if (ReferenceEquals(null, other))
          return false;
        if (ReferenceEquals(this, other))
          return true;

        return Equals(_block, other._block) && Equals(Fragment, other.Fragment) &&
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
          var hashCode = _block?.GetHashCode() ?? 0;
          hashCode = (hashCode * 397) ^ (Fragment?.GetHashCode() ?? 0);
          hashCode = (hashCode * 397) ^ OffsetIntoSpan;
          return hashCode;
        }
      }

      /// <summary> The cursor's serialized data. </summary>
      private class SerializedData : ISerializedBlockCursor
      {
        private readonly int _spanId;
        private readonly int _offset;
        private readonly BlockPath _path;

        public SerializedData(TextBlockCursor cursor)
        {
          _spanId = cursor.Fragment.Index;
          _offset = cursor.OffsetIntoSpan;
          _path = cursor.Block.GetBlockPath();
        }

        public IBlockContentCursor Deserialize(DocumentOwner owner)
        {
          var block = (TextBlock)_path.Get(owner);
          return new TextBlockCursor(block)
                 {
                   OffsetIntoSpan = _offset,
                   Fragment = block.GetSpanAtIndex(_spanId),
                 };
        }
      }
    }
  }
}