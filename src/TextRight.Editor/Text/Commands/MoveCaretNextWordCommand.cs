using System;
using System.Collections.Generic;
using System.Linq;
using TextRight.Core;
using TextRight.Core.Commands.Caret;
using TextRight.Core.Cursors;
using TextRight.Core.ObjectModel.Blocks.Text;
using TextRight.Core.ObjectModel.Cursors;

namespace TextRight.Editor.Text.Commands
{
  /// <summary> Move the caret to the next word in a TextBlock. </summary>
  public class MoveCaretNextWordCommand : CaretCommand
  {
    /// <inheritdoc />
    public override string Id
      => "caret.moveForwardByWord";

    /// <inheritdoc/>
    public override bool CanActivate(DocumentEditorContext context) 
      => context.Selection.Start.TryCast<TextCaret>(out var caret) && !caret.IsAtBlockEnd;

    /// <inheritdoc/>
    public override bool Activate(DocumentSelection cursor, CaretMovementMode movementMode, SelectionMode mode)
    {
      var original = (TextCaret)cursor.Start;
      var newPosition = TextBlockMoveToNextWord(original);
      if (original != newPosition)
      {
        cursor.MoveTo(newPosition, mode);
        return true;
      }

      return false;
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