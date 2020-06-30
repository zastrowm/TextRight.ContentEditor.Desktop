using System;
using System.Collections.Generic;
using System.Linq;

namespace TextRight.Core.Events
{
  /// <summary>
  ///  An object that can read a specific type of value to a <see cref="IPropertyReader"/>, and can
  ///  write a specific type of value to <see cref="IPropertyWriter"/>
  /// </summary>
  /// <typeparam name="T"> Generic type parameter. </typeparam>
  public interface IValueSerializer<T>
  {
    bool TryRead(IPropertyReader reader, IPropertyDescriptor descriptor, out T value);
    void Write(IPropertyWriter writer, IPropertyDescriptor descriptor, T value);
  }
}