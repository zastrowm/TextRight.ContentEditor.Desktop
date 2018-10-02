using System;
using System.Collections.Generic;
using System.Linq;

namespace TextRight.Core.Events
{
  /// <summary>
  ///  A strongly typed event that indicates that a property value has changed, along with the old
  ///  and new value.
  /// </summary>
  /// <typeparam name="T"> Generic type parameter. </typeparam>
  public class PropertyChangedEvent<T> : PropertyChangedEvent
  {
    public PropertyChangedEvent(object sender, IPropertyDescriptor<T> descriptor, T oldValue, T newValue)
    {
      Descriptor = descriptor;
      OldValue = oldValue;
      NewValue = newValue;
      Sender = sender;
    }

    /// <inheritdoc/>
    public override object Sender { get; }

    /// <summary> The property whose value changed. </summary>
    public IPropertyDescriptor<T> Descriptor { get; }

    /// <inheritdoc/>
    public override IPropertyDescriptor UntypedDescriptor
      => Descriptor;

    /// <summary> The old value of the property. </summary>
    public T OldValue { get; }

    /// <summary> The new value of the property. </summary>
    public T NewValue { get; }
  }

  /// <summary> An event that indicates a property value has changed. </summary>
  public abstract class PropertyChangedEvent : ChangeEvent
  {
    /// <summary> The descriptor to the property that changed. </summary>
    public abstract IPropertyDescriptor UntypedDescriptor { get; }
  }
}