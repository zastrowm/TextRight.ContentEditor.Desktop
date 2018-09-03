using System;
using System.Collections.Generic;
using System.Linq;

namespace TextRight.Core.Events
{
  /// <summary> An event that indicates a property value has changed. </summary>
  public abstract class PropertyChangedEvent : ChangeEvent
  {
    /// <summary> The descriptor to the property that changed. </summary>
    public abstract IPropertyDescriptor UntypedDescriptor { get; }
  }
}