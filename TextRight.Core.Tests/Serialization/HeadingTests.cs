using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using TextRight.Core.ObjectModel.Blocks;
using TextRight.Core.ObjectModel.Blocks.Text;
using TextRight.Core.ObjectModel.Serialization;

namespace TextRight.Core.Tests.Serialization
{
  public class HeadingTests
  {
    [Test]
    public void Verify_Serialization()
    {
      var paragraph = new HeadingBlock();
      var cursor = paragraph.GetTextCursor().ToBeginning();
      cursor.InsertText("This is some of the text");
      paragraph.AppendSpan(new StyledTextFragment("Some additional text"));

      var descriptorsLookup = new DescriptorsLookup(HeadingBlock.DescriptorInstance.Descriptor);

      // Act
      SerializationHelpers.VerifyDeserialization(paragraph, descriptorsLookup);
    }
  }
}