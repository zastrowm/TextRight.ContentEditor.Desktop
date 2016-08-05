using System;
using System.Linq;
using System.Collections.Generic;

namespace TextRight.ContentEditor.Core.Internal
{
  /// <summary> Provides Array.Empty since it's not available for all frameworks. </summary>
  internal static class ArrayEx
  {
    /// <summary> Returns a cached empty array. </summary>
    /// <typeparam name="T"> The type of array to return. </typeparam>
    /// <returns> An array of zero elements. </returns>
    public static T[] Empty<T>()
    {
      return Implementation<T>.Array;
    }

    private static class Implementation<T>
    {
      public static readonly T[] Array = new T[0];
    }
  }
}