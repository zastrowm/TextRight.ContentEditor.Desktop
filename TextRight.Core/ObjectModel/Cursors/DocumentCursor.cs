using System;
using System.Collections.Generic;
using System.Linq;
using TextRight.Core.Cursors;
using TextRight.Core.ObjectModel.Blocks;
using TextRight.Core.ObjectModel.Blocks.Text;

namespace TextRight.Core.ObjectModel.Cursors
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
      _pooledStartSelection = new PooledBlockContentCursor();
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

      CursorMoved?.Invoke(this, EventArgs.Empty);
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

    /// <summary> Move to the given caret location. </summary>
    /// <param name="blockCaret"> The block caret. </param>
    public void MoveTo(BlockCaret blockCaret) 
      // TODO make it not TextBlock specific
      => MoveTo(new TextBlockCursor((TextCaret)blockCaret));

    /// <summary> The current caret position. </summary>
    public BlockCaret Caret
    {
      get
      {
      // TODO make it not TextBlock specific
        using (var copy = Cursor.Copy())
        {
          return ((TextBlockCursor)copy.Cursor).ToValue();
        }
      }
    }

    /// <summary> A readonly cursor which allows positional information to be read. </summary>
    public ReadonlyCursor Cursor
      => new ReadonlyCursor(_pooledEndSelection.Cursor);

    /// <summary> The start of the current selection. </summary>
    public ReadonlyCursor SelectionStart
      => new ReadonlyCursor(_pooledStartSelection.Cursor);

    /// <summary> Invoked when the cursor has moved. </summary>
    public event EventHandler CursorMoved;

    /// <summary> Checks if the caret is of the given type. </summary>
    public bool Is<T>()
      where T : struct, IEquatable<T>, IBlockCaret
      => Caret.Is<T>();
  }
}