using System;
using System.Collections.Generic;
using System.Linq;
using TextRight.ContentEditor.Core.ObjectModel.Blocks;

namespace TextRight.ContentEditor.Core.Editing.Commands
{
  /// <summary> Moves a caret forward by through different words. </summary>
  internal static class CaretWordMover
  {
    /// <summary> Moves the caret to the beginning of the next word. </summary>
    /// <param name="context"> The context's whose caret should be moved. </param>
    public static void MoveCaretToBeginningOfNextWord(DocumentEditorContext context)
    {
      var caret = context.Caret;
      var blockCaret = caret.BlockCursor;

      if (!blockCaret.IsAtEnd && blockCaret is TextBlock.TextBlockCursor)
      {
        var textCursor = (TextBlock.TextBlockCursor)blockCaret;

        CharacterType characterType = Characterize(textCursor.CharacterAfter);
        CharacterType lastCharacterType;

        // navigate until we get to a character category that A) is different from the last
        // seen category and B) is not an Don't-Care-Category (AKA < 0)
        do
        {
          lastCharacterType = characterType;

          if (!textCursor.MoveForward())
          {
            break;
          }

          characterType = Characterize(textCursor.CharacterAfter);
        } while (lastCharacterType == characterType
                 || characterType < CharacterType.PlannedCharacters);
      }
      else
      {
        // we either don't know what kind of block cursor it is, or we want to move
        // to the next block anyways. 
        caret.MoveForward();
      }
    }

    /// <summary> Moves the caret to the end of the previous word. </summary>
    /// <param name="context"> The context's whose caret should be moved. </param>
    public static void MoveCaretToEndOfPreviousWord(DocumentEditorContext context)
    {
      var caret = context.Caret;
      var blockCaret = caret.BlockCursor;

      if (!blockCaret.IsAtBeginning && blockCaret is TextBlock.TextBlockCursor)
      {
        var textCursor = (TextBlock.TextBlockCursor)blockCaret;

        CharacterType characterType;
        CharacterType lastCharacterType;

        // navigate backwards through all of the initial whitespace/undesirable characters
        // until we reach a non whitespace/undesirable.
        do
        {
          characterType = Characterize(textCursor.CharacterBefore);
        } while (characterType < CharacterType.PlannedCharacters
                 && textCursor.MoveBackward());

        // now move backwards until we change categories
        do
        {
          lastCharacterType = characterType;

          if (!textCursor.MoveBackward())
          {
            break;
          }

          characterType = Characterize(textCursor.CharacterBefore);
        } while (lastCharacterType == characterType);
      }
      else
      {
        // we either don't know what kind of block cursor it is, or we want to move
        // to the next block anyways. 
        caret.MoveBackward();
      }
    }

    /// <summary> Identifies the "type" of the character for analyzing groups of words. </summary>
    /// <param name="letter"> The letter that should be categorized. </param>
    /// <returns> The "type" of the character </returns>
    private static CharacterType Characterize(char letter)
    {
      if (letter == '\'')
      {
        // TODO should we do this?
        // we treat these as contractions
        return CharacterType.Letter;
      }
      if (char.IsPunctuation(letter))
      {
        return CharacterType.Punctuation;
      }
      else if (char.IsLetter(letter))
      {
        return CharacterType.Letter;
      }
      else
      {
        return CharacterType.Unknown;
      }
    }

    // NOTE the numbers are important for the above algorithms
    private enum CharacterType
    {
      Unknown = -1,
      PlannedCharacters = 0,
      Letter = 1,
      Punctuation = 2,
    }
  }
}