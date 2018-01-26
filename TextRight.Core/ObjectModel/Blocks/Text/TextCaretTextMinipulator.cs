using System;
using System.Collections.Generic;
using System.Linq;

namespace TextRight.Core.ObjectModel.Blocks.Text
{
  /// <summary> Exposes methods for inserting and removing text using a TextCaret. </summary>
  public static class TextCaretTextMinipulator
  {
    /// <summary> Deletes text at the given position. </summary>
    public static TextCaret DeleteText(this TextCaret caret, int numberOfCharacters)
    {
      // TODO GRAPHEMES

      //while (numberOfCharacters > 0)
      {
        int numberOfCharactersRemainingInCurrentFragment = caret.Span.NumberOfChars - caret.Offset.CharOffset;
        int numberOfCharactersToRemove = numberOfCharacters;

        if (numberOfCharactersToRemove > numberOfCharactersRemainingInCurrentFragment)
        {
          numberOfCharactersToRemove = numberOfCharactersRemainingInCurrentFragment;
        }

        // TODO special case when we're deleting the entire fragment
        caret.Span.RemoveCharacters(caret.Offset.CharOffset, numberOfCharactersToRemove);

        numberOfCharacters -= numberOfCharactersToRemove;
        // TODO what happens for multiple fragments
        return TextCaret.FromOffset(caret.Span, caret.Offset.GraphemeOffset);
      }
    }

    /// <summary> Inserts text at the given location. </summary>
    public static TextCaret InsertText(this TextCaret original, string text)
    {
      var caret = original;
      var newDestination = caret.Offset.CharOffset + text.Length;
      caret.Span.InsertText(text, caret.Offset.CharOffset);

      while (caret.Offset.CharOffset < newDestination)
      {
        caret = caret.GetNextPosition();
      }

      return caret;
    }
  }
}