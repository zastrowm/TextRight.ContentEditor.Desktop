using System;
using System.Collections.Generic;
using System.Linq;

namespace TextRight.ContentEditor.Core.Editing.Commands
{
  /// <summary> A navigation simple action command. </summary>
  internal class SimpleCaretActionCommand : DelegateSimpleActionCommand, ICaretCommand
  {
    /// <summary> Constructor. </summary>
    /// <param name="id"> The unique id of the command. </param>
    /// <param name="callback"> The callback. </param>
    /// <param name="shouldPreserveCaretState"> True if
    ///  <see cref="DocumentEditorContext.CaretMovementMode"/> should not have
    ///  <see cref="CaretMovementMode.SetModeToNone"/> after the command is
    ///  executed. </param>
    public SimpleCaretActionCommand(string id, Action<DocumentEditorContext> callback, bool shouldPreserveCaretState)
      : base(id, callback)
    {
      ShouldPreserveCaretState = shouldPreserveCaretState;
    }

    /// <inheritdoc />
    public bool ShouldPreserveCaretState { get; }
  }
}