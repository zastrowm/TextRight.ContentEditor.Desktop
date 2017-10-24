using System;
using System.Collections.Generic;
using System.Linq;

namespace TextRight.Core.ObjectModel
{
  /// <summary>
  ///  An object that emits events that <see cref="IEventListener"/>s can listen to.
  /// </summary>
  public interface IEventEmitter
  {
    /// <summary> True if any listeners are present. </summary>
    bool HasListeners { get; }

    /// <summary> Raises an event with the given event arguments. </summary>
    /// <param name="args"> The event args that indicate the event that is fired. </param>
    /// <returns>
    ///  True if an event listener was notified, false if no event listeners were present.
    /// </returns>
    bool FireEvent(EventEmitterArgs args);

    /// <summary> Adds a new <see cref="IEventListener"/> to listen to events. </summary>
    /// <param name="eventListener"> The event listener to add. </param>
    void SubscribeListener(IEventListener eventListener);

    /// <summary> Removes an <see cref="IEventListener"/> from listening to events. </summary>
    /// <param name="eventListener"> The event listener to remove. </param>
    /// <returns> True if the listener was removed, false if it was never listening to this object. </returns>
    bool UnsubscribeListener(IEventListener eventListener);
  }
}