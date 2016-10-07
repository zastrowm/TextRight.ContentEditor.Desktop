using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TextRight.Core.ObjectModel
{
  /// <summary>
  ///  Represents an immutable value that can be compared to another instance to see if a change has
  ///  occurred since the original instance was acquired.
  /// </summary>
  public struct ChangeIndex : IEquatable<ChangeIndex>
  {
    private readonly ulong _index;

    private ChangeIndex(ulong index)
    {
      _index = index;
    }

    /// <summary> Gets a ChangeIndex that represents the fact that a new change has occurred. </summary>
    public ChangeIndex Next()
    {
      return new ChangeIndex(_index + 1);
    }

    /// <summary> Gets a ChangeIndex that indicates that the change is out of date already. </summary>
    public ChangeIndex Previous()
    {
      return new ChangeIndex(_index - 1);
    }

    /// <summary> Returns true if the given instance is different from this instance. </summary>
    public bool HasChanged(ref ChangeIndex value)
    {
      if (value == this)
        return false;

      value = this;
      return true;
    }

    /// <summary />
    public bool Equals(ChangeIndex other)
    {
      return _index == other._index;
    }

    /// <inheritdoc />
    public override bool Equals(object obj)
    {
      if (ReferenceEquals(null, obj))
        return false;
      return obj is ChangeIndex && Equals((ChangeIndex)obj);
    }

    /// <inheritdoc />
    public override int GetHashCode()
    {
      return _index.GetHashCode();
    }

    /// <summary />
    public static bool operator ==(ChangeIndex left, ChangeIndex right)
    {
      return left.Equals(right);
    }

    /// <summary />
    public static bool operator !=(ChangeIndex left, ChangeIndex right)
    {
      return !left.Equals(right);
    }
  }
}