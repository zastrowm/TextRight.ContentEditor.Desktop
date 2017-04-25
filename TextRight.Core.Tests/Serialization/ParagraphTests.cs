using System;
using System.Collections.Generic;
using System.Linq;
using TextRight.Core.ObjectModel.Blocks;
using TextRight.Core.ObjectModel.Blocks.Text;
using TextRight.Core.ObjectModel.Serialization;
using Xunit;

namespace TextRight.Core.Tests.Serialization
{
  public class ParagraphTests
  {
    [Fact]
    public void Verify_Serialization()
    {
      var paragraph = new ParagraphBlock();
      var cursor = paragraph.GetTextCursor().ToBeginning();
      cursor.InsertText("This is some of the text");
      StyledTextFragment fragment = new StyledTextFragment("Some additional text");
      paragraph.Content.AppendSpan(fragment, true);

      var descriptorsLookup = new DescriptorsLookup(ParagraphBlock.RegisteredDescriptor.Descriptor);

      // Act
      SerializationHelpers.VerifyDeserialization(paragraph, descriptorsLookup);
    }
  }
}