using System;
using System.Collections.Generic;
using System.Linq;

namespace TextRight.Core.ObjectModel
{
  /// <summary>
  ///  Base class for giving out unique identifiers (that derive from <see cref="RegisteredId"/>)
  ///  to various types of components.
  /// </summary>
  /// <typeparam name="T"> The type of identifier to give out. </typeparam>
  public class Registry<T>
    where T : RegisteredId, new()
  {
    private static readonly Dictionary<string, T> RegisteredIds 
      = new Dictionary<string, T>();

    internal Registry()
    {
      
    }

    /// <summary>
    ///   Retrieve a new entry for the given item.
    /// </summary>
    public static T RegisterNew(string name)
    {
      lock (RegisteredIds)
      {
        if (RegisteredIds.TryGetValue(name, out var item))
          return item;

        var newId = new T();
        newId.Initialize(RegisteredIds.Count + 1, name);
        RegisteredIds.Add(name, newId);
        return newId;
      }
    }
  }
}