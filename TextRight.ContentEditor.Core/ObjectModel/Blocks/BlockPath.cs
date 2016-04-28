using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using JetBrains.Annotations;

namespace TextRight.ContentEditor.Core.ObjectModel.Blocks
{
  /// <summary>
  ///  An abstract way to identify a block within the hierarchy of a Document.  Only valid until
  ///  blocks are added or removed.
  /// </summary>
  [DebuggerDisplay("{ToDebugString(),nq}")]
  public struct BlockPath
  {
    /// <summary> The ids of the block in the hierarchy </summary>
    private readonly int[] _ids;

    /// <summary> Constructor. </summary>
    /// <param name="block"> The block for which the path should be created. </param>
    public BlockPath(Block block)
    {
      var ids = new List<int>();

      while (block?.Parent != null)
      {
        ids.Add(block.Index);
        block = block.Parent;
      }

      _ids = ids.ToArray();
    }

    /// <summary> Gets the block in the given document that this path represents. </summary>
    /// <param name="owner"> The owner that contains the block that should be retrieved. </param>
    /// <returns> The block that the BlockPath represents. </returns>
    [Pure]
    public Block Get(DocumentOwner owner)
    {
      if (_ids == null)
        return null;

      BlockCollection collection = owner.Root;
      Block block = null;

      for (int i = _ids.Length - 1; i >= 0; i--)
      {
        int blockIndex = _ids[0];

        Debug.Assert(collection != null);
        block = collection.GetNthBlock(blockIndex);
        collection = block as BlockCollection;
      }

      return block;
    }

    [Pure]
    internal string ToDebugString()
    {
      return string.Join(",", _ids.Reverse());
    }
  }
}