using System;
using System.Collections.Generic;
using System.Linq;
using TextRight.Core.Editing.Actions;

namespace TextRight.Core.Editing.Commands
{
  /// <summary> Redos a previously undone action. </summary>
  public class RedoCommand : IContextualCommand
  {
    /// <inheritdoc />
    public string Id
      => "redo";

    /// <inheritdoc />
    string IContextualCommand.GetName(DocumentEditorContext context)
    {
      return "Redo";
    }

    /// <inheritdoc/>
    string IContextualCommand.GetDescription(DocumentEditorContext context)
    {
      return "Redos the previous undone action";
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
      context.UndoStack.Redo();
    }
  }
}