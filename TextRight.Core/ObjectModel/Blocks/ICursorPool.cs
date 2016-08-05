using System;
using System.Collections.Generic;
using System.Linq;
using TextRight.ContentEditor.Core.ObjectModel.Cursors;

namespace TextRight.ContentEditor.Core.ObjectModel.Blocks
{
  /// <summary>
  ///  Pools cursors so that they do not need to be continually created and destroyed.
  /// </summary>
  public interface ICursorPool
  {
    /// <summary> Gets a cursor from the pool. </summary>
    /// <param name="block"> The block for which the cursor is valid. </param>
    /// <returns> The cursor from the pool. </returns>
    IBlockContentCursor Borrow(Block block);

    /// <summary> Puts a cursor back into the pool. </summary>
    /// <param name="cursor"> The cursor to put into the pool. </param>
    void Recycle(IBlockContentCursor cursor);

    /// <summary> Gets a cursor copy that can be disposed to recycle the cursor. </summary>
    /// <param name="block"> The block for which the cursor is valid. </param>
    CursorCopy GetCursorCopy(Block block);
  }
}