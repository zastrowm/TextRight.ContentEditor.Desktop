using System;
using System.Collections.Generic;
using System.Linq;
using TextRight.Core.ObjectModel.Blocks;
using TextRight.Core.Utilities;

namespace TextRight.Core.Cursors
{
  /// <summary>
  ///  Implementation of <see cref="ICaretMover{TCaret}"/> and <see cref="ICaretMover"/> for carets
  ///  that implement <see cref="ISimpleCaret{TCaret,TBlock}"/>.
  /// </summary>
  /// <typeparam name="TCaret"> The type of the caret being moved. </typeparam>
  /// <typeparam name="TBlock"> Type of the block the caret belongs to. </typeparam>
  public abstract class SimpleCaretMover<TCaret, TBlock> : ICaretMover<TCaret>
    where TCaret : struct, ISimpleCaret<TCaret, TBlock>, IEquatable<TCaret>
    where TBlock : ContentBlock
  {
    /// <summary>
    ///  Unpackages a <see cref="BlockCaret"/> into the corresponding <see cref="TCaret"/> type.
    /// </summary>
    /// <param name="caret"> The caret to unpack. </param>
    public abstract TCaret FromBlockCaret(BlockCaret caret);

    /// <inheritdoc />
    public TCaret Convert(BlockCaret caret)
      => FromBlockCaret(caret);

    /// <inheritdoc />
    public BlockCaret MoveForward(BlockCaret caret)
      => FromBlockCaret(caret).GetNextPosition().ToBlockCaret();

    /// <inheritdoc />
    public BlockCaret MoveBackward(BlockCaret caret)
      => FromBlockCaret(caret).GetPreviousPosition().ToBlockCaret();

    /// <inheritdoc />
    /// <inheritdoc />
    public bool IsAtBlockEnd(BlockCaret caret)
      => FromBlockCaret(caret).IsAtBlockEnd;

    /// <inheritdoc />
    public ContentBlock GetBlock(BlockCaret blockCaret)
      => FromBlockCaret(blockCaret).Block;

    /// <inheritdoc />
    public bool IsAtBlockStart(BlockCaret caret)
      => FromBlockCaret(caret).IsAtBlockStart;

    /// <inheritdoc />
    public ISerializedBlockCaret Serialize(BlockCaret caret)
      => FromBlockCaret(caret).Serialize();
  }
}