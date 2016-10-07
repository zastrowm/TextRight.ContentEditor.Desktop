using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TextRight.Core.ObjectModel.Blocks;

namespace TextRight.Core.ObjectModel.Serialization
{
  /// <summary>
  ///  A simple data structure that contains attributes and children of an element in the document.
  ///  Used for easily, but inefficiently, serializing the document.
  /// </summary>
  public class SerializeNode
  {
    private readonly Dictionary<string, string> _attributes;

    /// <summary> Constructor. </summary>
    public SerializeNode(string typeId)
    {
      TypeId = typeId;
      Children = new List<SerializeNode>();
      _attributes = new Dictionary<string, string>();
    }

    /// <summary> Constructor. </summary>
    public SerializeNode(RegisteredDescriptor descriptor)
      : this(descriptor.Descriptor.Id)
    {
    }

    /// <summary>
    ///  A human readable id of what the node represents, like "heading", "paragraph", or "list-item".
    /// </summary>
    public string TypeId { get; private set; }

    /// <summary> The children of the serialized node. </summary>
    public List<SerializeNode> Children { get; }

    /// <summary> Any attributes to associated with the node. </summary>
    public IReadOnlyDictionary<string, string> Attributes
      => _attributes;

    /// <summary> Adds a piece of data to the node. </summary>
    /// <typeparam name="T"> The type of the data to add. </typeparam>
    /// <param name="name"> The name of the piece of data being added. </param>
    /// <param name="value"> The value of the data to add. </param>
    public void AddData<T>(string name, T value)
    {
      _attributes[name] = Convert.ToString(value);
    }

    /// <summary> Gets a specific piece of data from the node. </summary>
    /// <typeparam name="T"> The type of data being retrieved. </typeparam>
    /// <param name="name"> The name of the data being retrieved. </param>
    /// <returns>
    ///  The typed-data that was stored in the node, or default(T) if the given named-data was not
    ///  present.
    /// </returns>
    public T GetDataOrDefault<T>(string name)
    {
      string strValue;
      if (_attributes.TryGetValue(name, out strValue))
      {
        return (T)Convert.ChangeType(strValue, typeof(T));
      }

      return default(T);
    }
  }
}