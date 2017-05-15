using System;
using System.Collections.Generic;
using System.Linq;
using TextRight.Core.Cursors;
using TextRight.Core.ObjectModel.Blocks;
using TextRight.Core.ObjectModel.Blocks.Text;
using TextRight.Core.ObjectModel.Cursors;

namespace TextRight.Core.Editing.Commands.Caret
{
  /// <summary> Move the caret to the next word in a TextBlock. </summary>
  public class MoveCaretNextWordCommand : CaretCommand
  {
    /// <inheritdoc />
    public override string Id
      => "caret.moveForwardByWord";

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
        var original = ((TextBlockCursor)current.Cursor).ToValue();
        var newPosition = TextBlockMoveToNextWord(original);
        if (original != newPosition)
        {
          ((TextBlockCursor)current.Cursor).MoveTo(newPosition);
          caret.MoveTo(current);
          return true;
        }

        return false;
      }
    }

    private TextCaret TextBlockMoveToNextWord(TextCaret textCaret)
    {
      if (textCaret.IsAtBlockEnd)
        return textCaret;

      var characterType = TextCharacterizer.Characterize(textCaret.CharacterAfter);
      TextCharacterizer.CharacterType lastCharacterType;

      // navigate until we get to a character category that A) is different from the last
      // seen category and B) is not an Don't-Care-Category (AKA < 0)
      do
      {
        lastCharacterType = characterType;

        if (!CaretHelpers.TryMoveForward(ref textCaret))
        {
          break;
        }

        characterType = TextCharacterizer.Characterize(textCaret.CharacterAfter);
      } while (lastCharacterType == characterType
               || characterType < TextCharacterizer.CharacterType.PlannedCharacters);

      return textCaret;
    }
  }
}