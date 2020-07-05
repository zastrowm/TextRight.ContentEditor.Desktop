using System;
using System.Collections.Generic;
using System.Linq;
using TextRight.Core.ObjectModel;

namespace TextRight.Editor
{
  /// <summary>
  ///   A view for a document-item representation.
  /// </summary>
  public interface IDocumentItemView : IEditorData
  {
    /// <summary> The DocumentItem associated with the View. </summary>
    IDocumentItem DocumentItem { get; }
  }
}