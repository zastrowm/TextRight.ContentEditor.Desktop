using System;
using System.Collections.Generic;
using System.Linq;

namespace TextRight.ContentEditor.Core.Editing.Commands
{
  /// <summary> A command that moves the caret. </summary>
  public abstract class CaretNavigationCommand : EditorCommand
  {
    protected CaretNavigationCommand(string id)
      : base(id)
    {
    }
  }
}