using System;
using System.Collections.Generic;
using System.Linq;
using TextRight.ContentEditor.Core.ObjectModel.Blocks;
using TextRight.ContentEditor.Core.ObjectModel.Cursors;

namespace TextRight.ContentEditor.Core.Editing
{
  /// <summary> Move the caret to the previous word in a TextBlock. </summary>
  public class MoveCaretPreviousWordCommand : CaretCommand
  {
    /// <inheritdoc />
    public override string Id
      => "caret.moveBackwardByWord";

    /// <inheritdoc/>
    public override bool CanActivate(DocumentEditorContext context)
    {
      return context.Caret.Cursor.Block is TextBlock && !context.Caret.Cursor.IsAtBeginning;
    }

    /// <inheritdoc/>
    public override bool Activate(DocumentCursor caret, CaretMovementMode movementMode)
    {
      using (var current = caret.Cursor.Copy())
      {
        if (TextBlockMove(current.Cursor))
        {
          caret.MoveTo(current);
          return true;
        }

        return false;
      }
    }

    private bool TextBlockMove(IBlockContentCursor cursor)
    {
      var blockCaret = cursor;

      // we either don't know what kind of block cursor it is, or we want to move
      // to the next block anyways. 
      if (blockCaret.IsAtBeginning)
        return false;

      var textCursor = (TextBlockCursor)blockCaret;

      TextCharacterizer.CharacterType characterType;
      TextCharacterizer.CharacterType lastCharacterType;

      // navigate backwards through all of the initial whitespace/undesirable characters
      // until we reach a non whitespace/undesirable.
      do
      {
        characterType = TextCharacterizer.Characterize(textCursor.CharacterBefore);
      } while (characterType < TextCharacterizer.CharacterType.PlannedCharacters
               && textCursor.MoveBackward());

      // now move backwards until we change categories
      do
      {
        lastCharacterType = characterType;

        if (!textCursor.MoveBackward())
        {
          break;
        }

        characterType = TextCharacterizer.Characterize(textCursor.CharacterBefore);
      } while (lastCharacterType == characterType);

      return true;
    }
  }
}