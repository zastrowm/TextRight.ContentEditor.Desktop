using System;
using System.Collections.Generic;
using System.Linq;

namespace TextRight.Core.ObjectModel.Blocks
{
  /// <summary>
  ///  Represents an object attached to the document object model.
  /// </summary>
  public abstract class DocumentItem : EventEmitter, IDocumentItem
  {
    /// <inheritdoc />
    public IEditorData Tag { get; set; }
  }
}