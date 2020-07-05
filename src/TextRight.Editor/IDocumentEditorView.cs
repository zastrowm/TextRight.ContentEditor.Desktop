using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using TextRight.Core.ObjectModel.Blocks;
using TextRight.Editor;

namespace TextRight.Core
{
  public interface IDocumentEditorView : IDocumentItemView
  {
    /// <summary> Gets the block closest to the given point. </summary>
    /// <param name="point"> The point at which the block should be queried. </param>
    /// <returns> The closes block to the given point. </returns>
    [NotNull]
    Block GetBlockFor(DocumentPoint point);
  }
}