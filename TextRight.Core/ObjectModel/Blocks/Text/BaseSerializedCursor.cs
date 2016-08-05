using System;
using System.Collections.Generic;
using System.Linq;
using TextRight.ContentEditor.Core.ObjectModel.Cursors;

namespace TextRight.ContentEditor.Core.ObjectModel.Blocks
{
  /// <summary> Base class for serialized cursors. </summary>
  /// <typeparam name="TCursor"> The type of cursor to serialize </typeparam>
  public abstract class BaseSerializedCursor<TCursor> : ISerializedBlockCursor
    where TCursor : IBlockContentCursor
  {
    private BlockPath _path;

    /// <summary> Specialised constructor for use only by derived class. </summary>
    /// <param name="cursor"> The cursor to serialize. </param>
    protected BaseSerializedCursor(TCursor cursor)
    {
      _path = cursor.Block.GetBlockPath();
    }

    /// <inheritdoc/>
    public CursorCopy Deserialize(DocumentOwner owner)
    {
      var associatedBlock = (ContentBlock)_path.Get(owner);
      var cursorCopy = associatedBlock.CursorPool.GetCursorCopy(associatedBlock);

      Deserialize((TCursor)cursorCopy.Cursor);

      return cursorCopy;
    }

    /// <summary>
    ///  To be implemented by subclasses.  Should serialize all cursor specific data into the TCursor
    ///  class.
    /// </summary>
    /// <param name="cursor"> The cursor to serialize. </param>
    public abstract void Deserialize(TCursor cursor);
  }
}