using System;
using System.Collections.Generic;
using System.Linq;
using TextRight.Core.Editing.Commands;

namespace TextRight.Core.ObjectModel.Blocks.Collections
{
  /// <summary> The collection of blocks that exist at the top of the document. </summary>
  public class RootBlockCollection : VerticalBlockCollectionBase<IBlockCollectionView>
  {
    public static readonly RegisteredDescriptor RegisteredDescriptor
      = RegisteredDescriptor.Register<BlockDescriptor>();

    internal RootBlockCollection()
    {
    }

    /// <inheritdoc />
    public override RegisteredDescriptor Descriptor
      => RegisteredDescriptor;

    private class BlockDescriptor : FactoryBlockDescriptor<RootBlockCollection>
    {
      /// <inheritdoc />
      public override string Id
        => "RootCollection";

      /// <inheritdoc />
      public override Block CreateInstance()
        => new RootBlockCollection();

      /// <inheritdoc />
      public override IEnumerable<IContextualCommand> GetCommands(DocumentOwner document)
      {
        yield break;
      }
    }
  }
}