using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;
using TextRight.ContentEditor.Core.ObjectModel.Blocks;
using TextRight.ContentEditor.Core.ObjectModel.Serialization;

namespace TextRight.ContentEditor.Core.Tests.Serialization
{
  internal class RootBlockCollectionTests
  {
    [Test]
    public void Verify_Serialization()
    {
      var collection = new RootBlockCollection();
      collection.Append(CreateBlock());
      collection.Append(CreateBlock());
      collection.Append(CreateBlock());

      var descriptorsLookup = new DescriptorsLookup(RootBlockCollection.RegisteredDescriptor.Descriptor,
                                                    ParagraphBlock.RegisteredDescriptor.Descriptor);

      // Act
      SerializationHelpers.VerifyDeserialization(collection, descriptorsLookup);
    }

    private ParagraphBlock CreateBlock()
    {
      var paragraph = new ParagraphBlock();
      var cursor = paragraph.GetTextCursor().ToBeginning();
      cursor.InsertText("This is some of the text");
      paragraph.AppendSpan(new StyledTextFragment("Some additional text"));
      return paragraph;
    }
  }
}