using System;
using System.Collections.Generic;
using System.Linq;
using TextRight.Core.ObjectModel.Blocks;
using TextRight.Core.ObjectModel.Blocks.Text;

namespace TextRight.Core.Commands.Text
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

      var caret = (TextCaret)textBlock.GetCaretAtStart();

      var previous = textBlock.PreviousBlock as TextBlock;
      if (previous == null)
        return false;

      var contentToMerge = textBlock.Content.ExtractContent(caret, (TextCaret)textBlock.GetCaretAtEnd());
      previous.Content.AppendAll(contentToMerge.Spans);

      collection.RemoveBlock(textBlock);

      return true;
    }

    /// <summary> Breaks the block into two at the given location. </summary>
    /// <exception cref="ArgumentNullException"> Thrown when one or more required arguments are null. </exception>
    /// <param name="caret"> The caret at which the block should be split. </param>
    /// <returns>
    ///  A caret pointing to the beginning of the block that follows the block that was split.  Will
    ///  be TextCaret.Invalid if the block could not be split.
    /// </returns>
    public static TextCaret TryBreakBlock(TextCaret caret)
    {
      if (!caret.IsValid)
        throw new ArgumentException("invalid caret", nameof(caret));

      if (!CanBreak(caret))
        return TextCaret.Invalid;

      var targetBlock = caret.Span.Parent;
      var blockCollection = targetBlock.Parent;

      if (caret.IsAtBlockEnd)
      {
        var secondaryBlock = (TextBlock)CreateSimilarBlock(targetBlock);
        blockCollection.InsertBlockAfter(targetBlock, secondaryBlock);
        return secondaryBlock.Content.GetCaretAtEnd();
      }

      if (caret.IsAtBlockStart)
      {
        blockCollection.InsertBlockBefore(targetBlock, CreateSimilarBlock(targetBlock));
        return targetBlock.Content.GetCaretAtStart();
      }

      var textBlockContent = caret.Span.Owner;
      var extractedContent = textBlockContent.ExtractContent(caret, textBlockContent.GetCaretAtEnd());

      var newTextBlock = (TextBlock)CreateSimilarBlock(targetBlock);

      newTextBlock.Content = extractedContent;
      blockCollection.InsertBlockAfter(targetBlock, newTextBlock);

      return newTextBlock.Content.GetCaretAtStart();
    }

    private static ContentBlock CreateSimilarBlock(ContentBlock block)
    {
      return (ContentBlock)block.DescriptorHandle.CreateInstance();
    }

    /// <summary>
    ///  True if the block can break into two at the given position.
    /// </summary>
    /// <param name="cursor"> The caret that specified the position. </param>
    /// <returns> true if we can break, false if not. </returns>
    public static bool CanBreak(TextCaret cursor)
      => true; // TODO
  }
}