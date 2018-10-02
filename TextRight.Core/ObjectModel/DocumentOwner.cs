using System;
using System.Collections.Generic;
using System.Linq;
using TextRight.Core.ObjectModel.Blocks.Collections;
using TextRight.Core.ObjectModel.Serialization;

namespace TextRight.Core.ObjectModel
{
  /// <summary> Represents a single TextRight document. </summary>
  public class DocumentOwner : IEquatable<DocumentOwner>
  {
    /// <summary> Default constructor. </summary>
    public DocumentOwner()
      : this((RootBlockCollection)RootBlockCollection.Descriptor.CreateInstance())
    {
    }

    /// <summary> Constructor. </summary>
    /// <param name="collection"> The collection that should be used as the root. </param>
    private DocumentOwner(RootBlockCollection collection)
    {
      Root = collection;
    }

    /// <summary> The top level collection of elements.  </summary>
    public RootBlockCollection Root { get; }

    /// <summary> Makes a deep copy of this instance. </summary>
    /// <returns> A copy of this instance. </returns>
    public DocumentOwner Clone()
    {
      return new DocumentOwner((RootBlockCollection)Root.Clone());
    }

    public SerializeNode SerializeAsNode()
    {
      var node = new SerializeNode("temp/document");

      node.Children.Add(Root.Serialize());

      return node;
    }

    /// <nodoc />
    public bool Equals(DocumentOwner other)
    {
      if (ReferenceEquals(null, other))
        return false;
      if (ReferenceEquals(this, other))
        return true;
      return Equals(Root, other.Root);
    }

    /// <inheritdoc />
    public override bool Equals(object obj)
    {
      if (ReferenceEquals(null, obj))
        return false;
      if (ReferenceEquals(this, obj))
        return true;
      if (obj.GetType() != GetType())
        return false;
      return Equals((DocumentOwner)obj);
    }

    /// <inheritdoc />
    public override int GetHashCode()
    {
      return Root?.GetHashCode() ?? 0;
    }
  }
}