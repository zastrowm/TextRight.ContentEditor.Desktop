using System;
using System.Collections.Generic;
using System.Linq;
using TextRight.ContentEditor.Core.ObjectModel;

namespace TextRight.ContentEditor.Desktop.Commands
{
  /// <summary> Interface for action command. </summary>
  public interface IActionCommand
  {
    /// <summary> The unique id of the command. </summary>
    string Id { get; }

    /// <summary> Executes the action on the specified context. </summary>
    /// <param name="context"> The context on which the command should be executed. </param>
    void Execute(DocumentEditorContext context);
  }
}