using System;
using System.Collections.Generic;
using System.Linq;
using TextRight.Core.ObjectModel.Blocks;

namespace TextRight.Core.ObjectModel.Serialization
{
  /// <summary> The context in which a node is being deserialized. </summary>
  public class SerializationContext
  {
    private readonly DescriptorsLookup _descriptors;

    public SerializationContext(DescriptorsLookup lookup)
    {
      _descriptors = lookup;
    }

    /// <summary> Retrieves the descriptor with the given id. </summary>
    public BlockDescriptor FindDescriptor(string id)
    {
      return _descriptors.FindDescriptor(id);
    }

    /// <summary> Deserializes the given node into the block that the node represents. </summary>
    /// <param name="node"> The node that represents the block that is going to be deserialized. </param>
    /// <returns> A newly created block that represents the node that was deserialized. </returns>
    public Block Deserialize(SerializeNode node)
    {
      var childDescriptor = FindDescriptor(node.TypeId);
      var block = childDescriptor.CreateInstance();
      block.Deserialize(this, node);
      return block;
    }
  }
}