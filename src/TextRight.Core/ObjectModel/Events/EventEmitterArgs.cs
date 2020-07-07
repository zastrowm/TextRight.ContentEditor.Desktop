using System;
using System.Collections.Generic;
using System.Linq;

namespace TextRight.Core.ObjectModel
{
  /// <summary> Arguments emitted from <see cref="EventEmitter"/>. </summary>
  public abstract class EventEmitterArgs
  {
    /// <summary> TODO Attempts to invoke the arguments on the given listener. </summary>
    /// <param name="sender"> Source of the event. </param>
    /// <param name="listener"> The listener whose specific-interface should be invoked. </param>
    /// <returns>
    ///  True if the listener was of the correct type and was invoked, false otherwise.
    /// </returns>
    public abstract bool TryHandle(object sender, IEventListener listener);

    internal bool TryHandleOrGeneral(object sender, IEventListener listener)
    {
      bool isHandled = false;

      if (listener is IGeneralEventListener generalListener)
      {
        generalListener.Notify(sender, this);
        isHandled = true;
      }

      isHandled |= TryHandle(sender, listener);
      return isHandled;
    }
  }

  /// <summary> TODO. </summary>
  public abstract class EventEmitterArgs<TReceiver> : EventEmitterArgs
    where TReceiver : class
  {
    protected abstract void Handle(object sender, TReceiver listener);

    /// <inheritdoc />
    public override bool TryHandle(object sender, IEventListener listener)
    {
      if (!(listener is TReceiver typed))
        return false;

      Handle(sender, typed);
      return false;
    }
  }
}