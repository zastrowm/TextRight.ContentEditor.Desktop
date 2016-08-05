using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using TextRight.ContentEditor.Core.ObjectModel.Blocks;
using TextRight.ContentEditor.Core.ObjectModel.Serialization;

namespace TextRight.ContentEditor.Core.Tests.Serialization
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

      var descriptorsLookup = new DescriptorsLookup(HeadingBlock.RegisteredDescriptor.Descriptor);

      // Act
      SerializationHelpers.VerifyDeserialization(paragraph, descriptorsLookup);
    }
  }
}