using System;
using System.Collections.Generic;
using System.Linq;
using TextRight.ContentEditor.Core.ObjectModel.Blocks;
using TextRight.ContentEditor.Core.ObjectModel.Cursors;

namespace TextRight.ContentEditor.Core.Editing
{
  /// <summary> Move the caret to the next word in a TextBlock. </summary>
  public class MoveCaretNextWordCommand : CaretCommand
  {
    /// <inheritdoc/>
    public override bool CanActivate(DocumentEditorContext context)
    {
      return context.Caret.Cursor.Block is TextBlock && !context.Caret.Cursor.IsAtEnd;
    }

    /// <inheritdoc/>
    public override bool Activate(DocumentCursor caret, CaretMovementMode movementMode)
    {
      using (var current = caret.Cursor.Copy())
      {
        if (TextBlockMoveToNextWord(current.Cursor))
        {
          caret.MoveTo(current);
          return true;
        }

        return false;
      }
    }

    private bool TextBlockMoveToNextWord(IBlockContentCursor cursor)
    {
      var blockCaret = cursor;

      // we either don't know what kind of block cursor it is, or we want to move
      // to the next block anyways. 
      if (blockCaret.IsAtEnd)
      {
        return false;
      }

      var textCursor = (TextBlockCursor)blockCaret;

      var characterType = TextCharacterizer.Characterize(textCursor.CharacterAfter);
      TextCharacterizer.CharacterType lastCharacterType;

      // navigate until we get to a character category that A) is different from the last
      // seen category and B) is not an Don't-Care-Category (AKA < 0)
      do
      {
        lastCharacterType = characterType;

        if (!textCursor.MoveForward())
        {
          break;
        }

        characterType = TextCharacterizer.Characterize(textCursor.CharacterAfter);
      } while (lastCharacterType == characterType
               || characterType < TextCharacterizer.CharacterType.PlannedCharacters);

      return true;
    }
  }
}