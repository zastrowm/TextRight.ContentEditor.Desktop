using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using TextRight.Core.ObjectModel.Blocks;
using TextRight.Core.Utilities;

namespace TextRight.Core.ObjectModel.Serialization
{
  /// <summary>
  ///  Generic implementation of <see cref="BaseBlockProperty"/> for ease of implementation in
  ///  derived classes.
  /// </summary>
  /// <remarks> Otherwise the implementations are nearly identical. </remarks>
  /// <typeparam name="T"> The type of data that the block property stores. </typeparam>
  internal abstract class SimpleValueBlockProperty<T> : BlockProperty<T>
  {
    private readonly Func<Block, T> _getter;
    private readonly Action<Block, T> _setter;

    internal SimpleValueBlockProperty(string id, PropertyInfo propertyInfo)
    {
      _getter = DelegateBuilder.BuildDelegate<Func<Block, T>>(propertyInfo.GetMethod);
      _setter = DelegateBuilder.BuildDelegate<Action<Block, T>>(propertyInfo.SetMethod);
      Id = id;
    }

    /// <inheritdoc />
    public override string Id { get; }

    /// <inheritdoc />
    public override T GetValue(Block block)
      => _getter.Invoke(block);

    public override void SetValue(Block block, T value)
      => _setter.Invoke(block, value);

    public override void Read(Block block, IDataReader reader)
      => SetValue(block, Read(reader));

    public override void Write(Block block, IDataWriter writer)
      => Write(writer, GetValue(block));

    public abstract T Read(IDataReader reader);
    public abstract void Write(IDataWriter writer, T value);
  }
}