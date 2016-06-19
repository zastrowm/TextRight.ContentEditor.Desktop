using System;
using System.Collections.Generic;
using System.Linq;
using TextRight.ContentEditor.Core.ObjectModel.Cursors;
using TextRight.ContentEditor.Core.Utilities;

namespace TextRight.ContentEditor.Core.ObjectModel.Blocks
{
  /// <summary>
  ///  A view of a <see cref="IBlockContentCursor"/> that only allows the consumer to query
  ///  properties of the cursor.
  /// </summary>
  public struct ReadonlyCursor
  {
    private readonly IBlockContentCursor _cursor;

    /// <summary> Constructor. </summary>
    /// <param name="cursor"> The cursor that this instance should wrap in a readonly-view. </param>
    public ReadonlyCursor(IBlockContentCursor cursor)
    {
      _cursor = cursor;
    }

    /// <summary> Gets the block associated withe the cursor. </summary>
    public Block Block
      => _cursor.Block;

    /// <summary> True if the cursor is at the end of the block. </summary>
    public bool IsAtEnd
      => _cursor.IsAtEnd;

    /// <summary> True if the cursor is at the end of the block. </summary>
    public bool IsAtBeginning
      => _cursor.IsAtBeginning;

    /// <summary> Creates a copy of the cursor stored in this instance. </summary>
    public CursorCopy Copy()
      => new CursorCopy(_cursor);

    /// <summary>
    ///  Save the cursor information so that it can be restored at a later time.
    /// </summary>
    /// <returns>
    ///  An ISerializedBlockCursor that represents the serialized cursor.
    /// </returns>
    public ISerializedBlockCursor Serialize()
      => _cursor.Serialize();

    /// <summary> Returns a measurement of where a cursor should appear </summary>
    /// <returns> A MeasuredRectangle representing where the cursor should appear. </returns>
    public MeasuredRectangle MeasureCursorPosition()
      => _cursor.MeasureCursorPosition();
  }
}