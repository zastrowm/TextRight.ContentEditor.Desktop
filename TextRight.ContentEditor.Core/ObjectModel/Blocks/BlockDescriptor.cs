using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using TextRight.ContentEditor.Core.Editing;

namespace TextRight.ContentEditor.Core.ObjectModel.Blocks
{
  /// <summary>
  ///  Provides information about a specific type of block, allowing both creation and serialization.
  /// </summary>
  public abstract class BlockDescriptor
  {
    /// <summary> The unique id of the block. </summary>
    public abstract string Id { get; }

    /// <summary> Gets the type of the block that is described by this instance. </summary>
    public abstract Type BlockType { get; }

    /// <summary> Creates a new instance of the block. </summary>
    /// <param name="document"> The document for which the block is being created. </param>
    /// <returns> The new instance of the block. </returns>
    [Pure]
    public abstract Block CreateInstance(DocumentOwner document);

    /// <summary> All of the commands that should be available when the block is in a document. </summary>
    /// <param name="document"></param>
    public abstract IEnumerable<IContextualCommand> GetCommands(DocumentOwner document);
  }

  /// <summary> Generic version of <see cref="BlockDescriptor"/> </summary>
  /// <typeparam name="TBlock"> Type of the block being described. </typeparam>
  public abstract class BlockDescriptor<TBlock> : BlockDescriptor
    where TBlock : Block, new()
  {
    /// <inheritdoc />
    public override Block CreateInstance(DocumentOwner document)
      => new TBlock();

    /// <inheritdoc />
    public override Type BlockType
      => typeof(TBlock);
  }
}