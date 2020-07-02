using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using TextRight.Core.ObjectModel.Blocks;

namespace TextRight.Core.ObjectModel
{
  /// <summary> An item that exists in the Document ObjectModel. </summary>
  public interface IDocumentItem
  {
    [CanBeNull]
    Block.ITagData Tag { get; set; }
  }
}