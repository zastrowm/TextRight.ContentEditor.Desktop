using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using TextRight.Core.ObjectModel.Serialization;

namespace TextRight.Editor.Wpf.Serialization
{
  /// <summary>
  ///  Provides methods for converting to and from <see cref="SerializeNode"/> and
  ///  <see cref="XElement"/>.
  /// </summary>
  public class XmlNodeSerializer
  {
    private const string TypeIdAttributeName = "type";
    private const string AttributeElementName = "Attribute";
    private const string ValueAttributeName = "value";
    private const string KeyAttributeName = "key";
    private const string NodeElementName = "Block";

    /// <summary> Converts a SerializeNode into an XElement </summary>
    public static XElement Serialize(SerializeNode node)
    {
      var childNodes = node.Children.Select(Serialize);
      var attributeNodes = node.Attributes.Select(kvp => new XElement(AttributeElementName,
                                                                      new XAttribute(KeyAttributeName, kvp.Key),
                                                                      new XAttribute(ValueAttributeName, kvp.Value)));
      var root = new XElement(NodeElementName,
                              childNodes,
                              attributeNodes,
                              new XAttribute(TypeIdAttributeName, node.TypeId));
      return root;
    }

    /// <summary> Converts the XElement into a SerializeNode </summary>
    public static SerializeNode Deserialize(XElement root)
    {
      // ReSharper disable PossibleNullReferenceException
      var typeId = root.Attribute(TypeIdAttributeName).Value;

      var node = new SerializeNode(typeId);

      // recreate each element
      foreach (var attributeElement in root.Elements(AttributeElementName))
      {
        node.AddData(attributeElement.Attribute(KeyAttributeName).Value,
                     attributeElement.Attribute(ValueAttributeName).Value);
      }

      // recreate each block
      foreach (var childElement in root.Elements(NodeElementName))
      {
        node.Children.Add(Deserialize(childElement));
      }

      return node;
      // ReSharper restore PossibleNullReferenceException
    }
  }
}