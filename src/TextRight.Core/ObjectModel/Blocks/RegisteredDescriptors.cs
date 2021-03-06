using System;
using System.Collections.Generic;
using System.Linq;

namespace TextRight.Core.ObjectModel.Blocks
{
  /// <summary>
  ///  Contains a handle to a <see cref="BlockDescriptor"/> that has been registered app-
  ///  domain wide.
  /// </summary>
  public sealed class RegisteredDescriptors
  {
    /// <summary> All handle types that have been registered in this AppDomain. </summary>
    private static readonly Dictionary<Type, BlockDescriptor> Descriptors
      = new Dictionary<Type, BlockDescriptor>();

    /// <summary> Registers a new ContentBlockDescriptor type. </summary>
    /// <typeparam name="T"> Generic type parameter. </typeparam>
    /// <returns> A DescriptorHandle. </returns>
    public static T Register<T>()
      where T : BlockDescriptor, new()
    {
      if (Descriptors.TryGetValue(typeof(T), out var value))
        return (T)value;

      var instance = new T();
      Descriptors.Add(typeof(T), instance);
      return instance;
    }
  }
}