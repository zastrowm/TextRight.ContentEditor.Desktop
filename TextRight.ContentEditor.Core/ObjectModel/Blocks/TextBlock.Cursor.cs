using System;
using System.Collections.Generic;
using System.Linq;
using TextRight.ContentEditor.Desktop.Blocks;

namespace TextRight.ContentEditor.Desktop.ObjectModel.Blocks
{
  /// <summary> A text block. </summary>
  public partial class TextBlock
  {
    /// <summary> Iterates a TextBlock. </summary>
    internal class TextBlockCursor : IBlockContentCursor, ITextContentCursor
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
      void ITextContentCursor.InsertText(string text)
      {
        Fragment.Text = Fragment.Text.Insert(OffsetIntoSpan, text);
        OffsetIntoSpan += text.Length;
      }

      /// <inheritdoc/>
      bool ITextContentCursor.CanInsertText()
      {
        return true;
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
    }
  }
}