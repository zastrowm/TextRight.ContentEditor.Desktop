using System;
using System.Collections.Generic;
using System.Linq;
using TextRight.ContentEditor.Core.Editing;

namespace TextRight.ContentEditor.Core.ObjectModel.Blocks
{
  /// <summary> Describes the <see cref="ParagraphBlock"/> block. </summary>
  public class ParagraphBlockDescriptor : ContentBlockDescriptor<ParagraphBlock>
  {
    /// <inheritdoc />
    public override string Id
      => "paragraph";

    /// <param name="document"></param>
    /// <inheritdoc/>
    public override IEnumerable<IContextualCommand> GetCommands(DocumentOwner document)
    {
      yield break;
    }
  }
}