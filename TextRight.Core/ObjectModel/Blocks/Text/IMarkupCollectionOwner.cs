using System;
using System.Collections.Generic;
using System.Linq;

namespace TextRight.Core.ObjectModel.Blocks.Text
{
  /// <summary> Interface for an object that has a definitive length. </summary>
  internal interface IMarkupCollectionOwner
  {
    /// <summary> Gets the length of the instance. </summary>
    int Length { get; }
  }
}