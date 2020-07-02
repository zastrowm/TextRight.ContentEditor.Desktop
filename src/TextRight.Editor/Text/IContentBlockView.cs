using System;
using System.Collections.Generic;
using System.Linq;
using TextRight.Core.Cursors;
using TextRight.Core.ObjectModel;
using TextRight.Core.ObjectModel.Blocks;
using TextRight.Core.ObjectModel.Cursors;
using TextRight.Core.Utilities;

namespace TextRight.Editor.Text
{
  public interface IBlockView
  {
    /// <summary> Gets the bounds of the block if the entire thing was selected. </summary>
    /// <returns> The bounds that encompass the area consumed by the block. </returns>
    MeasuredRectangle MeasureSelectionBounds();

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

  public interface IContentBlockView : IBlockView, Block.ITagData
  {
    
  }

  public static class Measurers
  {
    public static T GetView<T>(this Block block)
      where T : class, IBlockView
    {
      return block.GetViewOrNull<T>()
             ?? throw new InvalidOperationException($"block {block} does not have a view attached to it");
    }
    
    
    public static T GetViewOrNull<T>(this Block block)
      where T : class, IBlockView
    {
      return block.Tag as T;
    }
  
  }
}