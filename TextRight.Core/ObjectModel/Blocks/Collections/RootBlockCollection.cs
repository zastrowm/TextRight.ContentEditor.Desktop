using System;
using System.Collections.Generic;
using System.Linq;
using TextRight.Core.Commands;
using TextRight.Core.ObjectModel.Blocks.Text;

namespace TextRight.Core.ObjectModel.Blocks.Collections
{
  /// <summary> The collection of blocks that exist at the top of the document. </summary>
  public class RootBlockCollection : VerticalBlockCollectionBase<IBlockCollectionView>
  {
    public static readonly RootBlockCollectionDescriptor Descriptor
      = RegisteredDescriptors.Register<RootBlockCollectionDescriptor>();

    internal RootBlockCollection()
      : base(new ParagraphBlock())
    {
    }

    /// <inheritdoc />
    public override BlockDescriptor DescriptorHandle
      => Descriptor;

    public class RootBlockCollectionDescriptor : FactoryBlockDescriptor<RootBlockCollection>
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