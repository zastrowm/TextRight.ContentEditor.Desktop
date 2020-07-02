using System;
using System.Collections.Generic;
using System.Linq;
using TextRight.Core.ObjectModel.Blocks;
using TextRight.Core.Utilities;

namespace TextRight.Core.Cursors
{
  /// <summary>
  ///  A caret care which conforms to the contract required for a <see cref="ICaretMover"/> to
  ///  derive from <see cref="SimpleCaretMover{TCaret,TBlock}"/>.
  ///  
  ///  You should not implement any of this interface implicitly, as it's simply a way to more
  ///  simply create instances of <see cref="ICaretMover{TCaret}"/> and to unify the contract that
  ///  most Caret classes expose.
  /// </summary>
  /// <typeparam name="TCaret"> Type of the caret. </typeparam>
  /// <typeparam name="TBlock"> Type of the block. </typeparam>
  public interface ISimpleCaret<TCaret, TBlock> : IBlockCaret, IEquatable<TCaret>
    where TCaret : IBlockCaret, IEquatable<TCaret>
    where TBlock : ContentBlock
  {
    /// <summary> Gets the next possible place for the caret.  </summary>
    TCaret GetNextPosition();

    /// <summary> Gets the previous possible place for the caret. </summary>
    TCaret GetPreviousPosition();

    /// <summary> The block that this cursor is associated with. </summary>
    TBlock Block { get; }

    /// <summary>
    ///  True if the cursor represents a valid location in the block, false if it does not.
    /// </summary>
    bool IsValid { get; }

    /// <summary> Gets an object that holds the serialized data for this caret. </summary>
    ISerializedBlockCaret Serialize();
  }
}