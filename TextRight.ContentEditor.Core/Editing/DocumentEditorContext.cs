using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using TextRight.ContentEditor.Core.Editing.Commands;
using TextRight.ContentEditor.Core.ObjectModel;
using TextRight.ContentEditor.Core.ObjectModel.Blocks;

namespace TextRight.ContentEditor.Core.Editing
{
  /// <summary> Represents a TextRight document that is being edited. </summary>
  public class DocumentEditorContext
  {
    /// <summary> Default constructor. </summary>
    public DocumentEditorContext()
    {
      Document = new DocumentOwner();

      var cursor = Document.Root.FirstBlock.GetCursor();
      cursor.MoveToBeginning();

      Caret = new DocumentCursor(Document, cursor);
      CaretMovementMode = new CaretMovementMode();
    }

    /// <summary> The document that is being edited. </summary>
    public DocumentOwner Document { get; }

    /// <summary> The Caret's current position. </summary>
    public DocumentCursor Caret { get; }

    /// <summary> TODO </summary>
    public IBlockContentCursor Cursor
      => Caret.BlockCursor;

    /// <summary> Movement information about the caret. </summary>
    public CaretMovementMode CaretMovementMode { get; }

    public bool Execute(EditorCommand command)
    {
      // TODO put this somewhere better
      if (command == TextCommands.BreakBlock)
      {
        BreakCurrentBlock();
        return true;
      }

      var context = new CommandExecutionContext();
      context.ConfigureFor(Cursor);

      // first check if the caret itself handles the command
      if (TryHandleCommand(command, context, Cursor as ICommandProcessorHook))
        return true;

      // if that doesn't work, walk up the tree checking to see if any of the
      // blocks up to the top-most block can handle the command. 
      do
      {
        if (TryHandleCommand(command, context, context.CurrentBlock as ICommandProcessorHook))
          return true;
      } while (context.MoveUp());

      return false;
    }

    /// <summary>
    ///  Allows the given processor hook to try and process the given command.
    /// </summary>
    /// <param name="command"> The command to try to handle. </param>
    /// <param name="context"></param>
    /// <param name="processorHook"> The processor hook, which can be null
    ///   (convenient for passing in parameters using "as" cast that may or may not
    ///   implement ICommandProcessorHook). </param>
    /// <returns> True if the command was handled, false otherwise. </returns>
    private bool TryHandleCommand(EditorCommand command,
                                  CommandExecutionContext context,
                                  [CanBeNull] ICommandProcessorHook processorHook)
    {
      return processorHook?.CommandProcessor?.TryProcess(this, command, context) == true;
    }

    /// <summary> Break the current block at the current position. </summary>
    private void BreakCurrentBlock()
    {
      var currentBlock = Cursor.Block;
      var parentBlock = currentBlock.Parent;

      if (parentBlock.CanBreak(Cursor))
      {
        var newBlock = parentBlock.Break(Cursor);
        if (newBlock != null)
        {
          var cursor = newBlock.GetCursor();
          cursor.MoveToBeginning();
          Caret.MoveTo(cursor);
        }
      }
    }
  }
}