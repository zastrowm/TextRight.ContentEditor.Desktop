using System;
using System.Collections.Generic;
using System.Linq;
using TextRight.ContentEditor.Desktop.ObjectModel.Blocks;

namespace TextRight.ContentEditor.Desktop.Blocks
{
  /// <summary> Holds a specific spot in the document. </summary>
  public class DocumentCursor
  {
    private IBlockContentCursor _blockCursor;

    /// <summary> Constructor. </summary>
    /// <param name="owner"> The Document that owns the given cursor. </param>
    /// <param name="blockCursor"> The block cursor. </param>
    public DocumentCursor(DocumentOwner owner, IBlockContentCursor blockCursor)
    {
      Owner = owner;
      _blockCursor = blockCursor;
    }

    /// <summary> The Document that owns the given cursor. </summary>
    public DocumentOwner Owner { get; }

    /// <summary> Moves the cursor forward within its current block. </summary>
    /// <returns> True if the cursor moved forward, false if it did not move forward. </returns>
    public bool MoveForwardInBlock()
      => _blockCursor.MoveForward();

    /// <summary> Moves the cursor backward within its current block. </summary>
    /// <returns> True if the cursor moved backward false if it did not move backward. </returns>
    public bool MoveBackwardInBlock()
      => _blockCursor.MoveBackward();

    /// <summary> Move the cursor forward in the document. </summary>
    /// <returns> True if the cursor was moved forward, false otherwise. </returns>
    public bool MoveForward()
    {
      if (MoveForwardInBlock())
        return true;

      var block = _blockCursor.Block;
      var nextBlock = BlockTreeWalker.GetNextNonContainerBlock(block);

      if (nextBlock == null)
        return false;

      var nextCursor = nextBlock.GetCursor();
      nextCursor.MoveToBeginning();
      _blockCursor = nextCursor;

      return true;
    }

    /// <summary> Move the cursor backward in the document. </summary>
    /// <returns> True if the cursor was moved backward, false otherwise. </returns>
    public bool MoveBackward()
    {
      if (MoveBackwardInBlock())
        return true;

      var block = _blockCursor.Block;
      var previousBlock = BlockTreeWalker.GetPreviousNonContainerBlock(block);

      if (previousBlock == null)
        return false;

      var nextCursor = previousBlock.GetCursor();
      nextCursor.MoveToBeginning();
      _blockCursor = nextCursor;

      return true;
    }

    // TODO this should be somewhere else
    /// <summary> Inserts text at the given cursor location. </summary>
    /// <param name="text"> The text to insert. </param>
    public void InsertText(string text)
    {
      _blockCursor.InsertText(text);
    }
  }
}