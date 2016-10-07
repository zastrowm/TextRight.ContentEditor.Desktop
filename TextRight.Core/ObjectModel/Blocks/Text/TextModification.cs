using System;
using System.Collections.Generic;
using System.Linq;

namespace TextRight.Core.ObjectModel.Blocks.Text
{
  /// <summary> Holds relevent information concerning the change applied to a block of text. </summary>
  public struct TextModification
  {
    public TextModification(int index, int numberOfCharacters, bool wasAdded)
    {
      Index = index;
      NumberOfCharacters = numberOfCharacters;
      WasAdded = wasAdded;
    }

    /// <summary> The index at which the change starts. </summary>
    public int Index { get; }

    /// <summary> The number of characters that the modification effects </summary>
    public int NumberOfCharacters { get; }

    /// <summary>
    ///  True if <see cref="NumberOfCharacters"/> was added, false if it represents the number of
    ///  characters removed.
    /// </summary>
    public bool WasAdded { get; }

    /// <summary> Inverse of <see cref="WasAdded"/> </summary>
    public bool WasRemoved
      => !WasAdded;
  }
}