﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace TextRight.Core.Cursors
{
  /// <summary> Moves a specific type of caret through a block. </summary>
  public interface ICaretMover
  {
    /// <summary> Moves the BlockCaret through the block to the next unit. </summary>
    /// <param name="caret"> The caret to move. The <see cref="BlockCaret.Mover"/> of
    ///  <paramref name="caret"/> should be the same instance whose method is being invoked. </param>
    /// <returns>
    ///  A BlockCaret that represents the position one unit forward from <paramref name="caret"/>, or
    ///  <see cref="BlockCaret.Invalid"/> if the caret could not be moved forward in the block.
    /// </returns>
    BlockCaret MoveForward(BlockCaret caret);

    /// <summary> Moves the BlockCaret through the block to the previous unit. </summary>
    /// <param name="caret"> The caret to move. The <see cref="BlockCaret.Mover"/> of
    ///  <paramref name="caret"/> should be the same instance whose method is being invoked. </param>
    /// <returns>
    ///  A BlockCaret that represents the position one unit backward from <paramref name="caret"/>, or
    ///  <see cref="BlockCaret.Invalid"/> if the caret could not be moved backward in the block.
    /// </returns>
    BlockCaret MoveBackward(BlockCaret caret);

    /// <summary> Query if 'caret' is at the beginning of the current content. </summary>
    /// <param name="caret"> The caret to move. The <see cref="BlockCaret.Mover"/> of
    ///  <paramref name="caret"/> should be the same instance whose method is being invoked. </param>
    /// <returns> True if the caret is at the start of the content, false otherwise. </returns>
    bool IsAtBlockStart(BlockCaret caret);

    /// <summary> Query if 'caret' is at the end of the current content. </summary>
    /// <param name="caret"> The caret to move. The <see cref="BlockCaret.Mover"/> of
    ///  <paramref name="caret"/> should be the same instance whose method is being invoked. </param>
    /// <returns> True if the caret is at the end of the content, false otherwise. </returns>
    bool IsAtBlockEnd(BlockCaret caret);
  }
}