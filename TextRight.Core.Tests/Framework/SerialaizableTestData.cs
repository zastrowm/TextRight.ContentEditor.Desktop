using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Xunit.Abstractions;

namespace TextRight.Core.Tests.Framework
{
  public abstract class SerialaizableTestData<T> : IXunitSerializable
    where T : new()
  {
    public void Deserialize(IXunitSerializationInfo info)
    {
      foreach (var field in typeof(T).GetFields())
      {
        field.SetValue(this, info.GetValue(field.Name, field.FieldType));
      }

      foreach (var property in typeof(T).GetProperties())
      {
        property.SetValue(this, info.GetValue(property.Name, property.PropertyType));
      }
    }

    public void Serialize(IXunitSerializationInfo info)
    {
      foreach (var field in typeof(T).GetFields())
      {
        info.AddValue(field.Name, field.GetValue(this));
      }

      foreach (var property in typeof(T).GetProperties())
      {
        info.AddValue(property.Name, property.GetValue(this));
      }
    }
  }
}