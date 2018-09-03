using System;
using System.Collections.Generic;
using System.Linq;

namespace TextRight.Core.Events
{
  /// <summary> An object whose interested in events that are fired. </summary>
  public interface IChangeListener
  {
    /// <summary> Handles the given event. </summary>
    void HandleEvent(ChangeEvent changeEvent);
  }
}