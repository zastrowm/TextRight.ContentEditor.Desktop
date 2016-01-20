using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace TextRight.ContentEditor.Core.Editing.Commands
{
  /// <summary> An object that attempts to process commands. </summary>
  public interface ICommandProcessor
  {
    /// <summary>
    ///  Checks to see if the given command can be processed and if so, processes
    ///  it.
    /// </summary>
    /// <param name="context"> The context in which the command is being
    ///   processed. </param>
    /// <param name="command"> The command that should be processed. </param>
    /// <param name="hookOwner"> The hook on which the processor was retrieved. </param>
    /// <returns> True if the command has been handled, false otherwise. </returns>
    bool TryProcess([NotNull] DocumentEditorContext context,
                    [NotNull] EditorCommand command,
                    [NotNull] CommandExecutionContext hookOwner);
  }
}