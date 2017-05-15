using System;
using System.Collections.Generic;
using System.Linq;

namespace TextRight.Core.Cursors
{
  /// <summary> Represents a generic caret that is pointing to content within a block. </summary>
  public struct BlockCaret : IBlockCaret
  {
    public static readonly BlockCaret Invalid
      = default(BlockCaret);

    /// <summary> Constructor. </summary>
    /// <param name="mover"> The type-specific mover associated with the caret. </param>
    /// <param name="instanceDatum"> Any instance data that should be stored with the caret. </param>
    /// <param name="instanceOffset1"> (Optional) Any instance data that should be stored with the
    ///  caret. </param>
    /// <param name="instanceOffset2"> (Optional) Any instance data that should be stored with the
    ///  caret. </param>
    /// <param name="instanceOffset3"> (Optional) Any instance data that should be stored with the
    ///  caret. </param>
    /// <param name="instanceOffset4"> (Optional) Any instance data that should be stored with the
    ///  caret. </param>
    public BlockCaret(ICaretMover mover,
                      object instanceDatum,
                      int instanceOffset1 = 0,
                      int instanceOffset2 = 0,
                      int instanceOffset3 = 0,
                      int instanceOffset4 = 0)
      : this()
    {
      InstanceDatum = instanceDatum;
      Mover = mover;
      InstanceOffset1 = instanceOffset1;
      InstanceOffset2 = instanceOffset2;
      InstanceOffset3 = instanceOffset3;
      InstanceOffset4 = instanceOffset4;
    }

    /// <inheritdoc />
    public bool IsAtBlockStart
      => Mover.IsAtBlockStart(this);

    /// <inheritdoc />
    public bool IsAtBlockEnd
      => Mover.IsAtBlockStart(this);

    /// <inheritdoc />
    public BlockCaret ToBlockCaret()
      => this;

    /// <summary> True if the caret is of the specified type. </summary>
    public bool Is<TCaret>()
      where TCaret : struct, IEquatable<TCaret>, IBlockCaret 
      => Mover is ICaretMover<TCaret>;

    /// <summary> The type-specific mover associated with the caret. </summary>
    public ICaretMover Mover { get; }

    /// <summary> Any instance data that should be stored with the caret. </summary>
    public object InstanceDatum { get; }

    /// <summary> Arbitrary data to be stored in the caret. </summary>
    public int InstanceOffset1 { get; }

    /// <summary> Arbitrary data to be stored in the caret. </summary>
    public int InstanceOffset2 { get; }

    /// <summary> Arbitrary data to be stored in the caret. </summary>
    public int InstanceOffset3 { get; }

    /// <summary> Arbitrary data to be stored in the caret. </summary>
    public int InstanceOffset4 { get; }
  }
}