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

      if (command == TextCommands.BreakBlock)
      {
        return BreakBlocks(block, context, command, commandContext);
      }

      var navigationCommand = command as BuiltInCaretNavigationCommand;
      if (navigationCommand == null)
        return false;

      return DispatchCaretCommand(context, navigationCommand, commandContext);
    }

    /// <summary> Break the blocks at the given caret location. </summary>
    private bool BreakBlocks(BlockCollection collection,
                             DocumentEditorContext context,
                             EditorCommand command,
                             CommandExecutionContext commandContext)
    {
      var newBlock = collection.TryBreakBlock(context.Cursor);
      if (newBlock == null)
        return false;

      var cursor = newBlock.GetCursor();
      cursor.MoveToBeginning();
      context.Caret.MoveTo(cursor);
      return true;
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
          return NavigateForward(context, commandContext);
        case BuiltInCaretNavigationCommand.NavigationType.Backward:
          return NavigateBackward(context, commandContext);
        case BuiltInCaretNavigationCommand.NavigationType.Up:
          return NavigateUp(context, commandContext);
        case BuiltInCaretNavigationCommand.NavigationType.Down:
          return NavigateDown(context, commandContext);
        case BuiltInCaretNavigationCommand.NavigationType.NextWord:
          return NavigateNextWord(context, commandContext);
        case BuiltInCaretNavigationCommand.NavigationType.PreviousWord:
          return NavigatePreviousWord(context, commandContext);
      }

      return false;
    }

    /// <summary> Navigate to the beginning of the next paragraph. </summary>
    private bool NavigateNextWord(DocumentEditorContext context, CommandExecutionContext commandContext)
    {
      return NavigateForward(context, commandContext);
    }

    /// <summary> Navigate to the back of the next paragraph. </summary>
    private bool NavigatePreviousWord(DocumentEditorContext context, CommandExecutionContext commandContext)
    {
      return NavigateBackward(context, commandContext);
    }

    /// <summary> Navigate to the back of the next paragraph. </summary>
    private bool NavigateBackward(DocumentEditorContext context, CommandExecutionContext commandContext)
    {
      var currentChildBlock = commandContext.GetChildBlock(1);
      if (currentChildBlock == null || currentChildBlock.IsFirst)
        return false;

      var previousBlock = currentChildBlock.GetPreviousBlock();
      var newCursor = previousBlock.GetCursor();
      newCursor.MoveToEnd();

      context.Caret.MoveTo(newCursor);

      return true;
    }

    /// <summary> Navigate to the beginning of the next paragraph. </summary>
    private bool NavigateForward(DocumentEditorContext context, CommandExecutionContext commandContext)
    {
      var currentChildBlock = commandContext.GetChildBlock(1);
      if (currentChildBlock == null || currentChildBlock.IsLast)
        return false;

      var nextBlock = currentChildBlock.GetNextBlock();

      var newCursor = nextBlock.GetCursor();
      newCursor.MoveToBeginning();

      context.Caret.MoveTo(newCursor);
      return true;
    }

    /// <summary> Navigate up between two paragraphs. </summary>
    private bool NavigateUp(DocumentEditorContext context, CommandExecutionContext commandContext)
    {
      var currentChildBlock = commandContext.GetChildBlock(1);
      if (currentChildBlock == null || currentChildBlock.IsFirst)
        return false;

      var previousBlock = currentChildBlock.GetPreviousBlock();

      var newCaret = previousBlock.GetCaretFromBottom(context.CaretMovementMode);
      context.Caret.MoveTo(newCaret);

      return true;
    }

    /// <summary> Navigate down between two paragraphs. </summary>
    private bool NavigateDown(DocumentEditorContext context, CommandExecutionContext commandContext)
    {
      var currentChildBlock = commandContext.GetChildBlock(1);
      if (currentChildBlock == null || currentChildBlock.IsLast)
        return false;

      var nextBlock = currentChildBlock.GetNextBlock();

      var newCaret = nextBlock.GetCaretFromTop(context.CaretMovementMode);
      context.Caret.MoveTo(newCaret);

      return true;
    }
  }
}