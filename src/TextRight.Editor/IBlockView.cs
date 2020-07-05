using System;
using System.Collections.Generic;
using System.Linq;
using TextRight.Core.Cursors;
using TextRight.Core.ObjectModel.Cursors;
using TextRight.Core.Utilities;

namespace TextRight.Editor
{
  /// <summary>
  ///   A view representation for a block within the document.
  /// </summary>
  public interface IBlockView : IDocumentItemView
  {
    /// <summary> Measures the bounds of the block if the entire thing was selected. </summary>
    MeasuredRectangle MeasureSelectionBounds();

    // <summary> Measures the bounds of a specific caret position. </summary>
    MeasuredRectangle Measure(BlockCaret caret);

    /// <summary>
    ///  Retrieves a caret within the block that represents the given
    ///  CaretMovementMode as if a cursor with the given mode was arriving from
    ///  the top of the block.
    ///  
    ///  For example, for a CaretMovementMode with a Mode of
    ///  <see cref="CaretMovementMode.Mode.Position"/>
    ///  and a TextBlock, the caret should represent a caret that is
    ///  <see cref="CaretMovementMode.Position"/> units from the left-side of the
    ///  text on the first line in the text block.
    /// </summary>
    /// <seealso cref="GetCaretFromTop"/>
    /// <param name="movementMode"> The caret movement mode. </param>
    /// <returns> The given caret. </returns>
    BlockCaret GetCaretFromBottom(CaretMovementMode movementMode);

    /// <summary>
    ///  Retrieves a caret within the block that represents the given
    ///  CaretMovementMode as if a cursor with the given mode was arriving from
    ///  the bottom of the block.
    /// </summary>
    /// <seealso cref="GetCaretFromTop"/>
    /// <param name="movementMode"> The caret movement mode. </param>
    /// <returns> The given caret. </returns>
    BlockCaret GetCaretFromTop(CaretMovementMode movementMode);
  }
}