using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TextRight.ContentEditor.Core.ObjectModel.Serialization
{
  /// <summary>
  ///  A simple data structure that contains attributes and children of an element in the document.
  ///  Used for easily, but inefficiently, serializing the document.
  /// </summary>
  public class SerializeNode
  {
    /// <summary> Constructor. </summary>
    /// <param name="type"> The type of object being serialized.. </param>
    public SerializeNode(Type type)
    {
      Type = type.Name;
      Children = new List<SerializeNode>();
    }

    /// <summary> The type of object serialized. </summary>
    public string Type { get; }

    /// <summary> The children of the serialized node. </summary>
    public List<SerializeNode> Children { get; }

    /// <summary> Any data that needs to be associated with the node. </summary>
    public string Data { get; set; }
  }
}