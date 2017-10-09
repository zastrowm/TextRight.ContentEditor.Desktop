using System;
using System.Collections.Generic;
using System.Linq;
using TextRight.Core.ObjectModel.Blocks;
using TextRight.Core.ObjectModel.Blocks.Text;
using TextRight.Core.Utilities;

namespace TextRight.Core.Cursors
{
  /// <summary> Represents a generic caret that is pointing to content within a block. </summary>
  public struct BlockCaret : IEquatable<BlockCaret>
    // we want it to have the same api surface as other block carets, but we don't really
    // want it to be an IBlockCaret
#if DEBUG
    , IBlockCaret
#endif

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
      => Mover.IsAtBlockEnd(this);

#if DEBUG

    /// <inheritdoc />
    public BlockCaret ToBlockCaret()
      => this;

#endif

    public BlockCaret MoveForward()
      => Mover.MoveForward(this);

    public BlockCaret MoveBackward()
      => Mover.MoveBackward(this);

    public MeasuredRectangle Measure()
      => Mover.Measure(this);

    public ISerializedBlockCaret Serialize()
      => Mover.Serialize(this);

    /// <summary> True if the BlockCaret is pointing at a potentially valid location. </summary>
    public bool IsValid
      => Mover != null;

    /// <summary> True if the caret is of the specified type. </summary>
    public bool Is<TCaret>()
      where TCaret : struct, IEquatable<TCaret>, IBlockCaret
      => Mover is ICaretMover<TCaret>;

    /// <summary> Cast the caret into the given caret type. </summary>
    public TCaret As<TCaret>()
      where TCaret : struct, IEquatable<TCaret>, IBlockCaret
    {
      if (Mover is ICaretMover<TCaret> typedMover)
      {
        return typedMover.Convert(this);
      }

      throw new ArgumentException($"Block caret does not represent a cursor of type: {typeof(TCaret)}");
    }

    public bool TryCast<TCaret>(out TCaret caret) 
      where TCaret : struct, IBlockCaret, IEquatable<TCaret>
    {
      if (Mover is ICaretMover<TCaret> typedMover)
      {
        caret = typedMover.Convert(this);
        return true;
      }

      caret = default(TCaret);
      return false;
    }

    /// <summary> The block associated with this caret. </summary>
    public ContentBlock Block
      => Mover?.GetBlock(this);

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

    /// <summary />
    public bool Equals(BlockCaret other) =>
      Equals(Mover, other.Mover) && Equals(InstanceDatum, other.InstanceDatum) &&
      InstanceOffset1 == other.InstanceOffset1 &&
      InstanceOffset2 == other.InstanceOffset2 &&
      InstanceOffset3 == other.InstanceOffset3 &&
      InstanceOffset4 == other.InstanceOffset4;

    /// <inheritdoc />
    public override bool Equals(object obj)
    {
      if (ReferenceEquals(null, obj))
        return false;

      return obj is BlockCaret && Equals((BlockCaret)obj);
    }

    /// <inheritdoc />
    public override int GetHashCode()
    {
      unchecked
      {
        var hashCode = (Mover != null ? Mover.GetHashCode() : 0);
        hashCode = (hashCode * 397) ^ (InstanceDatum != null ? InstanceDatum.GetHashCode() : 0);
        hashCode = (hashCode * 397) ^ InstanceOffset1;
        hashCode = (hashCode * 397) ^ InstanceOffset2;
        hashCode = (hashCode * 397) ^ InstanceOffset3;
        hashCode = (hashCode * 397) ^ InstanceOffset4;
        return hashCode;
      }
    }

    /// <summary />
    public static bool operator ==(BlockCaret left, BlockCaret right)
      => left.Equals(right);

    /// <summary />
    public static bool operator !=(BlockCaret left, BlockCaret right)
      => !left.Equals(right);
  }
}