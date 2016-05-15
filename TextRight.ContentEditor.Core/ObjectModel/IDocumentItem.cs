using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;

namespace TextRight.ContentEditor.Core.ObjectModel
{
  /// <summary> An item that exists in the Document ObjectModel. </summary>
  public interface IDocumentItem
  {
    
  }

  /// <summary> An item that exists in the Document ObjectModel. </summary>
  /// <typeparam name="T"> The type of view associated with the target. </typeparam>
  public interface IDocumentItem<T> : IDocumentItem
    where T : IDocumentItemView
  {
    /// <summary> The View that is currently attached to the item. </summary>
    [CanBeNull]
    T Target { get; set; }
  }
}