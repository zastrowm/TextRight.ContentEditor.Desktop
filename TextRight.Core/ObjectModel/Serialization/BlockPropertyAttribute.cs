using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace TextRight.Core.ObjectModel.Serialization
{
  /// <summary> Holds a information about a TextBlock property that should be serialized. </summary>
  [AttributeUsage(AttributeTargets.Property)]
  public class BlockPropertyAttribute : Attribute
  {
    /// <summary> Constructor. </summary>
    /// <param name="name"> The name of the property when storing it. </param>
    public BlockPropertyAttribute(string name)
    {
      Name = name;
    }

    /// <summary> The name of the property when storing it. </summary>
    public string Name { get; }

    internal BaseBlockProperty GetBlockProperty(PropertyInfo info)
    {
      var type = info.PropertyType;

      if (type == typeof(int))
        return new IntBlockProperty(Name, info);
      if (type == typeof(long))
        return new LongBlockProperty(Name, info);
      if (type == typeof(double))
        return new DoubleBlockProperty(Name, info);
      if (type == typeof(bool))
        return new BoolBlockProperty(Name, info);
      if (type == typeof(string))
        return new StringBlockProperty(Name, info);

      throw new ArgumentException(
        $"Property named {info.Name} cannot have an attribute {nameof(BlockPropertyAttribute)} as there is no corresponding block property that can represent it.  Try implementing {nameof(ICustomSerialized)} to serialize it.",
        nameof(info));
    }
  }
}