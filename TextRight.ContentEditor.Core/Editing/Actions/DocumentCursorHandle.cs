using System;
using System.Collections.Generic;
using System.Linq;
using TextRight.ContentEditor.Core.ObjectModel;
using TextRight.ContentEditor.Core.ObjectModel.Blocks;

namespace TextRight.ContentEditor.Core.Editing.Actions
{
  /// <summary>
  ///  Holds a cursor that can be saved and restored at a later point, yet optimized so that if the
  ///  document hasn't changed, the cursor that was originally used to create the document can be
  ///  used directly.
  /// </summary>
  public class DocumentCursorHandle
  {
    private readonly ISerializedBlockCursor _serializedCursor;

    /// <summary> Constructor. </summary>
    /// <param name="cursor"> The cursor that should be serialized for later use. </param>
    public DocumentCursorHandle(DocumentCursor cursor)
    {
      _serializedCursor = cursor.BlockCursor.Serialize();
    }

    /// <summary>
    ///  Retrieves a live cursor which can be used to perform operations on the document.
    /// </summary>
    /// <param name="context"> The context for which the cursor is valid. </param>
    /// <returns> A DocumentCursor. </returns>
    public IBlockContentCursor Get(DocumentEditorContext context)
    {
      // TODO if we're at the same revision # as when we created the cursor, we don't have to
      // deserialize we can use the original cursor. 
      var blockCursor = _serializedCursor.Deserialize(context.Document);
      return blockCursor;
    }
  }
}