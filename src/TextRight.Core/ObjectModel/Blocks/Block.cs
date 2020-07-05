using System;
using System.Collections.Generic;
using System.Linq;
using TextRight.Core.Cursors;
using TextRight.Core.ObjectModel.Blocks.Collections;
using TextRight.Core.ObjectModel.Cursors;
using TextRight.Core.ObjectModel.Serialization;
using TextRight.Core.Utilities;

namespace TextRight.Core.ObjectModel.Blocks
{
  /// <summary>
  ///  Represents the base class for both <see cref="BlockCollection"/> and
  ///  <see cref="ContentBlock"/>.
  /// </summary>
  public abstract class Block : DocumentItem
  {
    /// <summary> Default constructor. </summary>
    internal Block()
    {
    }

    /// <summary>
    ///   The index of the block within the parent's collection.
    /// </summary>
    public int Index { get; set; }

    /// <summary>
    ///   The block that owns this block.
    /// </summary>
    public BlockCollection Parent { get; set; }

    /// <inheritdoc />
    protected override EventEmitter ParentEmitter
      => Parent;

    /// <summary> True if the block is the first child of the parent collection. </summary>
    public bool IsFirst
      => Parent == null || Index == 0;

    /// <summary> True if the block is the last child of the parent collection. </summary>
    public bool IsLast
      => Parent == null || GetNextBlock() == null;

    /// <summary> Get the previous block in the block collection. </summary>
    public Block GetPreviousBlock()
      => PreviousBlock;

    /// <summary> Get the next block in the block collection. </summary>
    public Block GetNextBlock()
      => NextBlock;

    /// <summary> Get the path down to this block in the document. </summary>
    /// <returns> The path to the block in the hierarchy. </returns>
    public BlockPath GetBlockPath() 
      => new BlockPath(this);

    /// <summary> Gets a handle to the descriptor for this specific block type. </summary>
    public abstract BlockDescriptor DescriptorHandle { get; }

    /// <summary> Makes a deep copy of this instance. </summary>
    /// <returns> A copy of this instance. </returns>
    public abstract Block Clone();

    /// <summary> Serializes the given block into a SerializedNode. </summary>
    /// <returns> A SerializeNode that represents the contents in the given block. </returns>
    public SerializeNode Serialize()
    {
      var node = new SerializeNode(DescriptorHandle);
      SerializeInto(node);
      return node;
    }

    /// <summary> Saves the state of the current instance into <paramref name="node"/>. </summary>
    /// <param name="node"> The node into which the state of this block should be stored. </param>
    protected abstract void SerializeInto(SerializeNode node);

    /// <summary> Loads the state from <paramref name="node"/> into the current instance. </summary>
    /// <param name="context"> The context in which the node is being deserialized. </param>
    /// <param name="node"> The node that contains the data that should be loaded into this block. </param>
    public abstract void Deserialize(SerializationContext context, SerializeNode node);

    /// <summary>
    ///  Retrieves the block that comes after this block in the parent collection.
    /// </summary>
    internal Block NextBlock { get; set; }

    /// <summary>
    ///  Retrieves the block that comes before this block in the parent collection.
    /// </summary>
    internal Block PreviousBlock { get; set; }
  }
}