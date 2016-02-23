using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using TextRight.ContentEditor.Core.Editing.Commands;
using TextRight.ContentEditor.Core.ObjectModel.Blocks;

namespace TextRight.ContentEditor.Core.Editing
{
  /// <summary>
  ///  Provides the implementation of executing commands for a
  ///  DocumentEditorContext.
  /// </summary>
  public class CommandPipeline
  {
    private readonly DocumentEditorContext _owner;

    /// <summary> Constructor. </summary>
    /// <param name="owner"> The owner of the command pipeline. </param>
    public CommandPipeline(DocumentEditorContext owner)
    {
      if (owner == null)
        throw new ArgumentNullException(nameof(owner));

      _owner = owner;
      PipelineHooks = new List<ICommandProcessorPipelineHook>()
                      {
                        BuiltInCaretNavigationCommand.PipelineHook,
                      };
    }

    /// <summary> All of the pre and post execution hooks. </summary>
    public List<ICommandProcessorPipelineHook> PipelineHooks { get; set; }

    /// <summary>
    ///  Executes the given command starting at the caret and bubbling up until
    ///  something processes the command.
    /// </summary>
    /// <param name="command"> The command to execute. </param>
    /// <returns> True if the command was handled, false if it was not. </returns>
    public bool Execute(EditorCommand command)
    {
      PreProcessCommand(command);

      var wasProcessed = ExecuteCommandDirect(command);

      if (wasProcessed)
      {
        PostProcessCommand(command);
      }

      return wasProcessed;
    }

    /// <summary> Actually attempts to execute the given commands. </summary>
    private bool ExecuteCommandDirect(EditorCommand command)
    {
      // TODO put this somewhere better
      if (command == TextCommands.BreakBlock)
      {
        _owner.BreakCurrentBlock();
        return true;
      }

      var context = new CommandExecutionContext();
      context.ConfigureFor(_owner.Cursor);

      // first check if the caret itself handles the command
      if (TryHandleCommand(command, context, _owner.Cursor as ICommandProcessorHook))
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
    ///  Performs any additional processing that needs to be peformed based on the
    ///  type of the command.
    /// </summary>
    private void PreProcessCommand(EditorCommand command)
    {
      foreach (var hook in PipelineHooks)
      {
        hook.PreProcess(command, _owner);
      }
    }

    /// <summary>
    ///  Performs any additional processing that needs to be peformed based on the
    ///  type of the command.
    /// </summary>
    private void PostProcessCommand(EditorCommand command)
    {
      foreach (var hook in PipelineHooks)
      {
        hook.PostProcess(command, _owner);
      }
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
      return processorHook?.CommandProcessor?.TryProcess(_owner, command, context) == true;
    }
  }
}