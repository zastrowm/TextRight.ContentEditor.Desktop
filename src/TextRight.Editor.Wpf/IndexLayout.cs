using System;
using System.Collections.Generic;
using System.Linq;
using TextRight.Core.ObjectModel;
using TextRight.Editor.Wpf.View;

namespace TextRight.Editor.Wpf
{
  /// <summary>
  ///  Caches the index for a specific <see cref="ILayoutable"/>, allowing easy checking to see if
  ///  it's changed.
  /// </summary>
  /// <typeparam name="T"> Generic type parameter. </typeparam>
  public struct IndexLayout : IEquatable<IndexLayout>
  {
    private readonly ChangeIndex _index;
    private readonly ILayoutable _layoutable;

    public IndexLayout(ILayoutable instance)
    {
      _layoutable = instance;
      _index = instance?.LayoutIndex ?? new ChangeIndex();
    }

    /// <summary />
    public bool Equals(IndexLayout other)
      => _layoutable == other._layoutable && _index.Equals(other._index);

    /// <summary />
    public override bool Equals(object obj)
    {
      if (ReferenceEquals(null, obj))
        return false;

      return obj is IndexLayout other && Equals(other);
    }

    /// <summary> Returns true if the given instance is different from this instance. </summary>
    /// <param name="value"> [in,out] The value to update to point to this instance, if they values
    ///  are different. </param>
    /// <returns> True if changed, false if not. </returns>
    public bool HasChanged(ref IndexLayout value)
    {
      if (value == this)
        return false;

      value = this;
      return true;
    }

    /// <summary />
    public override int GetHashCode()
      => _index.GetHashCode();

    /// <summary />
    public static bool operator ==(IndexLayout left, IndexLayout right)
      => left.Equals(right);

    /// <summary />
    public static bool operator ==(IndexLayout left, BaseBlockView right)
      => left.Equals(new IndexLayout(right));

    /// <summary />
    public static bool operator !=(IndexLayout left, IndexLayout right)
      => !left.Equals(right);

    /// <summary />
    public static bool operator !=(IndexLayout left, BaseBlockView right)
      => !left.Equals(new IndexLayout(right));

    /// <summary />
    public static implicit operator IndexLayout(BaseBlockView right)
      => new IndexLayout(right);
  }
}