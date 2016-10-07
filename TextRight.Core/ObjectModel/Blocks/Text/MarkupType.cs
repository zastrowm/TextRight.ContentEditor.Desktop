using System;
using System.Collections.Generic;
using System.Linq;

namespace TextRight.Core.ObjectModel.Blocks.Text
{
  /// <summary>
  ///  A type designator for <see cref="Markup"/> instances so that the original owner of the markup
  ///  can be identified.
  /// </summary>
  public struct MarkupType
  {
    private readonly long _internalIndex;

    private readonly string _fullName;

    /// <summary> Constructor. </summary>
    /// <param name="internalIndex"> Zero-based index of the internal index for a specific document. </param>
    /// <param name="fullName"> The full name of the markup type. </param>
    /// <param name="keepOnInvalidation"> (Optional) True if the markup is kept even if the underlying
    ///  text changes. </param>
    internal MarkupType(long internalIndex, string fullName, bool keepOnInvalidation = false)
    {
      _internalIndex = internalIndex;
      _fullName = fullName;

      KeepOnInvalidation = keepOnInvalidation;
    }

    /// <summary> True if the markup is kept even if the underlying text changes </summary>
    private bool KeepOnInvalidation { get; }

    /// <summary />
    public bool Equals(MarkupType other)
    {
      return _internalIndex == other._internalIndex &&
             string.Equals(_fullName, other._fullName, StringComparison.OrdinalIgnoreCase);
    }

    /// <inheritdoc/>
    public override bool Equals(object obj)
    {
      if (ReferenceEquals(null, obj))
        return false;
      return obj is MarkupType && Equals((MarkupType)obj);
    }

    /// <inheritdoc/>
    public override int GetHashCode()
    {
      unchecked
      {
        return (_internalIndex.GetHashCode() * 397) ^ StringComparer.OrdinalIgnoreCase.GetHashCode(_fullName);
      }
    }
  }
}