using System;
using System.Collections.Generic;
using System.Linq;

namespace TextRight.Core.ObjectModel.Blocks
{
  /// <summary>
  ///  Contains a handle to a <see cref="BlockDescriptor"/> that has been registered app-
  ///  domain wide.
  /// </summary>
  public sealed class RegisteredDescriptor
  {
    /// <summary> All handle types that have been registered in this AppDomain. </summary>
    private static readonly Dictionary<Type, RegisteredDescriptor> RegisteredHandles
      = new Dictionary<Type, RegisteredDescriptor>();

    /// <summary> Constructor. </summary>
    /// <param name="descriptor"> The descriptor that is referenced. </param>
    private RegisteredDescriptor(BlockDescriptor descriptor)
    {
      Descriptor = descriptor;
    }

    /// <summary> The descriptor that is registered. </summary>
    public BlockDescriptor Descriptor { get; set; }

    /// <summary> Registers a new ContentBlockDescriptor type. </summary>
    /// <typeparam name="T"> Generic type parameter. </typeparam>
    /// <returns> A DescriptorHandle. </returns>
    public static RegisteredDescriptor Register<T>()
      where T : BlockDescriptor, new()
    {
      var handle = new RegisteredDescriptor(new T());
      RegisteredHandles.Add(typeof(T), handle);
      return handle;
    }
  }
}