using System;
using System.Collections.Generic;
using System.Linq;
using TextRight.Core.Cursors;
using TextRight.Core.ObjectModel.Cursors;

namespace TextRight.Core.Actions
{
  /// <summary>
  ///  Holds a cursor that can be saved and restored at a later point, yet optimized so that if the
  ///  document hasn't changed, the cursor that was originally used to create the document can be
  ///  used directly.
  /// </summary>
  public class DocumentCursorHandle
  {
    private readonly ISerializedBlockCaret _serializedCursor;

    /// <summary> Constructor. </summary>
    /// <param name="caret"> The caret that should be saved for later usage. </param>
    public DocumentCursorHandle(BlockCaret caret)
    {
      _serializedCursor = caret.Serialize();
    }

    /// <summary> Deserializes the caret so that it can be reused. </summary>
    /// <param name="context"> The context which can be used to deserialize the caret. </param>
    /// <returns> The caret. </returns>
    public BlockCaret GetCaret(DocumentEditorContext context)
    {
      // TODO if we're at the same revision # as when we created the cursor, we don't have to
      // deserialize we can use the original cursor. 
      // 
      // you know.  Like the documentation on the class says :: )

      return _serializedCursor.Deserialize(context);
    }

    /// <summary>
    ///  Implicit cast that converts the given DocumentCursor to a DocumentCursorHandle.
    /// </summary>
    public static implicit operator DocumentCursorHandle(DocumentSelection cursor)
    {
      return new DocumentCursorHandle(cursor.Start);
    }
  }

  /// <summary> Extension methods related to <see cref="DocumentCursorHandle"/> </summary>
  public static class DocumentCursorHandleExtensions
  {
    /// <summary>
    ///  Moves to he cursor to point to the location given by <paramref name="handle"/>.
    /// </summary>
    public static void MoveTo(this DocumentSelection cursor, DocumentCursorHandle handle, DocumentEditorContext context)
      => cursor.Replace(handle.GetCaret(context));
  }
}