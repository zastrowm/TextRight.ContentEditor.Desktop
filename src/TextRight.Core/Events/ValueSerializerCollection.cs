using System;
using System.Collections.Generic;
using System.Linq;

namespace TextRight.Core.Events
{
  /// <summary> Contains a collection of value serializers. </summary>
  public class ValueSerializerCollection
  {
    private readonly Dictionary<Type, object> _valueSerializers
      = new Dictionary<Type, object>();

    public ValueSerializerCollection()
    {
      RemoveAllSerializers();
    }

    /// <summary> Adds a new serializer that can serialize the given type. </summary>
    public void Add<T>(IValueSerializer<T> serializer)
    {
      lock (_valueSerializers)
      {
        _valueSerializers[typeof(T)] = serializer;
      }
    }

    public void RemoveAllSerializers()
    {
      lock (_valueSerializers)
      {
        _valueSerializers.Clear();

        var builtInSerializer = new BuiltInValueSerializer();
        builtInSerializer.AddTo(this);
      }
    }

    /// <summary> Attempts to find a serializer for the given type. </summary>
    public bool TryGetSerializer<T>(out IValueSerializer<T> serializer)
    {
      lock (_valueSerializers)
      {
        if (_valueSerializers.TryGetValue(typeof(T), out var untyped))
        {
          serializer = (IValueSerializer<T>)untyped;
          return true;
        }
        else
        {
          serializer = null;
          return false;
        }
      }
    }
  }
}