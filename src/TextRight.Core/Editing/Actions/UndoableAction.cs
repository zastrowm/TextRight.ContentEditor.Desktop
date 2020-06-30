using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TextRight.Core.Commands;

namespace TextRight.Core.Actions
{
  /// <summary>
  ///  An action that can be executed and then undone if required. Acts on a snapshot of a document
  ///  at a specific point in time.
  /// </summary>
  /// <remarks>
  ///  <see cref="IContextualCommand"/> is something that can be executed on many parts of a
  ///  document while <see cref="UndoableAction"/> is tied to the specific part of the document at
  ///  the time of it's creation.
  /// </remarks>
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

    /// <summary> Attempts to merge the given action into this instance. </summary>
    /// <param name="context"> The context in which the action should be merged. </param>
    /// <param name="action"> The action to merge, if possible. </param>
    /// <returns> True if the action was merged, false otherwise. </returns>
    public virtual bool TryMerge(DocumentEditorContext context, UndoableAction action)
    {
      return false;
    }
  }
}