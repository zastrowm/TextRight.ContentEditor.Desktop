using System;
using System.Collections.Generic;
using System.Linq;

namespace TextRight.ContentEditor.Desktop.ObjectModel.Blocks
{
  /// <summary> A cursor that can move through a specific type of block. </summary>
  public interface IBlockContentCursor
  {
    /// <summary> Gets the block associated withe the cursor. </summary>
    Block Block { get; }

    /// <summary> Move to the beginning of the block. </summary>
    void MoveToBeginning();

    /// <summary> Move to the end of the block. </summary>
    void MoveToEnd();

    /// <summary> Try to move forward one unit in the block. </summary>
    /// <returns> True if the cursor was able to be moved forward, false otherwise. </returns>
    bool MoveForward();

    /// <summary> Try to move backward one unit in the block. </summary>
    /// <returns> True if the cursor was able to be moved backward, false otherwise. </returns>
    bool MoveBackward();
  }
}