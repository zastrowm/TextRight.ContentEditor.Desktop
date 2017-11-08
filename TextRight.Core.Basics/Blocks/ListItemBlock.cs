using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TextRight.Core.Commands;
using TextRight.Core.ObjectModel;
using TextRight.Core.ObjectModel.Blocks;
using TextRight.Core.ObjectModel.Blocks.Collections;
using TextRight.Core.ObjectModel.Blocks.Text;

namespace TextRight.Core.Blocks
{
  public interface IListItemBlockView : IBlockCollectionView
  {
  }

  public class ListItemBlock : VerticalBlockCollectionBase<IListItemBlockView>
  {
    public static readonly RegisteredDescriptor Descriptor
      = RegisteredDescriptor.Register<ListItemBlockDescriptor>();

    public override RegisteredDescriptor DescriptorHandle
      => Descriptor;
    
    public ListItemBlock()
      : base(new ParagraphBlock())
    {
    }

    public class ListItemBlockDescriptor : BlockDescriptor<ListItemBlock>
    {
      public override string Id
        => "listitem";

      public override IEnumerable<IContextualCommand> GetCommands(DocumentOwner document)
      {
        yield break;
      }
    }
  }
}