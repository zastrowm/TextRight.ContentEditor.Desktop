using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using JetBrains.Annotations;
using TextRight.Core.Commands;
using TextRight.Core.Events;

namespace TextRight.Core.ObjectModel.Blocks
{
  /// <summary>
  ///  Provides information about a specific type of block, allowing both creation and serialization.
  /// </summary>
  public abstract class BlockDescriptor
  {
    /// <summary> All of the properties registered to this descriptor. </summary>
    public PropertyDescriptorCollection Properties { get; }
      = new PropertyDescriptorCollection();

    /// <summary> The unique id of the block. </summary>
    public abstract string Id { get; }

    /// <summary> Gets the type of the block that is described by this instance. </summary>
    public abstract Type BlockType { get; }

    /// <summary> Creates a new instance of the block. </summary>
    /// <returns> The new instance of the block. </returns>
    [Pure]
    public abstract Block CreateInstance();

    /// <summary> All of the commands that should be available when the block is in a document. </summary>
    /// <param name="document"></param>
    public abstract IEnumerable<IContextualCommand> GetCommands(DocumentOwner document);
  }

  /// <summary> Generic version of <see cref="BlockDescriptor"/> </summary>
  /// <typeparam name="TBlock"> Type of the block being described. </typeparam>
  public abstract class FactoryBlockDescriptor<TBlock> : BlockDescriptor
    where TBlock : Block
  {
    /// <inheritdoc />
    public override Type BlockType
      => typeof(TBlock);
  }

  /// <summary> Generic version of <see cref="BlockDescriptor"/> </summary>
  /// <typeparam name="TBlock"> Type of the block being described. </typeparam>
  public abstract class BlockDescriptor<TBlock> : FactoryBlockDescriptor<TBlock>
    where TBlock : Block, new()
  {
    /// <inheritdoc />
    public override Block CreateInstance()
      => new TBlock();

    /// <summary> Registers the given properties with <see cref="BlockDescriptor.Properties"/>. </summary>
    public IPropertyDescriptor<T> RegisterProperty<T>(Expression<Func<TBlock, T>> propertyGetter, string id)
      => Properties.RegisterProperty(propertyGetter, id);
  }
}