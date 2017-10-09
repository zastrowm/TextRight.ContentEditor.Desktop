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
    private BlockCaret _startCaret;
    private BlockCaret _endCaret;

    /// <summary> Constructor. </summary>
    /// <param name="owner"> The Document that owns the given cursor. </param>
    /// <param name="blockCaret"> The block cursor. </param>
    public DocumentCursor(DocumentOwner owner, BlockCaret blockCaret)
    {
      Owner = owner;
      MoveTo(blockCaret);
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
      => _startCaret != _endCaret;

    /// <summary> Move to the given caret location. </summary>
    /// <param name="blockCaret"> The block caret. </param>
    public void MoveTo(BlockCaret blockCaret)
    {
      _endCaret = blockCaret;

      if (!ShouldExtendSelection)
      {
        _startCaret = blockCaret;
      }

      CursorMoved?.Invoke(this, EventArgs.Empty);
    }

    /// <summary> The current caret position. </summary>
    public BlockCaret Caret
      => _endCaret;

    public BlockCaret SelectionStart
      => _startCaret;

    /// <summary> Invoked when the cursor has moved. </summary>
    public event EventHandler CursorMoved;

    /// <summary> Checks if the caret is of the given type. </summary>
    public bool Is<T>()
      where T : struct, IEquatable<T>, IBlockCaret
      => Caret.Is<T>();
  }
}