using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace TextRight.Core.Tests.Framework
{
  public static class ReflectionHelpers
  {
    public static TValue GetFieldValue<TValue>(this object instance, string name)
    {
      return (TValue)instance.GetType()
        .GetField(name, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static)
        .GetValue(instance);
    }
  }
}
