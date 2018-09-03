using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace TextRight.Core.Events
{
  /// <summary> Contains all registered properties for a given type. </summary>
  /// <typeparam name="TClass"> The type for which this registry is valid. </typeparam>
  internal static class PropertyDescriptorRegistry<TClass>
  {
    /// <summary> All of the properties that have been registered thus far. </summary>
    // ReSharper disable once StaticMemberInGenericType
    private static readonly Dictionary<string, IPropertyDescriptor> RegisteredProperties 
      = new Dictionary<string, IPropertyDescriptor>(StringComparer.CurrentCultureIgnoreCase);

    internal static IPropertyDescriptor<T> RegisterProperty<T>(Expression<Func<TClass, T>> propertyGetter, string id)
    {
      if (RegisteredProperties.TryGetValue(id, out var untypedValue))
      {
        if (untypedValue is IPropertyDescriptor<T> descriptor)
          return descriptor;

        throw new ArgumentException($"Property «{id}» already registered as a type «{untypedValue.DataType}», but attempting to re-register as type «{typeof(T)}»");
      }

      var property = GetPropertyInfo(propertyGetter);
      var newDescriptor = new PropertyDescriptor<T>(id, property);

      RegisteredProperties[id] = newDescriptor;

      return newDescriptor;
    }

    private static PropertyInfo GetPropertyInfo<T>(Expression<Func<TClass, T>> propertyLambda)
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