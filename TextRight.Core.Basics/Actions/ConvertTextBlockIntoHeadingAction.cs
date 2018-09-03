using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TextRight.Core.Blocks;
using TextRight.Core.Actions;
using TextRight.Core.ObjectModel.Blocks;
using TextRight.Core.ObjectModel.Blocks.Text;

namespace TextRight.Core.Actions
{
  /// <summary> Converts a TextBlock into a heading. </summary>
  public class ConvertTextBlockIntoHeadingAction : ConvertTextBlockToTextBlockAction<HeadingBlock>
  {
    private readonly int _level;

    /// <summary> Constructor. </summary>
    public ConvertTextBlockIntoHeadingAction(TextCaret caret, int level)
      : base(caret)
    {
      _level = level;
    }

    /// <inheritdoc />
    public override string Name
      => "Convert to Heading";

    /// <inheritdoc />
    public override string Description
      => "Convert paragraph into a heading";

    /// <inheritdoc />
    public override BlockDescriptor GetDestinationDescriptor() 
      => HeadingBlock.DescriptorInstance;

    /// <inheritdoc />
    public override void MakeChangesTo(HeadingBlock block)
    {
      block.HeadingLevel = _level;
    }
  }
}