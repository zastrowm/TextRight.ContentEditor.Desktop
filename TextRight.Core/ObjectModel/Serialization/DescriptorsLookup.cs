using System;
using System.Collections.Generic;
using System.Linq;
using TextRight.ContentEditor.Core.ObjectModel.Blocks;

namespace TextRight.ContentEditor.Core.ObjectModel.Serialization
{
  /// <summary> A lookup table of descriptors with their ids as a the key. </summary>
  public class DescriptorsLookup
  {
    private readonly Dictionary<string, BlockDescriptor> _descriptors
      = new Dictionary<string, BlockDescriptor>();

    public DescriptorsLookup(params BlockDescriptor[] descriptors)
    {
      foreach (var descriptor in descriptors)
      {
        AddDescriptor(descriptor);
      }
    }

    /// <summary> Adds a new descriptor that callers can use when inspecting blocks. </summary>
    public void AddDescriptor(BlockDescriptor descriptor)
    {
      _descriptors.Add(descriptor.Id, descriptor);
    }

    /// <summary> Retrieves the descriptor with the given id. </summary>
    public BlockDescriptor FindDescriptor(string id)
    {
      BlockDescriptor descriptor;
      if (!_descriptors.TryGetValue(id, out descriptor))
        throw new InvalidOperationException($"No descriptor with the given id: {id}");

      return descriptor;
    }
  }
}