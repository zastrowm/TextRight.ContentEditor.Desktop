using System;
using System.Collections.Generic;
using System.Linq;

namespace TextRight.Core.Actions
{
  /// <summary> Interface for <see cref="ActionStack"/> to facilitate testing. </summary>
  public interface IActionStack
  {
    /// <summary> Performs the given action and adds it to the undoable stack. </summary>
    /// <param name="undoableAction"> The undoable action. </param>
    void Do(UndoableAction undoableAction);
  }
}