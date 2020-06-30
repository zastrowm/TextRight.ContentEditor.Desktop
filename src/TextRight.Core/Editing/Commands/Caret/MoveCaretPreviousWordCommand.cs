using System;
using System.Collections.Generic;
using System.Linq;
using TextRight.Core.Cursors;
using TextRight.Core.ObjectModel.Blocks.Text;
using TextRight.Core.ObjectModel.Cursors;

namespace TextRight.Core.Commands.Caret
{
  /// <summary> Move the caret to the previous word in a TextBlock. </summary>
  public class MoveCaretPreviousWordCommand : CaretCommand
  {
    /// <inheritdoc />
    public override string Id
      => "caret.moveBackwardByWord";

    /// <inheritdoc/>
    public override bool CanActivate(DocumentEditorContext context)
      => context.Selection.Start.TryCast<TextCaret>(out var textCaret) && !textCaret.IsAtBlockEnd;

    /// <inheritdoc/>
    public override bool Activate(DocumentSelection cursor, CaretMovementMode movementMode)
    {
      var original = (TextCaret)cursor.Start;
      var newPosition = TextBlockMove(original);
      if (original != newPosition)
      {
        cursor.MoveTo(newPosition);
        return true;
      }

      return false;
    }

    private TextCaret TextBlockMove(TextCaret textCaret)
    {
      if (textCaret.IsAtBlockStart)
        return textCaret;

      TextCharacterizer.CharacterType characterType;
      TextCharacterizer.CharacterType lastCharacterType;

      // navigate backwards through all of the initial whitespace/undesirable characters
      // until we reach a non whitespace/undesirable.
      do
      {
        characterType = TextCharacterizer.Characterize(GetPreviousCharacter(textCaret));
      } while (characterType < TextCharacterizer.CharacterType.PlannedCharacters
               && CaretHelpers.TryMoveBackward(ref textCaret));

      // now move backwards until we change categories
      do
      {
        lastCharacterType = characterType;

        if (!CaretHelpers.TryMoveBackward(ref textCaret))
        {
          break;
        }

        characterType = TextCharacterizer.Characterize(GetPreviousCharacter(textCaret));
      } while (lastCharacterType == characterType);

      return textCaret;
    }

    private TextUnit GetPreviousCharacter(TextCaret caret)
    {
      var previous = caret.GetPreviousPosition();
      if (previous.IsValid)
        return previous.CharacterAfter;
      return TextUnit.Default;
    }
  }
}