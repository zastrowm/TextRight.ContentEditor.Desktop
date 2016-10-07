using System;
using System.Collections.Generic;
using System.Linq;

namespace TextRight.Core.Editing.Commands.Caret
{
  /// <summary> Characterizes text into Letter, punctuation, or letter.  Used when navigating text by words. </summary>
  internal static class TextCharacterizer
  {
    /// <summary> Identifies the "type" of the character for analyzing groups of words. </summary>
    /// <param name="letter"> The letter that should be categorized. </param>
    /// <returns> The "type" of the character </returns>
    public static CharacterType Characterize(char letter)
    {
      if (letter == '\'')
      {
        // TODO should we do this?
        // we treat these as contractions
        return CharacterType.Letter;
      }
      if (Char.IsPunctuation(letter))
      {
        return CharacterType.Punctuation;
      }
      else if (Char.IsLetter(letter))
      {
        return CharacterType.Letter;
      }
      else
      {
        return CharacterType.Unknown;
      }
    }

    public enum CharacterType
    {
      Unknown = -1,
      PlannedCharacters = 0,
      Letter = 1,
      Punctuation = 2,
    }
  }
}