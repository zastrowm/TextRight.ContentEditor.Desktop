using System;
using System.Collections.Generic;
using System.Linq;

namespace TextRight.Core.Actions
{
  /// <summary> Will report to merge actions if they occur within the given timespan, defaulting to .75 seconds. </summary>
  public class StandardMergePolicy : IActionStackMergePolicy
  {
    /// <summary>
    ///  The maximum time that can occur between two undo entries before they should no longer be
    ///  merged.
    /// </summary>
    public TimeSpan MaxDifference { get; set; }
    = TimeSpan.FromSeconds(.75);

    /// <inheritdoc/>
    public bool ShouldTryMerge(ActionStack.UndoStackEntry originalEntry, ActionStack.UndoStackEntry newEntry)
    {
      return newEntry.InsertTime - originalEntry.InsertTime < MaxDifference;
    }
  }
}