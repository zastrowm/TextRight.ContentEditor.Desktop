using System;
using System.Collections.Generic;
using System.Linq;
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
    public class TextBlockCursor : IBlockContentCursor,
                                   ITextContentCursor
    {
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

      /// <summary> Get the character after the current cursor position. </summary>
      public char CharacterAfter
        => OffsetIntoSpan != Fragment.Length ? Fragment.Text[OffsetIntoSpan] : '\0';

      /// <summary> Get the character before the current cursor position. </summary>
      public char CharacterBefore
        => OffsetIntoSpan != 0 ? Fragment.Text[OffsetIntoSpan - 1] : '\0';

      /// <inheritdoc />
      public void MoveToBeginning()
      {
        Fragment = _block._spans[0];
        OffsetIntoSpan = 0;
      }

      /// <inheritdoc />
      public void MoveToEnd()
      {
        Fragment = _block._spans[_block._spans.Count - 1];
        OffsetIntoSpan = Fragment.Length;
      }

      /// <inheritdoc />
      public bool IsAtEnd
      {
        get
        {
          return OffsetIntoSpan >= Fragment.Length
                 && Fragment.Index + 1 >= _block._spans.Count;
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
        if (Fragment.Index + 1 < _block._spans.Count)
        {
          Fragment = _block._spans[Fragment.Index + 1];
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
        return !IsAtBeginning && CharacterBefore != ' '
          ? MeasureBackward().FlattenRight()
          : MeasureForward().FlattenLeft();
      }

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
        Fragment = _block._spans[Fragment.Index - 1];
        OffsetIntoSpan = Fragment.Length;

        return true;
      }

      /// <inheritdoc/>
      public ISerializedBlockCursor Serialize()
        => new SerializedData(this);

      /// <inheritdoc/>
      bool ITextContentCursor.CanInsertText()
      {
        return true;
      }

      /// <inheritdoc/>
      void ITextContentCursor.InsertText(string text)
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
      void ITextContentCursor.DeleteText(int numberOfCharacters)
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
          var block = ((TextBlock)owner.Root.GetBlockFromPath(_path));
          return new TextBlockCursor(block)
                 {
                   OffsetIntoSpan = _offset,
                   Fragment = block.GetSpanAtIndex(_spanId),
                 };
        }
      }

      /// <summary>
      ///  Extracts the content from the current location to the end of the block.
      /// </summary>
      public StyledTextFragment[] ExtractToEnd()
      {
        return _block.ExtractContentToEnd(this);
      }
    }
  }
}