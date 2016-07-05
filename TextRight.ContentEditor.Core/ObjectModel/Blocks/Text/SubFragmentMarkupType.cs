using System;
using System.Collections.Generic;
using System.Linq;

namespace TextRight.ContentEditor.Core.ObjectModel.Blocks
{
  /// <summary> A sub fragment markup type. </summary>
  public struct SubFragmentMarkupType
  {
    private readonly long _internalIndex;

    private readonly string _fullName;

    /// <summary> Constructor. </summary>
    /// <param name="internalIndex"> Zero-based index of the internal index for a specific document. </param>
    /// <param name="fullName"> The full name of the markup type. </param>
    /// <param name="keepOnInvalidation"> (Optional) True if the markup is kept even if the underlying
    ///  text changes. </param>
    internal SubFragmentMarkupType(long internalIndex, string fullName, bool keepOnInvalidation = false)
    {
      _internalIndex = internalIndex;
      _fullName = fullName;

      KeepOnInvalidation = keepOnInvalidation;
    }

    /// <summary> True if the markup is kept even if the underlying text changes </summary>
    private bool KeepOnInvalidation { get; }

    /// <summary />
    public bool Equals(SubFragmentMarkupType other)
    {
      return _internalIndex == other._internalIndex &&
             string.Equals(_fullName, other._fullName, StringComparison.InvariantCultureIgnoreCase);
    }

    /// <inheritdoc/>
    public override bool Equals(object obj)
    {
      if (ReferenceEquals(null, obj))
        return false;
      return obj is SubFragmentMarkupType && Equals((SubFragmentMarkupType)obj);
    }

    /// <inheritdoc/>
    public override int GetHashCode()
    {
      unchecked
      {
        return (_internalIndex.GetHashCode() * 397) ^ StringComparer.InvariantCultureIgnoreCase.GetHashCode(_fullName);
      }
    }
  }
}