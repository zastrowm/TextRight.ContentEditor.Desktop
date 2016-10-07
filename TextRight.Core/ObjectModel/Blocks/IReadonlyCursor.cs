using System;
using System.Collections.Generic;
using System.Linq;
using TextRight.Core.ObjectModel.Cursors;
using TextRight.Core.Utilities;

namespace TextRight.Core.ObjectModel.Blocks
{
  /// <summary>
  ///  A view of a <see cref="IBlockContentCursor"/> that only allows the consumer to query
  ///  properties of the cursor. This interface only exists to make sure that all "read only
  ///  cursors" have the same properties.  Instead of this class <see cref="ReadonlyCursor"/> should
  ///  be used.
  /// </summary>
  internal interface IReadonlyCursor
  {
    /// <summary> Gets the block associated withe the cursor. </summary>
    Block Block { get; }

    /// <summary> True if the cursor is at the end of the block. </summary>
    bool IsAtEnd { get; }

    /// <summary> True if the cursor is at the end of the block. </summary>
    bool IsAtBeginning { get; }

    /// <summary> Creates a copy of the cursor stored in this instance. </summary>
    CursorCopy Copy();

    /// <summary>
    ///  Save the cursor information so that it can be restored at a later time.
    /// </summary>
    /// <returns>
    ///  An ISerializedBlockCursor that represents the serialized cursor.
    /// </returns>
    ISerializedBlockCursor Serialize();

    /// <summary> Returns a measurement of where a cursor should appear </summary>
    /// <returns> A MeasuredRectangle representing where the cursor should appear. </returns>
    MeasuredRectangle MeasureCursorPosition();

    /// <summary> Check if the underlying cursor is of type T. </summary>
    bool Is<T>()
      where T : IBlockContentCursor;
  }
}