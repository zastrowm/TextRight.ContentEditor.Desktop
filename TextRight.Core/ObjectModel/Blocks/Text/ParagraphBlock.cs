using System;
using System.Collections.Generic;
using System.Linq;
using TextRight.Core.Commands;
using TextRight.Core.Utilities;

namespace TextRight.Core.ObjectModel.Blocks.Text
{
  /// <summary> A block that holds text formatted as a paragraph. </summary>
  public sealed class ParagraphBlock : TextBlock, IDocumentItem<IContentBlockView>
  {
    /// <summary> Singleton-Instance of a descriptor. </summary>
    public static readonly ParagraphBlockDescriptor Descriptor
      = RegisteredDescriptors.Register<ParagraphBlockDescriptor>();

    /// <inheritdoc />
    public override BlockDescriptor DescriptorHandle
      => Descriptor;

    /// <summary> Describes the <see cref="ParagraphBlock"/> block. </summary>
    public class ParagraphBlockDescriptor : BlockDescriptor<ParagraphBlock>
    {
      /// <inheritdoc />
      public override string Id
        => "paragraph";

      /// <param name="document"></param>
      /// <inheritdoc/>
      public override IEnumerable<IContextualCommand> GetCommands(DocumentOwner document)
      {
        yield return new ConvertParagraphCommand();
      }
    }

    /// <inheritdoc/>
    public IContentBlockView Target { get; set; }

    /// <inheritdoc/>
    protected override IContentBlockView ContentBlockView
      => Target;
  }
}