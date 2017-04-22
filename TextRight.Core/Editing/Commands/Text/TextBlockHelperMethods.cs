using System;
using System.Collections.Generic;
using System.Linq;
using TextRight.Core.ObjectModel.Blocks;
using TextRight.Core.ObjectModel.Blocks.Text;

namespace TextRight.Core.Editing.Actions.Text
{
  /// <summary> Helper methods for manipulating TextBlocks </summary>
  internal static class TextBlockHelperMethods
  {
    /// <summary> Merges the given block with the previous block. </summary>
    /// <param name="textBlock"> The block to remove from the collection and whose content should be
    ///  merged with the previous block. </param>
    /// <returns> True if it was merged, false if it was not. </returns>
    public static bool MergeWithPrevious(TextBlock textBlock)
    {
      // TODO support more than just text blocks?
      if (textBlock == null)
        return false;

      // TODO handle parent-blocks
      if (textBlock.IsFirst)
        return false;

      var collection = textBlock.Parent;

      var cursor = textBlock.GetTextCursor();
      cursor.MoveToBeginning();

      var previous = textBlock.PreviousBlock as TextBlock;
      if (previous == null)
        return false;

      var fragments = cursor.ExtractToEnd();
      previous.Content.AppendAll(fragments);

      collection.RemoveBlock(textBlock);

      return true;
    }

    /// <summary> Breaks the block into two at the given location. </summary>
    /// <param name="cursor"> The caret at which the block should be split. </param>
    /// <returns>
    ///  The block that is the next sibling of the original block that was split
    ///  into two.
    /// </returns>
    public static ContentBlock TryBreakBlock(IBlockContentCursor cursor)
    {
      if (cursor == null)
        throw new ArgumentNullException(nameof(cursor));

      if (!CanBreak(cursor))
        return null;

      var targetBlock = cursor.Block;

      var blockCollection = targetBlock.Parent;

      ContentBlock secondaryBlock = null;

      if (cursor.IsAtEnd)
      {
        secondaryBlock = CreateSimilarBlock(targetBlock);
        blockCollection.InsertBlockAfter(targetBlock, secondaryBlock);
      }
      else if (cursor.IsAtBeginning)
      {
        secondaryBlock = targetBlock;
        blockCollection.InsertBlockBefore(targetBlock, CreateSimilarBlock(targetBlock));
      }
      else
      {
        var textBlockCursor = (TextBlockCursor)cursor;
        var fragments = textBlockCursor.ExtractToEnd();

        var newTextBlock = (TextBlock)CreateSimilarBlock(targetBlock);
        secondaryBlock = newTextBlock;

        // TODO should this be done by AppendSpan automatically?
        StyledTextFragment fragment1 = newTextBlock.Content.Fragments.First();
        newTextBlock.Content.RemoveSpan(fragment1);

        foreach (var fragment in fragments)
        {
          newTextBlock.Content.AppendSpan(fragment, true);
        }

        blockCollection.InsertBlockAfter(targetBlock, secondaryBlock);
      }

      return secondaryBlock;
    }

    private static ContentBlock CreateSimilarBlock(ContentBlock block)
    {
      return (ContentBlock)block.DescriptorHandle.Descriptor.CreateInstance();
    }

    /// <summary>
    ///  True if the block can break into two at the given position.
    /// </summary>
    /// <param name="cursor"> The caret that specified the position. </param>
    /// <returns> true if we can break, false if not. </returns>
    public static bool CanBreak(IBlockContentCursor cursor)
      => true; // TODO
  }
}