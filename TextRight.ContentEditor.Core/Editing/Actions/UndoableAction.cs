using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TextRight.ContentEditor.Core.Editing.Actions
{
  /// <summary> Interface for action that can be undone. </summary>
  public abstract class UndoableAction
  {
    /// <summary> A human-readable name of the action. </summary>
    public abstract string Name { get; }

    /// <summary> A human-readable description of the action. </summary>
    public abstract string Description { get; }

    /// <summary> Performs the given operation. </summary>
    /// <param name="context"> The context in which the action should be performed. </param>
    public abstract void Do(DocumentEditorContext context);

    /// <summary> Undoes any effects that occurred from calling <see cref="M:IUndoableAction.Do(???)" />. </summary>
    /// <param name="context"> The context in which the action should be performed. </param>
    public abstract void Undo(DocumentEditorContext context);
  }
}