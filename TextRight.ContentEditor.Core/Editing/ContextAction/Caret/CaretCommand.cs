using System;
using System.Collections.Generic;
using System.Linq;
using TextRight.ContentEditor.Core.Editing.Actions;
using TextRight.ContentEditor.Core.ObjectModel.Cursors;

namespace TextRight.ContentEditor.Core.Editing
{
  /// <summary> A command which modifies a caret. </summary>
  public abstract class CaretCommand : IContextualCommand
  {
    /// <summary>
    ///  True if the CaretMovementMode should not be reset after the command is executed.
    /// </summary>
    protected virtual bool ShouldPreserveCaretMovementMode
      => false;

    /// <summary> Activates the given command, acting on the given caret. </summary>
    /// <param name="caret"> The caret on which to act. </param>
    /// <param name="movementMode"></param>
    /// <returns> true if it succeeds, false if it fails. </returns>
    public abstract bool Activate(DocumentCursor caret, CaretMovementMode movementMode);

    /// <inheritdoc/>
    public virtual string GetName(DocumentEditorContext context)
      => null;

    /// <inheritdoc/>
    public virtual string GetDescription(DocumentEditorContext context)
      => null;

    /// <inheritdoc/>
    public virtual bool CanActivate(DocumentEditorContext context)
      => true;

    /// <inheritdoc/>
    public virtual void Activate(DocumentEditorContext context, ActionStack actionStack)
    {
      Activate(context.Caret, context.CaretMovementMode);
      if (!ShouldPreserveCaretMovementMode)
      {
        context.CaretMovementMode.SetModeToNone();
      }
    }
  }
}