using System;
using System.Collections.Generic;
using System.Linq;

namespace TextRight.Core.ObjectModel
{
  /// <summary>
  ///   A unique identifier for the current process, for a specific type of component.
  /// </summary>
  /// <typeparam name="T"></typeparam>
  public abstract class RegisteredId
  {
    internal RegisteredId()
    {
      
    }

    /// <summary>
    ///  The globally unique id of the object.
    /// </summary>
    internal int UniqueId { get; private set; }

    /// <summary>
    ///  The display name the id was registered with.
    /// </summary>
    public string DisplayName { get; private set; }

    internal void Initialize(int uniqueId, string displayName)
    {
      UniqueId = uniqueId;
      DisplayName = displayName;
    }
  }

  /// <summary>
  ///   A unique identifier for the current process, for a specific type of component.
  /// </summary>
  /// <typeparam name="T"></typeparam>
  public abstract class RegisteredId<T> : RegisteredId, IEquatable<RegisteredId<T>>
  {
    internal RegisteredId()
    {

    }

    /// <summary />
    public bool Equals(RegisteredId<T> other)
    {
      if (ReferenceEquals(null, other))
        return false;
      if (ReferenceEquals(this, other))
        return true;

      return UniqueId == other.UniqueId;
    }

    /// <summary />
    public override bool Equals(object obj)
    {
      if (ReferenceEquals(null, obj))
        return false;
      if (ReferenceEquals(this, obj))
        return true;
      if (obj.GetType() != this.GetType())
        return false;

      return Equals((RegisteredId<T>)obj);
    }

    /// <summary />
    public override int GetHashCode()
      => UniqueId;

    /// <summary />
    public static bool operator ==(RegisteredId<T> left, RegisteredId<T> right)
      => Equals(left, right);

    /// <summary />
    public static bool operator !=(RegisteredId<T> left, RegisteredId<T> right)
      => !Equals(left, right);
  }
}