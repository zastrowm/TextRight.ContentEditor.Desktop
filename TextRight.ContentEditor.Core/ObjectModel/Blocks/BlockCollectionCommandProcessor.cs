using System;
using System.Collections.Generic;
using System.Linq;
using TextRight.ContentEditor.Core.Editing;
using TextRight.ContentEditor.Core.Editing.Commands;

namespace TextRight.ContentEditor.Core.ObjectModel.Blocks
{
  /// <summary> Handles commands for TextBlock collections. </summary>
  internal class BlockCollectionCommandProcessor : ICommandProcessor
  {
    public static ICommandProcessor Instance
      = new BlockCollectionCommandProcessor();

    /// <inheritdoc />
    bool ICommandProcessor.TryProcess(DocumentEditorContext context,
                                      EditorCommand command,
                                      CommandExecutionContext commandContext)
    {
      var block = commandContext.CurrentBlock as BlockCollection;
      if (block == null)
        return false;

      var navigationCommand = command as BuiltInCaretNavigationCommand;
      if (navigationCommand == null)
        return false;

      return DispatchCaretCommand(context, navigationCommand, commandContext);
    }

    /// <summary> Dispatch the current caret command to the correct method below. </summary>
    private bool DispatchCaretCommand(DocumentEditorContext context,
                                      BuiltInCaretNavigationCommand builtIn,
                                      CommandExecutionContext commandContext)
    {
      BlockTreeWalker.GetNextNonContainerBlock(context.Cursor.Block);

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
      }

      return false;
    }

    /// <summary> Navigate to the beginning of the next paragraph. </summary>
    private bool NavigateNextWord(DocumentEditorContext context)
    {
      return NavigateForward(context);
    }

    /// <summary> Navigate to the back of the next paragraph. </summary>
    private bool NavigatePreviousWord(DocumentEditorContext context)
    {
      return context.Caret.MoveBackward();
    }

    /// <summary> Navigate to the back of the next paragraph. </summary>
    private bool NavigateBackward(DocumentEditorContext context)
    {
      // TODO move to the end of the previous block
      return context.Caret.MoveBackward();
    }

    /// <summary> Navigate to the beginning of the next paragraph. </summary>
    private bool NavigateForward(DocumentEditorContext context)
    {
      // TODO move to the beginning of the next block
      return context.Caret.MoveForward();
    }

    /// <summary> Navigate up between two paragraphs. </summary>
    private bool NavigateUp(DocumentEditorContext context)
    {
      // TODO this should only look at our own blocks
      bool wasHandled = TextBlockCursorMover.BackwardMover.MoveCaretTowardsPositionInNextLine(context.Cursor,
                                                                                              context.CaretMovementMode);

      if (wasHandled)
        return true;

      var previousBlock = BlockTreeWalker.GetPreviousNonContainerBlock(context.Cursor.Block);
      if (previousBlock == null)
        return false;

      var newCaret = previousBlock.GetCaretFromBottom(context.CaretMovementMode);
      context.Caret.MoveTo(newCaret);

      return true;
    }

    /// <summary> Navigate down between two paragraphs. </summary>
    private bool NavigateDown(DocumentEditorContext context)
    {
      // TODO this should only look at our own blocks
      bool wasHandled = TextBlockCursorMover.ForwardMover.MoveCaretTowardsPositionInNextLine(context.Cursor,
                                                                                             context.CaretMovementMode);

      if (wasHandled)
        return true;

      var nextBlock = BlockTreeWalker.GetNextNonContainerBlock(context.Cursor.Block);
      if (nextBlock == null)
        return false;

      var newCaret = nextBlock.GetCaretFromTop(context.CaretMovementMode);
      context.Caret.MoveTo(newCaret);

      return true;
    }
  }
}