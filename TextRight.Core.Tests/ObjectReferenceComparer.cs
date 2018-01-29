using System;
using System.Collections.Generic;
using System.Linq;

namespace TextRight.Core.Tests
{
  public class ObjectReferenceComparer<T> : IEqualityComparer<T>
  {
    public bool Equals(T x, T y) 
      => ReferenceEquals(x, y);

    public int GetHashCode(T obj) 
      => obj?.GetHashCode() ?? 0;
  }
}