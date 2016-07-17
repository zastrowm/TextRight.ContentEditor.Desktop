using System;
using System.Collections.Generic;
using System.Linq;
using TextRight.ContentEditor.Core.Editing;
using TextRight.ContentEditor.Core.ObjectModel.Serialization;

namespace TextRight.ContentEditor.Core.ObjectModel.Blocks
{
  /// <summary> A block that holds text formatted as a paragraph. </summary>
  public sealed class ParagraphBlock : TextBlockBase<ITextBlockView>
  {
    /// <summary> Singleton-Instance of a descriptor. </summary>
    public static readonly RegisteredDescriptor RegisteredDescriptor
      = RegisteredDescriptor.Register<BlockDescriptor>();

    /// <inheritdoc />
    public override RegisteredDescriptor Descriptor
      => RegisteredDescriptor;

    /// <inheritdoc/>
    protected override TextBlock SuperClone()
    {
      return new ParagraphBlock();
    }

    /// <inheritdoc/>
    protected override void SerializeToNode(SerializeNode node)
    {
      // no-op
    }

    /// <inheritdoc/>
    public override TextBlockAttributes GetAttributes()
    {
      return new Attributes();
    }

    /// <summary />
    private class Attributes : TextBlockAttributes
    {
      public override TextBlock CreateInstance()
      {
        return new ParagraphBlock();
      }
    }

    /// <summary> Describes the <see cref="ParagraphBlock"/> block. </summary>
    private class BlockDescriptor : ContentBlockDescriptor<ParagraphBlock>
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
  }
}