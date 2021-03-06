﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TextRight.Core.Actions;

namespace TextRight.Core.Commands
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
    void IContextualCommand.Activate(DocumentEditorContext context, IActionStack actionStack)
    {
      context.UndoStack.Undo();
    }
  }
}