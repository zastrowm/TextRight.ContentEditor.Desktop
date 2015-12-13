using System;
using System.Collections.Generic;
using System.Linq;

namespace TextRight.ContentEditor.Core.Editing.Commands
{
  /// <summary>
  ///  A command that can be executed against a
  ///  <see cref="DocumentEditorContext"/>.
  /// </summary>
  public interface IActionCommand
  {
    /// <summary> Executes the action on the specified context. </summary>
    /// <param name="context"> The context on which the command should be executed. </param>
    void Execute(DocumentEditorContext context);
  }
}