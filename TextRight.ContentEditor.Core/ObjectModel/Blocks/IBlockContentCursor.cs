using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using TextRight.ContentEditor.Core.ObjectModel.Cursors;
using TextRight.ContentEditor.Core.Utilities;

namespace TextRight.ContentEditor.Core.ObjectModel.Blocks
{
  /// <summary> A cursor that can move through a specific type of block. </summary>
  public interface IBlockContentCursor
  {
    /// <summary> Gets the block associated withe the cursor. </summary>
    [NotNull]
    ContentBlock Block { get; }

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

    /// <summary> Sets the block that this cursor is associated with. </summary>
    /// <param name="block"> The block associated withe the cursor. </param>
    void Reset(ContentBlock block);

    /// <summary>
    ///  Save the cursor information so that it can be restored at a later time.
    /// </summary>
    /// <returns>
    ///  An ISerializedBlockCursor that represents the serialized cursor.
    /// </returns>
    ISerializedBlockCursor Serialize();

    /// <summary> True if the cursor is at the end of the block. </summary>
    bool IsAtEnd { get; }

    /// <summary> True if the cursor is at the end of the block. </summary>
    bool IsAtBeginning { get; }

    /// <summary> A pool of similar cursors. </summary>
    ICursorPool CursorPool { get; }

    /// <summary> Returns a measurement of where a cursor should appear </summary>
    /// <returns> A MeasuredRectangle representing where the cursor should appear. </returns>
    MeasuredRectangle MeasureCursorPosition();

    /// <summary> Moves the given cursor to the location indicated by the parameter. </summary>
    /// <param name="cursor"> The cursor to move to. </param>
    void MoveTo(IBlockContentCursor cursor);
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
    CursorCopy Deserialize(DocumentOwner owner);
  }
}