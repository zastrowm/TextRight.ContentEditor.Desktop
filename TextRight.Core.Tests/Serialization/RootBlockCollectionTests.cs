using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TextRight.Core.ObjectModel.Blocks;
using TextRight.Core.ObjectModel.Blocks.Collections;
using TextRight.Core.ObjectModel.Blocks.Text;
using TextRight.Core.ObjectModel.Serialization;

using Xunit;

namespace TextRight.Core.Tests.Serialization
{
  public class RootBlockCollectionTests
  {
    [Fact]
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
      StyledTextFragment fragment = new StyledTextFragment("Some additional text");
      paragraph.Content.AppendSpan(fragment, true);
      return paragraph;
    }
  }
}