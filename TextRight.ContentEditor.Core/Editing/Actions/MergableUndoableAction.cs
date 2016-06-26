using System;
using System.Collections.Generic;
using System.Linq;

namespace TextRight.ContentEditor.Core.Editing.Actions
{
  /// <summary>
  ///  An UndoableAction that may be able to be merged with another action to reduce the number of
  ///  actions a user needs to undo.
  /// </summary>
  public abstract class MergableUndoableAction
  {
    /// <summary> Check if this instance can have the given action merged into it. </summary>
    /// <param name="action"> The action that may be able to be merged. </param>
    /// <returns> True if it can be merged, false if it cannot. </returns>
    public abstract bool CanMerge(UndoableAction action);

    /// <summary> Merges the given action into this instance. </summary>
    /// <param name="action"> The action to merge into this instance. </param>
    public abstract void Merge(UndoableAction action);

    /// <summary> Attempts to merge the given action into this instance. </summary>
    /// <param name="action"> The action to merge, if possible. </param>
    /// <returns> True if the action was merged, false otherwise. </returns>
    public bool TryMerge(UndoableAction action)
    {
      if (CanMerge(action))
      {
        Merge(action);
        return true;
      }

      return false;
    }
  }
}