using System;
using System.Collections.Generic;
using System.Linq;

namespace TextRight.Core.Cursors
{
  /// <summary> A caret for a specific kind of block. </summary>
  public interface IBlockCaret
  {
    /// <summary> True if the caret is pointing at the beginning of a block. </summary>
    bool IsAtBlockStart { get; }

    /// <summary> True if the caret is pointing at the end of a block. </summary>
    bool IsAtBlockEnd { get; }

    /// <summary> Converts this instance to a block caret. </summary>
    /// <returns> This instance as a BlockCaret. </returns>
    BlockCaret ToBlockCaret();
  }
}