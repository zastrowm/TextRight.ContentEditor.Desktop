using System;
using System.Collections.Generic;
using System.Linq;

namespace TextRight.ContentEditor.Core.Editing.Actions
{
  /// <summary> Determines when undo stack merging should be attempted. </summary>
  public interface IActionStackMergePolicy
  {
    /// <summary>
    ///  Predicate to determine if the two entries should be attempted to be merged.
    /// </summary>
    /// <param name="originalEntry"> The original entry that has already been done. </param>
    /// <param name="newEntry"> The entry that is about to be added to the undo stack. </param>
    /// <returns>
    ///  True if the merge should be attempted and originalEntry.Action.TryMerge(newEntry.Action)
    ///  should be invoked, false otherwise.
    /// </returns>
    bool ShouldTryMerge(ActionStack.UndoStackEntry originalEntry, ActionStack.UndoStackEntry newEntry);
  }
}