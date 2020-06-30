using System;
using System.Collections.Generic;
using System.Linq;
using TextRight.Core.Blocks;
using TextRight.Core.ObjectModel.Blocks;
using TextRight.Core.ObjectModel.Blocks.Text;
using TextRight.Core.ObjectModel.Serialization;
using Xunit;

namespace TextRight.Core.Tests.Serialization
{
  public class HeadingTests
  {
    [Fact]
    public void Verify_Serialization()
    {
      var paragraph = new HeadingBlock();
      var caret = (TextCaret)paragraph.GetCaretAtStart();

      caret.InsertText("This is some of the text")
           .InsertText("Some additional text");

      var descriptorsLookup = new DescriptorsLookup((BlockDescriptor)HeadingBlock.Descriptor);

      // Act
      SerializationHelpers.VerifyDeserialization(paragraph, descriptorsLookup);
    }
  }
}