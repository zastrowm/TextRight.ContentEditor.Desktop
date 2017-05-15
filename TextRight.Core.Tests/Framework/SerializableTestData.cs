using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Xunit.Abstractions;

namespace TextRight.Core.Tests.Framework
{
  /// <summary>
  ///  Test Data that can be serialized for XUnit. Required as otherwise XUnit aggregates member
  ///  data into a single test.
  /// </summary>
  public abstract class SerializableTestData<T> : IXunitSerializable
    where T : new()
  {
    public virtual bool IsRecursive
      => false;

    public void Deserialize(IXunitSerializationInfo info)
    {
      try
      {
        DeserializeParts("", info, this);
      }
      catch (Exception e)
      {
      }
    }

    public void Serialize(IXunitSerializationInfo info)
    {
      try
      {
        SerializeParts("", info, this);
      }
      catch (Exception e)
      {
      }
    }

    private void DeserializeParts(string prefix, IXunitSerializationInfo info, object instance)
    {
      var type = instance.GetType();

      foreach (var field in GetFields(type))
      {
        field.SetValue(instance, GetValue(info, prefix + field.Name, field.FieldType));
      }

      foreach (var property in GetProperties(type))
      {
        property.SetValue(instance, GetValue(info, prefix + property.Name, property.PropertyType));
      }
    }

    private void SerializeParts(string prefix, IXunitSerializationInfo info, object instance)
    {
      var type = instance.GetType();

      foreach (var field in GetFields(type))
      {
        SetValue(info, prefix + field.Name, field.GetValue(instance));
      }

      foreach (var property in GetProperties(type))
      {
        SetValue(info, prefix + property.Name, property.GetValue(instance));
      }
    }

    private static IEnumerable<PropertyInfo> GetProperties(Type type) 
      => type.GetProperties(BindingFlags.Instance | BindingFlags.Public).Where(p => p.SetMethod != null);

    private static IEnumerable<FieldInfo> GetFields(Type type) 
      => type.GetFields(BindingFlags.Instance | BindingFlags.Public);

    private object GetValue(IXunitSerializationInfo info, string name, Type type)
    {
      if (!IsRecursive || IsSupportedType(type))
        return info.GetValue(name, type);

      var value = Activator.CreateInstance(type);
      DeserializeParts(name + ".", info, value);
      return value;
    }

    private void SetValue(IXunitSerializationInfo info, string name, object value)
    {
      if (!IsRecursive || value == null || IsSupportedType(value.GetType()))
      {
        info.AddValue(name, value);
        return;
      }

      SerializeParts(name + ".", info, value);
    }

    private static readonly ISet<Type> SupportedTypes
      = new HashSet<Type>()
        {
          typeof(IXunitSerializable),
          typeof(char),
          typeof(char?),
          typeof(string),
          typeof(byte),
          typeof(byte?),
          typeof(short),
          typeof(short?),
          typeof(ushort),
          typeof(ushort?),
          typeof(int),
          typeof(int?),
          typeof(uint),
          typeof(uint?),
          typeof(long),
          typeof(long?),
          typeof(ulong),
          typeof(ulong?),
          typeof(float),
          typeof(float?),
          typeof(double),
          typeof(double?),
          typeof(Decimal),
          typeof(Decimal?),
          typeof(bool),
          typeof(bool?),
          typeof(DateTime),
          typeof(DateTime?),
          typeof(DateTimeOffset),
          typeof(DateTimeOffset?),
        };

    private static bool IsSupportedType(Type type)
    {
      return SupportedTypes.Contains(type);
    }
  }
}