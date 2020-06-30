using System;
using System.Collections.Generic;
using System.Linq;
using TextRight.Core.Cursors;

namespace TextRight.Core.ObjectModel.Cursors
{
  /// <summary> Holds a specific spot in the document. </summary>
  public sealed class DocumentSelection
  {
    private BlockCaret _caretEnd;
    private BlockCaret _caretStart;

    /// <summary> Constructor. </summary>
    /// <param name="owner"> The Document that owns the given cursor. </param>
    /// <param name="blockCaret"> The block cursor. </param>
    public DocumentSelection(DocumentOwner owner, BlockCaret blockCaret)
    {
      Owner = owner;
      MoveTo(blockCaret, SelectionMode.Replace);
    }

    /// <summary> The Document that owns the given cursor. </summary>
    public DocumentOwner Owner { get; }

    /// <summary>
    ///  True if Moving using MoveTo should keep <see cref="End"/> at its current position,
    ///  false if it should move it to match the new cursor position.
    /// </summary>
    public bool ShouldExtendSelection { get; set; }

    /// <summary> True if a selection is active, false if it's a simple cursor. </summary>
    public bool HasSelection
      => _caretEnd != _caretStart;

    /// <summary> Move to the given caret location. </summary>
    /// <param name="blockCaret"> The block caret. </param>
    /// <param name="mode"> If the current selection should be replaced or extended to include the new position. </param>
    public void MoveTo(BlockCaret blockCaret, SelectionMode mode)
    {
      _caretStart = blockCaret;

      if (mode != SelectionMode.Extend)
      {
        _caretEnd = blockCaret;
      }

      CursorMoved?.Invoke(this, EventArgs.Empty);
    }

    /// <summary>
    ///   Calls <see cref="MoveTo"/> with <see cref="SelectionMode.Replace"/>
    /// </summary>
    public void Replace(BlockCaret caret)
      => MoveTo(caret, SelectionMode.Replace);

    /// <summary> The starting position of the selection. </summary>
    public BlockCaret Start
      => _caretStart;

    /// <summary> The ending position of the selection. </summary>
    public BlockCaret End
      => _caretEnd;

    /// <summary> Invoked when the cursor has moved. </summary>
    public event EventHandler CursorMoved;

    /// <summary> Checks if the caret is of the given type. </summary>
    public bool Is<T>()
      where T : struct, IEquatable<T>, IBlockCaret
      => Start.Is<T>();
  }
}