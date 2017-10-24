using System;
using System.Collections.Generic;
using System.Linq;

namespace TextRight.Core.ObjectModel
{
  /// <summary> Event listener that listens for the actual arguments. </summary>
  public interface IGeneralEventListener : IEventListener
  {
    /// <summary>
    ///  Invoked when a <see cref="EventEmitter"/> fires the given event to all listeners.
    /// </summary>
    /// <param name="sender"> The sender of the event. </param>
    /// <param name="args"> The information associated with the event. </param>
    void Notify(object sender, EventEmitterArgs args);
  }
}