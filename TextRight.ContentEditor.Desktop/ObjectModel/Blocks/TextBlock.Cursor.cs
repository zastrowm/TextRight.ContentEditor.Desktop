using System;
using System.Collections.Generic;
using System.Linq;

namespace TextRight.ContentEditor.Desktop.ObjectModel.Blocks
{
  /// <summary> A text block. </summary>
  public partial class TextBlock
  {
    /// <summary> Iterates a TextBlock. </summary>
    internal class TextBlockCursor : IBlockContentCursor
    {
      private readonly TextBlock _block;
      private int _offsetIntoSpan;
      private TextSpan _span;

      /// <summary> Constructor. </summary>
      /// <param name="block"> The block for which the cursor is valid. </param>
      public TextBlockCursor(TextBlock block)
      {
        _block = block;
      }

      /// <inheritdoc />
      public Block Block
        => _block;

      /// <summary> Get the character after the current cursor position. </summary>
      public char CharacterAfter
        => _offsetIntoSpan != _span.Length ? _span.Text[_offsetIntoSpan] : '\0';

      /// <summary> Get the character before the current cursor position. </summary>
      public char CharacterBefore
        => _offsetIntoSpan != 0 ? _span.Text[_offsetIntoSpan - 1] : '\0';

      /// <inheritdoc />
      public void MoveToBeginning()
      {
        _span = _block._spans[0];
        _offsetIntoSpan = 0;
      }

      /// <inheritdoc />
      public void MoveToEnd()
      {
        _span = _block._spans[_block._spans.Count - 1];
        _offsetIntoSpan = _span.Length;
      }

      /// <inheritdoc />
      public bool MoveForward()
      {
        // we move right to end of the span
        if (_offsetIntoSpan < _span.Length)
        {
          _offsetIntoSpan++;
          return true;
        }

        // we're at the end of the span and as long as we can move to the next span,
        // do so. 
        if (_span.Index + 1 < _block._spans.Count)
        {
          _span = _block._spans[_span.Index + 1];
          // we're never at offset=0 unless we're at the beginning of the first span.
          _offsetIntoSpan = 1;
          return true;
        }

        return false;
      }

      /// <inheritdoc />
      public bool MoveBackward()
      {
        // we're at the beginning of the first span
        if (_offsetIntoSpan == 0)
          return false;

        if (_offsetIntoSpan > 2)
        {
          _offsetIntoSpan--;
          return true;
        }

        if (_offsetIntoSpan != 1 || _span.Index == 0)
        {
          // at offset 1 of the first span, so go to offset 0 which indicates the
          // beginning of the block. 
          _offsetIntoSpan--;
          return true;
        }

        // we're at the beginning of the current span, so go ahead and move onto
        // previous span. 
        _span = _block._spans[_span.Index - 1];
        _offsetIntoSpan = _span.Length;

        return true;
      }
    }
  }
}