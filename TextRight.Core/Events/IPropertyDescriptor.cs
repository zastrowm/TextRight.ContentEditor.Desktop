using System;
using System.Collections.Generic;
using System.Linq;

namespace TextRight.Core.Events
{
  /// <summary>
  ///  A description of a property for a specific class, along with a getter/setter to get the
  ///  value from the given class.
  /// </summary>
  public interface IPropertyDescriptor<T> : IPropertyDescriptor
  {
    /// <summary> Sets the property's value. </summary>
    void SetValue(object instance, T value);

    /// <summary> Retrieves the property's value. </summary>
    T GetValue(object instance);
  }

  /// <summary> A description of a property for a specific class. </summary>
  public interface IPropertyDescriptor
  {
    string Id { get; }

    Type DataType { get; }

    void Serialize(object instance, IPropertyWriter writer);

    bool Deserialize(object instance, IPropertyReader reader);
  }
}