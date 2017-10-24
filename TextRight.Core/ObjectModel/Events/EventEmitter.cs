using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;

namespace TextRight.Core.ObjectModel
{
  /// <summary>
  ///  A class that emits events that <see cref="IEventListener"/>s can listen to.
  /// </summary>
  public abstract class EventEmitter : IEventEmitter
  {
    private object _listeners;

    private (IEventListener single, List<IEventListener> listeners) GetCurrentListeners() 
      => (_listeners as IEventListener, _listeners as List<IEventListener>);

    /// <summary> True if any listeners are present. </summary>
    public bool HasListeners
      => _listeners != null;

    /// <summary> The object that this instance is a child of.  Can be null. </summary>
    [CanBeNull]
    protected abstract EventEmitter ParentEmitter { get; }

    /// <summary> Raises an event with the given event arguments. </summary>
    /// <param name="args"> The event args that indicate the event that is fired. </param>
    /// <returns>
    ///  True if an event listener was notified, false if no event listeners were present.
    /// </returns>
    public bool FireEvent(EventEmitterArgs args)
      => FireEvent(this, args);

    /// <summary> Fire the given event to all listeners. </summary>
    protected bool FireEvent(IEventEmitter sender, EventEmitterArgs args)
    {
      return FireEventSelf(sender, args)
             || (ParentEmitter?.FireEvent(args) ?? false);
    }

    /// <summary> Fire the given event to all listeners that are directly on this instance. </summary>
    private bool FireEventSelf(IEventEmitter sender, EventEmitterArgs args)
    {
      if (_listeners == null)
        return false;

      var (existingListener, existingListeners) = GetCurrentListeners();
      if (existingListener != null)
      {
        return args.TryHandleOrGeneral(sender, existingListener);
      }
      else if (existingListeners != null)
      {
        bool anyHandled = false;

        foreach (var listener in existingListeners)
        {
          anyHandled |= args.TryHandleOrGeneral(sender, listener);
        }

        return anyHandled;
      }

      return false;
    }

    /// <summary> Adds a new <see cref="IEventListener"/> to listen to events. </summary>
    /// <param name="eventListener"> The event listener to add. </param>
    public void SubscribeListener(IEventListener eventListener)
    {
      if (_listeners == null)
      {
        _listeners = eventListener;
        return;
      }

      var (existingListener, existingListeners) = GetCurrentListeners();

      if (existingListeners != null)
      {
        existingListeners.Add(eventListener);
      }
      else if (existingListener != null)
      {
        var listeners = new List<IEventListener>
                        {
                          existingListener,
                          eventListener
                        };
        _listeners = listeners;
      }
    }

    /// <summary> Removes an <see cref="IEventListener"/> from listening to events. </summary>
    /// <param name="eventListener"> The event listener to remove. </param>
    /// <returns> True if the listener was removed, false if it was never listening to this object. </returns>
    public bool UnsubscribeListener(IEventListener eventListener)
    {
      var (existingListener, existingListeners) = GetCurrentListeners();

      if (existingListener != null)
      {
        if (eventListener != existingListener)
          return false;

        _listeners = null;
        return true;
      }
      else if (existingListeners != null)
      {
        bool wasPresent = existingListeners.Remove(eventListener);
        if (wasPresent && existingListeners.Count == 1)
        {
          _listeners = existingListeners[0];
        }

        return wasPresent;
      }

      return false;
    }
  }
}