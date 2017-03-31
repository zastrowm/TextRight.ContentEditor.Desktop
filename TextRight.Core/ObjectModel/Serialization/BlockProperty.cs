using System;
using System.Collections.Generic;
using System.Linq;
using TextRight.Core.ObjectModel.Blocks;

namespace TextRight.Core.ObjectModel.Serialization
{
  /// <summary>
  ///  Represents a property of a block that should be serialized/deserialized when the block is
  ///  saved.
  /// </summary>
  /// <typeparam name="T"> The type of data that the property holds. </typeparam>
  /// <remarks> TODO expose this to consumers? </remarks>
  internal abstract class BlockProperty<T> : BaseBlockProperty
  {
    /// <summary> Default constructor. </summary>
    internal BlockProperty()
    {
    }

    public abstract string Id { get; }

    public abstract T GetValue(Block block);

    public abstract void SetValue(Block block, T value);
  }
}