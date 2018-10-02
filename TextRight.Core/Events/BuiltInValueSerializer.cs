using System;
using System.Collections.Generic;
using System.Linq;

namespace TextRight.Core.Events
{
  /// <summary>
  ///  A complex serializer that can serialize/deserialize all of the types for the parameter types on
  ///  <see cref="IValueSerializer{T}.Write"/>
  /// </summary>
  internal class BuiltInValueSerializer : IValueSerializer<int>,
                                          IValueSerializer<long>,
                                          IValueSerializer<double>,
                                          IValueSerializer<bool>,
                                          IValueSerializer<string>,
                                          IValueSerializer<byte[]>
  {
    public void AddTo(ValueSerializerCollection serializerCollection)
    {
      serializerCollection.Add<int>(this);
      serializerCollection.Add<long>(this);
      serializerCollection.Add<double>(this);
      serializerCollection.Add<bool>(this);
      serializerCollection.Add<string>(this);
      serializerCollection.Add<int>(this);
      serializerCollection.Add<byte[]>(this);
    }

    /// <inheritdoc />
    bool IValueSerializer<int>.TryRead(IPropertyReader reader, IPropertyDescriptor descriptor, out int value)
      => reader.TryRead(descriptor, out value);

    /// <inheritdoc />
    void IValueSerializer<int>.Write(IPropertyWriter writer, IPropertyDescriptor descriptor, int value)
      => writer.Write(descriptor, value);

    /// <inheritdoc />
    bool IValueSerializer<long>.TryRead(IPropertyReader reader, IPropertyDescriptor descriptor, out long value)
      => reader.TryRead(descriptor, out value);

    /// <inheritdoc />
    void IValueSerializer<long>.Write(IPropertyWriter writer, IPropertyDescriptor descriptor, long value)
      => writer.Write(descriptor, value);

    /// <inheritdoc />
    bool IValueSerializer<double>.TryRead(IPropertyReader reader, IPropertyDescriptor descriptor, out double value)
      => reader.TryRead(descriptor, out value);

    /// <inheritdoc />
    void IValueSerializer<double>.Write(IPropertyWriter writer, IPropertyDescriptor descriptor, double value)
      => writer.Write(descriptor, value);

    /// <inheritdoc />
    bool IValueSerializer<bool>.TryRead(IPropertyReader reader, IPropertyDescriptor descriptor, out bool value)
      => reader.TryRead(descriptor, out value);

    /// <inheritdoc />
    void IValueSerializer<bool>.Write(IPropertyWriter writer, IPropertyDescriptor descriptor, bool value)
      => writer.Write(descriptor, value);

    /// <inheritdoc />
    bool IValueSerializer<string>.TryRead(IPropertyReader reader, IPropertyDescriptor descriptor, out string value)
      => reader.TryRead(descriptor, out value);

    /// <inheritdoc />
    void IValueSerializer<string>.Write(IPropertyWriter writer, IPropertyDescriptor descriptor, string value)
      => writer.Write(descriptor, value);

    /// <inheritdoc />
    bool IValueSerializer<byte[]>.TryRead(IPropertyReader reader, IPropertyDescriptor descriptor, out byte[] value)
      => reader.TryRead(descriptor, out value);

    /// <inheritdoc />
    void IValueSerializer<byte[]>.Write(IPropertyWriter writer, IPropertyDescriptor descriptor, byte[] value)
      => writer.Write(descriptor, value);
  }
}