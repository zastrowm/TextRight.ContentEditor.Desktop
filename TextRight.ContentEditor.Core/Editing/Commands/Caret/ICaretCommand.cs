using System;
using System.Collections.Generic;
using System.Linq;

namespace TextRight.ContentEditor.Core.Editing.Commands
{
  /// <summary> A command which affects the Caret. </summary>
  public interface ICaretCommand
  {
    /// <summary>
    ///  True if <see cref="DocumentEditorContext.CaretMovementMode"/> should not
    ///  have <see cref="CaretMovementMode.SetModeToNone"/> after the command is
    ///  executed.
    /// </summary>
    bool ShouldPreserveCaretState { get; }
  }
}