using System;
using System.Collections.Generic;
using System.Linq;
using TextRight.ContentEditor.Core.Editing;

namespace TextRight.ContentEditor.Core.ObjectModel.Blocks
{
  /// <summary> BlockDescriptor for <see cref="HeadingBlock"/>. </summary>
  public class HeadingBlockDescriptor : ContentBlockDescriptor<HeadingBlock>
  {
    /// <inheritdoc />
    public override string Id
      => "heading+multilevel";

    /// <inheritdoc />
    public override IEnumerable<IContextualCommand> GetCommands(DocumentOwner document)
    {
      yield return new ConvertToLevelHeadingCommand(0);
      yield return new ConvertToLevelHeadingCommand(1);
      yield return new ConvertToLevelHeadingCommand(2);
      yield return new ConvertToLevelHeadingCommand(3);
      yield return new ConvertToLevelHeadingCommand(4);
      yield return new ConvertToLevelHeadingCommand(5);
    }
  }
}