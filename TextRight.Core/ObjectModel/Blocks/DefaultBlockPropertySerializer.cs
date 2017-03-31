using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using TextRight.Core.ObjectModel.Serialization;

namespace TextRight.Core.ObjectModel.Blocks
{
  /// <summary>
  ///  Serializes properties annotated with a <see cref="BlockPropertyAttribute"/> of a text block.
  /// </summary>
  internal class DefaultBlockPropertySerializer
  {
    private readonly List<BaseBlockProperty> _properties;

    /// <summary> Constructor. </summary>
    /// <param name="type"> The type from which to pull the properties from. </param>
    internal DefaultBlockPropertySerializer(IReflect type)
    {
      var propertiesToSerialize =
        from property in type.GetProperties(BindingFlags.GetProperty | BindingFlags.Public | BindingFlags.Instance)
        let attribute = property.GetCustomAttribute<BlockPropertyAttribute>()
        where attribute != null
        select new { attribute, property };

      _properties = new List<BaseBlockProperty>();

      foreach (var item in propertiesToSerialize)
      {
        var blockProperty = item.attribute.GetBlockProperty(item.property);
        _properties.Add(blockProperty);
      }
    }

    public void Read(Block block, IDataReader reader)
    {
      foreach (var property in _properties)
      {
        property.Read(block, reader);
      }
    }

    public void Write(Block block, IDataWriter writer)
    {
      foreach (var property in _properties)
      {
        property.Write(block, writer);
      }
    }
  }
}