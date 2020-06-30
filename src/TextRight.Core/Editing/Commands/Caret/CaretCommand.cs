using System;
using System.Collections.Generic;
using System.Linq;
using TextRight.Core.Actions;
using TextRight.Core.ObjectModel.Cursors;

namespace TextRight.Core.Commands.Caret
{
  /// <summary> A command which modifies a caret. </summary>
  public abstract class CaretCommand : IContextualCommand
  {
    /// <summary>
    ///  True if the CaretMovementMode should not be reset after the command is executed.
    /// </summary>
    protected virtual bool ShouldPreserveCaretMovementMode
      => false;

    /// <inheritdoc />
    public abstract string Id { get; }

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
    public virtual void Activate(DocumentEditorContext context, IActionStack actionStack)
    {
      Activate(context.Selection,
               context.CaretMovementMode,
               context.IsSelectionExtendActive ? SelectionMode.Extend : SelectionMode.Replace);
      
      if (!ShouldPreserveCaretMovementMode)
      {
        context.CaretMovementMode.SetModeToNone();
      }
    }

    /// <summary> Activates the given command, acting on the given caret. </summary>
    /// <param name="cursor"> The caret on which to act. </param>
    /// <param name="movementMode"> The current caret mode. </param>
    /// <param name="mode"></param>
    /// <param name="shouldExtend"> How the caret should be moved. </returns>
    public abstract bool Activate(DocumentSelection cursor, CaretMovementMode movementMode, SelectionMode mode);
  }
}