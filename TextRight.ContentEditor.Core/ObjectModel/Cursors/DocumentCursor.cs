using System;
using System.Collections.Generic;
using System.Linq;
using TextRight.ContentEditor.Core.ObjectModel.Blocks;

namespace TextRight.ContentEditor.Core.ObjectModel.Cursors
{
  /// <summary> Holds a specific spot in the document. </summary>
  public class DocumentCursor
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

    /// <summary> The Document that owns the given cursor. </summary>
    public DocumentOwner Owner { get; }

    /// <summary> The cursor that points to the content within the block. </summary>
    public IBlockContentCursor BlockCursor { get; private set; }

    /// <summary> Moves the cursor forward within its current block. </summary>
    /// <returns> True if the cursor moved forward, false if it did not move forward. </returns>
    private bool MoveForwardInBlock()
      => BlockCursor.MoveForward();

    /// <summary> Moves the cursor backward within its current block. </summary>
    /// <returns> True if the cursor moved backward false if it did not move backward. </returns>
    private bool MoveBackwardInBlock()
      => BlockCursor.MoveBackward();

    /// <summary> Move the cursor forward in the document. </summary>
    /// <returns> True if the cursor was moved forward, false otherwise. </returns>
    public bool MoveForward()
    {
      if (MoveForwardInBlock())
        return true;

      var block = BlockCursor.Block;
      var nextBlock = BlockTreeWalker.GetNextNonContainerBlock(block);

      if (nextBlock == null)
        return false;

      var nextCursor = nextBlock.GetCursor();
      nextCursor.MoveToBeginning();
      BlockCursor = nextCursor;

      return true;
    }

    /// <summary> Move the cursor backward in the document. </summary>
    /// <returns> True if the cursor was moved backward, false otherwise. </returns>
    public bool MoveBackward()
    {
      if (MoveBackwardInBlock())
        return true;

      var block = BlockCursor.Block;
      var previousBlock = BlockTreeWalker.GetPreviousNonContainerBlock(block);

      if (previousBlock == null)
        return false;

      var nextCursor = previousBlock.GetCursor();
      nextCursor.MoveToEnd();
      BlockCursor = nextCursor;

      return true;
    }
  }
}