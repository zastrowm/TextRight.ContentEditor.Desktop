using System;
using System.Collections.Generic;
using System.Linq;
using TextRight.ContentEditor.Core.ObjectModel.Blocks;

namespace TextRight.ContentEditor.Core.ObjectModel.Cursors
{
  /// <summary> Holds a specific spot in the document. </summary>
  public sealed class DocumentCursor
  {
    /// <summary> Constructor. </summary>
    /// <param name="owner"> The Document that owns the given cursor. </param>
    /// <param name="blockCursor"> The block cursor. </param>
    public DocumentCursor(DocumentOwner owner, IBlockContentCursor blockCursor)
    {
      Owner = owner;
      BlockCursor = blockCursor;
    }

    /// <summary> Move to the position at the given cursor. </summary>
    /// <param name="cursor"> The cursor. </param>
    public void MoveTo(DocumentCursor cursor)
    {
      MoveTo(cursor.BlockCursor);
    }

    /// <summary> Move to the position at the given block cursor. </summary>
    /// <param name="blockCursor"> The block cursor. </param>
    public void MoveTo(IBlockContentCursor blockCursor)
    {
      BlockCursor = blockCursor;
    }

    /// <summary> The cursor. </summary>
    public ReadonlyCursor Cursor
      => new ReadonlyCursor(BlockCursor);

    /// <summary> The Document that owns the given cursor. </summary>
    public DocumentOwner Owner { get; }

    /// <summary> The cursor that points to the content within the block. </summary>
    public IBlockContentCursor BlockCursor { get; private set; }
  }
}