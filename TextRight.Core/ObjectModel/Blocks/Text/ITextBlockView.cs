using System;
using System.Collections.Generic;
using System.Linq;
using TextRight.Core.Utilities;

namespace TextRight.Core.ObjectModel.Blocks.Text
{
  /// <summary> Hosts the view for the TextBlock. </summary>
  public interface ITextBlockView : IDocumentItemView
  {
    /// <summary> Notifies the view that a fragment has been inserted. </summary>
    /// <param name="previousSibling"> The fragment that precedes the new fragment. </param>
    /// <param name="newFragment"> The fragment that is inserted. </param>
    /// <param name="nextSibling"> The fragment that comes after the block that is being inserted. </param>
    void NotifyBlockInserted(StyledTextFragment previousSibling,
                             StyledTextFragment newFragment,
                             StyledTextFragment nextSibling);

    /// <summary> Returns the area consumed by the block. </summary>
    /// <returns> A MeasuredRectangle representing the area required to display the block. </returns>
    MeasuredRectangle MeasureBounds();
  }
}