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
    private readonly ISerializedBlockCursor _serializedCursor;

    /// <summary> TODO </summary>
    public DocumentCursorHandle(TextCaret caret)
      : this(new TextBlockCursor(caret))
    {
      
    }

    /// <summary> Constructor. </summary>
    /// <param name="cursor"> The cursor that should be serialized for later use. </param>
    public DocumentCursorHandle(ReadonlyCursor cursor)
    {
      _serializedCursor = cursor.Serialize();
    }

    /// <summary> Constructor. </summary>
    /// <param name="cursor"> The cursor that should be serialized for later use. </param>
    public DocumentCursorHandle(DocumentCursor cursor)
    {
      _serializedCursor = cursor.Cursor.Serialize();
    }

    /// <summary> Constructor. </summary>
    /// <param name="cursor"> The cursor that should be serialized for later use. </param>
    public DocumentCursorHandle(IBlockContentCursor cursor)
    {
      _serializedCursor = cursor.Serialize();
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
    {
      using (var copy = Get(context))
      {
        return ((TextBlockCursor)copy.Cursor).ToValue();
      }
    }

    /// <summary>
    ///  Retrieves a live cursor which can be used to perform operations on the document.
    /// </summary>
    /// <param name="context"> The context for which the cursor is valid. </param>
    /// <returns> A DocumentCursor. </returns>
    public CursorCopy Get(DocumentEditorContext context)
    {
      // TODO if we're at the same revision # as when we created the cursor, we don't have to
      // deserialize we can use the original cursor. 
      var blockCursor = _serializedCursor.Deserialize(context.Document);
      return blockCursor;
    }

    /// <summary>
    ///  Implicit cast that converts the given DocumentCursor to a DocumentCursorHandle.
    /// </summary>
    public static implicit operator DocumentCursorHandle(DocumentCursor cursor)
    {
      return new DocumentCursorHandle(cursor);
    }

    /// <summary>
    ///  Implicit cast that converts the given DocumentCursor to a DocumentCursorHandle.
    /// </summary>
    public static implicit operator DocumentCursorHandle(ReadonlyCursor cursor)
    {
      return new DocumentCursorHandle(cursor);
    }
  }

  /// <summary> Extension methods related to <see cref="DocumentCursorHandle"/> </summary>
  public static class DocumentCursorHandleExtensions
  {
    /// <summary>
    ///  Moves to he cursor to point to the location given by <paramref name="handle"/>.
    /// </summary>
    public static void MoveTo(this DocumentCursor cursor, DocumentCursorHandle handle, DocumentEditorContext context)
    {
      using (var copy = handle.Get(context))
      {
        cursor.MoveTo(copy);
      }
    }
  }
}