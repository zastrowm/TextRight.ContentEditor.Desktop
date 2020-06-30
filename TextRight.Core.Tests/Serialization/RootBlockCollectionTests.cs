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

      var descriptorsLookup = new DescriptorsLookup(RootBlockCollection.Descriptor,
                                                    ParagraphBlock.Descriptor);

      // Act
      SerializationHelpers.VerifyDeserialization(collection, descriptorsLookup);
    }

    private ParagraphBlock CreateBlock()
    {
      var paragraph = new ParagraphBlock();
      var cursor = (TextCaret)paragraph.GetCaretAtStart();
      cursor.InsertText("This is some of the text")
            .InsertText("Some additional text");
      return paragraph;
    }
  }
}