using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using TextRight.Core.ObjectModel.Serialization;

namespace TextRight.Core.Events
{
  /// <summary> Concrete implementation of <see cref="IPropertyDescriptor{T}"/> </summary>
  /// <typeparam name="T"> Generic type parameter. </typeparam>
  internal class PropertyDescriptor<T> : IPropertyDescriptor<T>
  {
    private readonly Func<object, T> _getter;
    private readonly Action<object, T> _setter;
    private readonly IValueSerializer<T> _serializer;

    public PropertyDescriptor(string id, PropertyInfo property, IValueSerializer<T> serializer)
      : this(id, CreateGetter(property), CreateSetter(property), serializer)
    {
    }

    public PropertyDescriptor(string id, Func<object, T> getter, Action<object, T> setter, IValueSerializer<T> serializer)
    {
      Id = id;
      _getter = getter;
      _setter = setter;
      _serializer = serializer;
    }

    /// <inheritdoc />
    public string Id { get; }

    /// <inheritdoc cref="IPropertyDescriptor.DataType" />
    public Type DataType
      => typeof(T);

    /// <inheritdoc />
    public void Serialize(object instance, IPropertyWriter writer)
    {
      var value = GetValue(instance);
      _serializer.Write(writer, this, value);
    }

    /// <inheritdoc />
    public bool Deserialize(object instance, IPropertyReader reader)
    {
      if (_serializer.TryRead(reader, this, out var value))
      {
        SetValue(instance, value);
        return true;
      }

      return false;
    }

    /// <inheritdoc />
    public void SetValue(object instance, T value)
      => _setter.Invoke(instance, value);

    /// <inheritdoc />
    public T GetValue(object instance)
      => _getter.Invoke(instance);

    /// <summary> Creates a Getter delegate for the given property. </summary>
    private static Func<object, T> CreateGetter(PropertyInfo property)
    {
      var paramInstance = Expression.Parameter(typeof(object));
      var expGet = Expression.Property(
        Expression.Convert(paramInstance, property.DeclaringType),
        property
        );

      return Expression.Lambda<Func<object, T>>(expGet, paramInstance).Compile();
    }

    /// <summary> Creates a Setter 
    ///           delegate for the given property. </summary>
    private static Action<object, T> CreateSetter(PropertyInfo property)
    {
      var paramInstance = Expression.Parameter(typeof(object));
      var paramValue = Expression.Parameter(typeof(T));

      var expAssign = Expression.Assign(
        Expression.Property(
          Expression.Convert(paramInstance, property.DeclaringType),
          property
          ),
        paramValue
        );

      return Expression.Lambda<Action<object, T>>(expAssign, paramInstance, paramValue).Compile();
    }
  }
}