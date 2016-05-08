using System;
using System.Collections.Generic;
using System.Linq;
using TextRight.ContentEditor.Core.Editing;
using TextRight.ContentEditor.Core.Editing.Commands;

namespace TextRight.ContentEditor.Core.ObjectModel.Blocks
{
  /// <summary> How the cursor should move in response to various caret commands. </summary>
  internal class TextBlockCursorCommandProcessor : ICommandProcessor
  {
    /// <summary> Singleton Instance. </summary>
    public static TextBlockCursorCommandProcessor Instance { get; }
      = new TextBlockCursorCommandProcessor();

    /// <summary>
    ///  Constructor that prevents a default instance of this class from being
    ///  created.
    /// </summary>
    private TextBlockCursorCommandProcessor()
    {
    }

    /// <inheritdoc />
    bool ICommandProcessor.TryProcess(DocumentEditorContext context,
                                      EditorCommand command,
                                      CommandExecutionContext unused)
    {
      // we only work on text block cursors.
      var cursor = context.Cursor as TextBlockCursor;
      if (cursor == null)
        return false;

      var builtIn = command as BuiltInCaretNavigationCommand;
      if (builtIn != null)
      {
        return DispatchCaretCommand(context, builtIn);
      }

      if (command == TextCommands.DeletePreviousCharacter)
      {
        if (!cursor.MoveBackward())
        {
          var block = ((TextBlock)context.Caret.BlockCursor.Block);
          var previous = block.PreviousBlock;

          if (previous == null)
          {
            // TODO handle parent
            return false;
          }

          // get the end of the previous block, as that's where the caret should end up after the merge. 
          var previousBlockCaret = previous.GetCursor();
          previousBlockCaret.MoveToEnd();

          bool wasMerged = block.Parent.MergeWithPrevious(block);

          if (wasMerged)
          {
            context.Caret.MoveTo(previousBlockCaret);
          }

          return wasMerged;
        }

        if (cursor.DeleteText(1))
          return true;

      }
      else if (command == TextCommands.DeleteNextCharacter)
      {
        cursor.DeleteText(1);
        // TODO what happens if we can't here
        return true;
      }

      return false;
    }

    private bool DispatchCaretCommand(DocumentEditorContext context, BuiltInCaretNavigationCommand builtIn)
    {
      switch (builtIn.Mode)
      {
        case BuiltInCaretNavigationCommand.NavigationType.Forward:
        {
          return context.Cursor.MoveForward();
        }
        case BuiltInCaretNavigationCommand.NavigationType.Backward:
        {
          return context.Cursor.MoveBackward();
        }
        case BuiltInCaretNavigationCommand.NavigationType.NextWord:
        {
          return MoveCaretToBeginningOfNextWord(context);
        }
        case BuiltInCaretNavigationCommand.NavigationType.PreviousWord:
        {
          return MoveCaretToEndOfPreviousWord(context);
        }
        case BuiltInCaretNavigationCommand.NavigationType.Up:
        {
          return TextBlockCursorMover.BackwardMover.MoveCaretTowardsPositionInNextLine(context.Cursor,
                                                                                       context.CaretMovementMode);
        }
        case BuiltInCaretNavigationCommand.NavigationType.Down:
        {
          return TextBlockCursorMover.ForwardMover.MoveCaretTowardsPositionInNextLine(context.Cursor,
                                                                                      context.CaretMovementMode);
        }

        case BuiltInCaretNavigationCommand.NavigationType.Home:
        {
          return TextBlockCursorMover.BackwardMover.MoveCaretTowardsLineEdge(context.Cursor);
        }
        case BuiltInCaretNavigationCommand.NavigationType.End:
        {
          return TextBlockCursorMover.ForwardMover.MoveCaretTowardsLineEdge(context.Cursor);
        }
        case BuiltInCaretNavigationCommand.NavigationType.BeginningOfBlock:
        {
          context.Cursor.MoveToBeginning();
          return true;
        }
        case BuiltInCaretNavigationCommand.NavigationType.EndOfBlock:
        {
          context.Cursor.MoveToEnd();
          return true;
        }
        default:
          throw new ArgumentOutOfRangeException();
      }
    }

    /// <summary> Moves the caret to the beginning of the next word. </summary>
    /// <param name="context"> The context's whose caret should be moved. </param>
    private static bool MoveCaretToBeginningOfNextWord(DocumentEditorContext context)
    {
      var blockCaret = context.Cursor;

      // we either don't know what kind of block cursor it is, or we want to move
      // to the next block anyways. 
      if (blockCaret.IsAtEnd)
      {
        return false;
      }

      var textCursor = (TextBlockCursor)blockCaret;

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

      return true;
    }

    /// <summary> Moves the caret to the end of the previous word. </summary>
    /// <param name="context"> The context's whose caret should be moved. </param>
    private static bool MoveCaretToEndOfPreviousWord(DocumentEditorContext context)
    {
      var blockCaret = context.Cursor;

      // we either don't know what kind of block cursor it is, or we want to move
      // to the next block anyways. 
      if (blockCaret.IsAtBeginning)
        return false;

      var textCursor = (TextBlockCursor)blockCaret;

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

      return true;
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