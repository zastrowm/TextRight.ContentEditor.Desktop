using System;
using System.Collections.Generic;
using System.Linq;
using TextRight.Core.ObjectModel.Blocks;
using TextRight.Core.ObjectModel.Blocks.Text;

namespace TextRight.Core.Actions
{
  /// <summary> Converts a TextBlock into a normal paragraph. </summary>
  public class ConvertIntoParagraphAction : ConvertTextBlockToTextBlockAction<ParagraphBlock>
  {
    public ConvertIntoParagraphAction(TextCaret caret)
      : base(caret)
    {
    }

    /// <inheritdoc />
    public override string Name
      => "Convert to Heading";

    /// <inheritdoc />
    public override string Description
      => "Convert paragraph into a heading";

    /// <inheritdoc />
    public override RegisteredDescriptor GetDestinationDescriptor() 
      => ParagraphBlock.RegisteredDescriptor;

    public override void MakeChangesTo(ParagraphBlock block)
    {
      // no-op, it's just plain ol' text
    }
  }
}