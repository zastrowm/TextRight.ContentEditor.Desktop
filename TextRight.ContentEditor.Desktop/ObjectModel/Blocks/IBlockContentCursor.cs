using System;
using System.Collections.Generic;
using System.Linq;
using TextRight.ContentEditor.Desktop.Blocks;

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

    /// <summary>
    ///  Save the cursor information so that it can be restored at a later time.
    /// </summary>
    /// <returns>
    ///  An ISerializedBlockCursor that represents the serialized cursor.
    /// </returns>
    ISerializedBlockCursor Serialize();

    void InsertText(string text);
  }

  /// <summary> Saved data that can be turned back into a IBlockContentCursor. </summary>
  public interface ISerializedBlockCursor
  {
    /// <summary>
    ///  Convert the serialized data back into a <see cref="IBlockContentCursor"/>.
    /// </summary>
    /// <param name="owner"> The document that contains the blocks that should be
    ///  part of the block cursor. </param>
    /// <returns>
    ///  An IBlockContentCursor that represents the serialized data.
    /// </returns>
    IBlockContentCursor Deserialize(DocumentOwner owner);
  }
}