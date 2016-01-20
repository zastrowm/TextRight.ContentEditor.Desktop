using System;
using System.Collections.Generic;
using System.Linq;
using TextRight.ContentEditor.Core.ObjectModel.Blocks;

namespace TextRight.ContentEditor.Core.Editing
{
  /// <summary> Represents the context for a block command. </summary>
  public class CommandExecutionContext
  {
    private readonly List<Block> _blockHistory = new List<Block>();

    /// <summary> Gets the current block. </summary>
    public Block CurrentBlock { get; private set; }

    /// <summary>
    ///  Configure the context for the given cursor and block that belongs to the
    ///  cursor.  This method exists to allow this classes to be pooled.
    /// </summary>
    internal void ConfigureFor(IBlockContentCursor cursor)
    {
      if (cursor == null)
        throw new ArgumentNullException(nameof(cursor));

      var block = cursor.Block;
      if (block == null)
        throw new ArgumentNullException(nameof(block));

      _blockHistory.Clear();

      CurrentBlock = block;
      _blockHistory.Add(CurrentBlock);
    }

    /// <summary>
    ///  Gets the Nth previous block.  If index is zero, it retrieves the current
    ///  item.
    /// </summary>
    /// <param name="index"> Zero-based index of the previous block to return. </param>
    /// <returns>
    ///  The block that is <paramref name="index"/> levels deep below the current
    ///  Block or null if there is no block that deep in history.
    /// </returns>
    public Block GetChildBlock(int index)
    {
      if (index < 0 || index >= _blockHistory.Count)
        return null;

      return _blockHistory[_blockHistory.Count - index - 1];
    }

    /// <summary>
    ///  Moves to the next block higher in the hierarchy, adding the current block
    ///  to the history queue so that it can be retrieved by
    ///  <see cref="GetChildBlock"/>
    /// </summary>
    /// <returns>
    ///  True if the CurrentBlock was changed, false if it was not because there
    ///  was no parent to navigate to.
    /// </returns>
    internal bool MoveUp()
    {
      if (CurrentBlock.Parent == null)
        return false;

      CurrentBlock = CurrentBlock.Parent;
      _blockHistory.Add(CurrentBlock);

      return true;
    }
  }
}