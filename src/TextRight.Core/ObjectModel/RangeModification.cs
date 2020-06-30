using System;
using System.Collections.Generic;
using System.Linq;

namespace TextRight.Core.ObjectModel
{
  /// <summary> Holds relevant information concerning the change applied to collection of items. </summary>
  public struct RangeModification
  {
    public RangeModification(int index, int numberOfItems, bool wasAdded)
    {
      Index = index;
      NumberOfItems = numberOfItems;
      WasAdded = wasAdded;
    }

    /// <summary> The index at which the change starts. </summary>
    public int Index { get; }

    /// <summary> The number of items that the modification effects. </summary>
    public int NumberOfItems { get; }

    /// <summary>
    ///  True if <see cref="NumberOfItems"/> was added, false if it represents the number of
    ///  items removed.
    /// </summary>
    public bool WasAdded { get; }

    /// <summary> Inverse of <see cref="WasAdded"/> </summary>
    public bool WasRemoved
      => !WasAdded;

    /// <inheritdoc />
    public override string ToString()
      => $"Index={Index}, Count={NumberOfItems}, WasAdded={WasAdded}";
  }
}