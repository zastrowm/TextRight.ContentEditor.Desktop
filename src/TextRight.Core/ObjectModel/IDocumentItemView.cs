using System;
using System.Collections.Generic;
using System.Linq;
using TextRight.Core.ObjectModel.Blocks;

namespace TextRight.Core.ObjectModel
{
  /// <summary> A view item for a specific type of <see cref="IDocumentItem{T}"/>. </summary>
  public interface IDocumentItemView : Block.ITagData
  {
    /// <summary> The DocumentItem associated with the View. </summary>
    IDocumentItem DocumentItem { get; }
  }
}