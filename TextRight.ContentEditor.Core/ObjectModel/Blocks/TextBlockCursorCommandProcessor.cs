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
      var cursor = context.Cursor as TextBlock.TextBlockCursor;
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
          return false;

        cursor.DeleteText(1);
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
          return NavigateForward(context);
        case BuiltInCaretNavigationCommand.NavigationType.Backward:
          return NavigateBackward(context);
        case BuiltInCaretNavigationCommand.NavigationType.Up:
          return NavigateUp(context);
        case BuiltInCaretNavigationCommand.NavigationType.Down:
          return NavigateDown(context);
        case BuiltInCaretNavigationCommand.NavigationType.NextWord:
          return NavigateNextWord(context);
        case BuiltInCaretNavigationCommand.NavigationType.PreviousWord:
          return NavigatePreviousWord(context);
        case BuiltInCaretNavigationCommand.NavigationType.Home:
          return NavigateHome(context);
        case BuiltInCaretNavigationCommand.NavigationType.End:
          return NavigateEnd(context);
        case BuiltInCaretNavigationCommand.NavigationType.BeginningOfBlock:
          return NavigateBeginningofBlock(context);
        case BuiltInCaretNavigationCommand.NavigationType.EndOfBlock:
          return NavigateEndOfBlock(context);
        default:
          throw new ArgumentOutOfRangeException();
      }
    }

    /// <inheritdoc />
    private bool NavigateForward(DocumentEditorContext context)
    {
      return context.Cursor.MoveForward();
    }

    /// <inheritdoc />
    private bool NavigateBackward(DocumentEditorContext context)
    {
      return context.Cursor.MoveBackward();
    }

    /// <inheritdoc />
    private bool NavigateNextWord(DocumentEditorContext context)
    {
      return CaretWordMover.MoveCaretToBeginningOfNextWord(context);
    }

    /// <inheritdoc />
    private bool NavigatePreviousWord(DocumentEditorContext context)
    {
      return CaretWordMover.MoveCaretToEndOfPreviousWord(context);
    }

    /// <inheritdoc />
    private bool NavigateHome(DocumentEditorContext context)
    {
      context.CaretMovementMode.SetModeToHome();
      return TextBlockCursorMover.BackwardMover.MoveCaretTowardsLineEdge(context);
    }

    /// <inheritdoc />
    private bool NavigateEnd(DocumentEditorContext context)
    {
      context.CaretMovementMode.SetModeToEnd();
      return TextBlockCursorMover.ForwardMover.MoveCaretTowardsLineEdge(context);
    }

    /// <inheritdoc />
    private bool NavigateUp(DocumentEditorContext context)
    {
      return TextBlockCursorMover.BackwardMover.MoveCaretTowardsPositionInNextLine(context);
    }

    /// <inheritdoc />
    private bool NavigateDown(DocumentEditorContext context)
    {
      return TextBlockCursorMover.ForwardMover.MoveCaretTowardsPositionInNextLine(context);
    }

    /// <inheritdoc />
    private bool NavigateBeginningofBlock(DocumentEditorContext context)
    {
      context.Cursor.MoveToBeginning();
      return true;
    }

    /// <inheritdoc />
    private bool NavigateEndOfBlock(DocumentEditorContext context)
    {
      context.Cursor.MoveToEnd();
      return true;
    }
  }
}