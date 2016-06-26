using System;
using System.Collections.Generic;
using System.Linq;
using TextRight.ContentEditor.Core.ObjectModel.Blocks;

namespace TextRight.ContentEditor.Core.ObjectModel.Cursors
{
  /// <summary> Holds a specific spot in the document. </summary>
  public sealed class DocumentCursor
  {
    private PooledBlockContentCursor _pooledEndSelection;
    private PooledBlockContentCursor _pooledStartSelection;

    /// <summary> Constructor. </summary>
    /// <param name="owner"> The Document that owns the given cursor. </param>
    /// <param name="blockCursor"> The block cursor. </param>
    public DocumentCursor(DocumentOwner owner, IBlockContentCursor blockCursor)
    {
      Owner = owner;
      _pooledEndSelection = new PooledBlockContentCursor();
      _pooledEndSelection.Set(blockCursor);
    }

    /// <summary> The Document that owns the given cursor. </summary>
    public DocumentOwner Owner { get; }

    /// <summary>
    ///  True if Moving using MoveTo should keep <see cref="SelectionStart"/> at its current position,
    ///  false if it should move it to match the new cursor position.
    /// </summary>
    public bool ShouldExtendSelection { get; set; }

    /// <summary> True if a selection is active, false if it's a simple cursor. </summary>
    public bool HasSelection
      => !_pooledStartSelection.Cursor.Equals(_pooledEndSelection.Cursor);

    /// <summary> Move to the position at the given block cursor. </summary>
    /// <param name="blockCursor"> The block cursor. </param>
    public void MoveTo(IBlockContentCursor blockCursor)
    {
      _pooledEndSelection.Set(blockCursor);

      if (!ShouldExtendSelection)
      {
        _pooledStartSelection.Set(blockCursor);
      }
    }

    /// <summary> Move to the position at the given cursor. </summary>
    /// <param name="cursor"> The cursor. </param>
    public void MoveTo(DocumentCursor cursor)
    {
      MoveTo(cursor._pooledEndSelection.Cursor);
    }

    /// <summary> Move to the position at the given cursor. </summary>
    /// <param name="cursorCopy"> The cursor copy. </param>
    public void MoveTo(CursorCopy cursorCopy)
    {
      MoveTo(cursorCopy.Cursor);
    }

    /// <summary> A readonly cursor which allows positional information to be read. </summary>
    public ReadonlyCursor Cursor
      => new ReadonlyCursor(_pooledEndSelection.Cursor);

    /// <summary> The start of the current selection. </summary>
    public ReadonlyCursor SelectionStart
      => new ReadonlyCursor(_pooledStartSelection.Cursor);
  }
}