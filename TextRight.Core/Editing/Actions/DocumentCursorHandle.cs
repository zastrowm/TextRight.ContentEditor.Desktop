using System;
using System.Collections.Generic;
using System.Linq;
using TextRight.Core.Cursors;
using TextRight.Core.ObjectModel.Blocks;
using TextRight.Core.ObjectModel.Blocks.Text;
using TextRight.Core.ObjectModel.Cursors;

namespace TextRight.Core.Editing.Actions
{
  /// <summary>
  ///  Holds a cursor that can be saved and restored at a later point, yet optimized so that if the
  ///  document hasn't changed, the cursor that was originally used to create the document can be
  ///  used directly.
  /// </summary>
  public class DocumentCursorHandle
  {
    private readonly ISerializedBlockCaret _serializedCursor;


    /// <summary> TODO </summary>
    public DocumentCursorHandle(BlockCaret caret)
    {
      _serializedCursor = caret.Serialize();
    }

    public TCaret GetCaret<TCaret>(DocumentEditorContext context, ICaretMover<TCaret> factory)
      where TCaret : struct, IBlockCaret, IEquatable<TCaret>
    {
      var genericCaret = GetCaret(context);
      if (genericCaret.Mover != factory)
        throw new ArgumentException($"Block cursor does not represent a cursor of type: {typeof(TCaret)}", nameof(factory));

      return factory.Convert(genericCaret);
    }

    public BlockCaret GetCaret(DocumentEditorContext context)
      => _serializedCursor.Deserialize(context);

    public BlockCaret Get(DocumentEditorContext context)
      => GetCaret(context);

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
      => cursor.MoveTo(handle.GetCaret(context));
  }
}