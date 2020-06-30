using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Xunit.Sdk;

namespace TextRight.Core.Tests.Framework
{
  [AttributeUsage(AttributeTargets.Method,Inherited = false)]
  public sealed class RangedDataAttribute : DataAttribute
  {
    private readonly int _startIndex;
    private readonly int _endIndex;

    public RangedDataAttribute(int startIndex, int endIndex)
    {
      _startIndex = startIndex;
      _endIndex = endIndex;
    }

    public override IEnumerable<object[]> GetData(MethodInfo testMethod)
    {
      if (testMethod == null)
        throw new ArgumentNullException("testMethod");

      return Enumerable.Range(_startIndex, _endIndex).Select(i => new object[] { i });
    }
  }
}