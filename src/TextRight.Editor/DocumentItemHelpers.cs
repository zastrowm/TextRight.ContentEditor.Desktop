using System;
using System.Collections.Generic;
using System.Linq;
using TextRight.Core.ObjectModel;
using TextRight.Editor.Text;

namespace TextRight.Editor
{
  /// <summary>
  ///   Helper methods for retrieving the editor views associated with a specific document item.
  /// </summary>
  public static class DocumentItemHelpers
  {
    /// <summary>
    ///   Attempts to cast the <see cref="IDocumentItem.Tag"/> to type <typeparamref name="T"/>, returning null
    ///   if the <see cref="IDocumentItem.Tag"/> is null or the tag is not compatible with T.
    /// </summary>
    public static T GetViewOrNull<T>(this IDocumentItem documentItem)
      where T : class, IDocumentItemView
    {
      return documentItem.Tag as T;
    }

    /// <summary>
    ///   Attempts to cast the <see cref="IDocumentItem.Tag"/> to type <typeparamref name="T"/>, throwing an exception
    ///   if <see cref="IDocumentItem.Tag"/> is null or the tag is not compatible with T.
    /// </summary>
    public static T GetView<T>(this IDocumentItem documentItem)
      where T : class, IDocumentItemView
    {
      return documentItem.GetViewOrNull<T>()
             ?? throw new InvalidOperationException($"block {documentItem} does not have a view attached to it");
    }
  }
}