using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
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
      var cursor = (TextCaret)paragraph.GetCaretAtStart();
      var next = cursor.InsertText("This is some of the text");
      next.InsertText("Some additional text");

      next.Content.GetText()
          .Should()
          .BeEquivalentTo("This is some of the textSome additional text");

      var descriptorsLookup = new DescriptorsLookup(ParagraphBlock.Descriptor);

      // Act
      SerializationHelpers.VerifyDeserialization(paragraph, descriptorsLookup);
    }
  }
}