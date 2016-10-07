using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TextRight.Core.Editing.Actions;

namespace TextRight.Core.Editing.Commands
{
  /// <summary> Undoes the previous action. </summary>
  public class UndoCommand : IContextualCommand
  {
    /// <inheritdoc />
    public string Id
      => "undo";

    /// <inheritdoc />
    string IContextualCommand.GetName(DocumentEditorContext context)
    {
      return "Undo";
    }

    /// <inheritdoc/>
    string IContextualCommand.GetDescription(DocumentEditorContext context)
    {
      return "Undoes the previous action";
    }

    /// <inheritdoc/>
    bool IContextualCommand.CanActivate(DocumentEditorContext context)
    {
      // TODO
      return true;
    }

    /// <inheritdoc/>
    void IContextualCommand.Activate(DocumentEditorContext context, ActionStack actionStack)
    {
      context.UndoStack.Undo();
    }
  }
}