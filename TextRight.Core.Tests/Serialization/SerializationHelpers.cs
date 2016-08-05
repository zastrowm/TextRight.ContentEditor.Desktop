using FluentAssertions;
using TextRight.ContentEditor.Core.ObjectModel.Blocks;
using TextRight.ContentEditor.Core.ObjectModel.Serialization;

static internal class SerializationHelpers
{
  /// <summary> Verifies that the given block serializes and then deserializes properly. </summary>
  /// <param name="originalBlock"> The original block to test the serialization process for. </param>
  /// <param name="descriptorsLookup"> The descriptors that should be used when deserializing. </param>
  public static void VerifyDeserialization(Block originalBlock, DescriptorsLookup descriptorsLookup)
  {
    var node = originalBlock.Serialize();
    var context = new SerializationContext(descriptorsLookup);

    var deserializedBlock = context.Deserialize(node);
    deserializedBlock.As<object>().ShouldBeEquivalentTo(originalBlock,
                                                        c => c.IgnoringCyclicReferences());
  }
}