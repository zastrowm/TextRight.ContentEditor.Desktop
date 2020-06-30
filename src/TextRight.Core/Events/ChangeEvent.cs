using System;
using System.Collections.Generic;
using System.Linq;

namespace TextRight.Core.Events
{
  /// <summary> Represents a change that occurred to an element. </summary>
  public abstract class ChangeEvent
  {
    /// <summary> The object that sent the message. </summary>
    public abstract object Sender { get; }
  }
}