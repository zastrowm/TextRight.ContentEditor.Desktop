using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace TextRight.Core.Events
{
  /// <summary>
  ///  Contains a collection of properties that have been registered for a given descriptor.  These
  ///  are the properties that contribute to an undo/redo item, and can take part in the
  ///  serialization/deserialization of the block.
  ///  
  ///  See <see cref="IPropertyDescriptor"/> for more information.
  /// </summary>
  public class PropertyDescriptorCollection : IEnumerable<IPropertyDescriptor>
  {
    /// <summary> All of the properties that have been registered thus far. </summary>
    private readonly Dictionary<string, IPropertyDescriptor> _properties
      = new Dictionary<string, IPropertyDescriptor>(StringComparer.CurrentCultureIgnoreCase);

    /// <inheritdoc />
    public IEnumerator<IPropertyDescriptor> GetEnumerator() 
      => _properties.Values.GetEnumerator();

    /// <inheritdoc />
    IEnumerator IEnumerable.GetEnumerator() 
      => GetEnumerator();

    /// <summary> Value serializers available for properties to use. </summary>
    internal ValueSerializerCollection ValueSerializers
      => GlobalRegistration.ValueSerializers;

    internal IPropertyDescriptor<T> RegisterProperty<TClass, T>(Expression<Func<TClass, T>> propertyGetter, string id)
    {
      if (_properties.TryGetValue(id, out var untypedValue))
      {
        if (untypedValue is IPropertyDescriptor<T> descriptor)
          return descriptor;

        throw new ArgumentException($"Property «{id}» already registered as a type «{untypedValue.DataType}», but attempting to re-register as type «{typeof(T)}»");
      }

      if (!ValueSerializers.TryGetSerializer<T>(out var serializer))
      {
        throw new ArgumentException($"Property «{id}» with type «{typeof(T)}», has no available {typeof(IValueSerializer<T>)}");
      }

      var property = GetPropertyInfo(propertyGetter);
      var newDescriptor = new PropertyDescriptor<T>(id, property, serializer);

      _properties[id] = newDescriptor;

      return newDescriptor;
    }

    private static PropertyInfo GetPropertyInfo<TClass, T>(Expression<Func<TClass, T>> propertyLambda)
    {
      // https://stackoverflow.com/questions/671968/retrieving-property-name-from-lambda-expression

      Type type = typeof(TClass);

      var member = propertyLambda.Body as MemberExpression;
      if (member == null)
        throw new ArgumentException(string.Format(
          "Expression '{0}' refers to a method, not a property.",
          propertyLambda.ToString()));

      var propInfo = member.Member as PropertyInfo;
      if (propInfo == null)
        throw new ArgumentException(string.Format(
          "Expression '{0}' refers to a field, not a property.",
          propertyLambda.ToString()));

      if (type != propInfo.DeclaringType)
        throw new ArgumentException(string.Format(
          "Expression '{0}' refers to a property that is not from type {1}.",
          propertyLambda.ToString(),
          type));

      return propInfo;
    }
  }
}