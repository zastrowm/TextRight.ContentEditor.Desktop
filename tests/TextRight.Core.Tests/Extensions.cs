using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TextRight.Core.Actions;
using TextRight.Core.Cursors;
using TextRight.Core.ObjectModel.Blocks;
using TextRight.Core.ObjectModel.Blocks.Collections;
using TextRight.Core.ObjectModel.Blocks.Text;

namespace TextRight.Core.Tests
{
  public static class Extensions
  {
    /// <summary>
    ///  Allows using a collection initializer to initialize a block collection
    ///  with blocks.
    /// </summary>
    public static void Add(this BlockCollection collection, Block block)
    {
      collection.Append(block);
    }

    /// <summary>
    ///  Allows using a collection initializer to initialize a block with text
    ///  fragments.
    /// </summary>
    public static void Add(this TextBlock block, string text)
    {
      block.Content.Insert(block.Content.GetCaretAtStart(), text);
    }

    /// <summary> Set the text of the TextBlock to be equal to the given text. </summary>
    public static TextBlock WithText(this TextBlock block, string text)
    {
      block.Content.RemoveAll();
      block.Add(text);
      return block;
    }

    /// <summary>
    ///  Convert a block into the text that is contained within the block.
    /// </summary>
    public static string AsText(this Block block)
    {
      var textBlock = (TextBlock)block;
      return textBlock.Content.AsText();
    }

    /// <summary>
    ///  Convert a block into the text that is contained within the block.
    /// </summary>
    public static string AsText(this TextBlockContent content)
    {
      var builder = new StringBuilder();
      content.Buffer.AppendTo(builder);
      return builder.ToString();
    }

    /// <summary>
    ///  By default, a BlockCollection has a single child. That interferes with
    ///  our added elements, so remove the elements that were auto-added.
    /// </summary>
    public static BlockCollection RemoveFirstChilds(this BlockCollection collection)
    {
      collection.RemoveBlock(collection.FirstBlock);

      foreach (var block in collection.Children)
      {
        (block as BlockCollection)?.RemoveFirstChilds();
      }

      return collection;
    }

    /// <summary> Get the Nth block out of the collection. </summary>
    public static ContentBlock NthBlock(this BlockCollection collection, int index)
    {
      return (ContentBlock)collection.Children.Skip(index).First();
    }

    /// <summary> Gets a cursor to the end of the block. </summary>
    public static BlockCaret EndCursor(this ContentBlock block, int offset = 0)
    {
      return block.GetCaretAtEnd();
    }

    public static BlockCaret BeginCaret(this ContentBlock block, int offset = 0)
    {
      var caret = ((TextBlock)block).Content.GetCaretAtStart();
      while (offset > 0)
      {
        caret = caret.GetNextPosition();
        offset -= 1;
      }

      return caret;
    }

    public static BlockCaret EndCaret(this ContentBlock block, int offset = 0)
    {
      var caret = ((TextBlock)block).Content.GetCaretAtEnd();
      while (offset > 0)
      {
        caret = caret.GetPreviousPosition();
        offset -= 1;
      }

      return caret;
    }

    /// <summary> Gets a cursor to the beginning of the block. </summary>
    public static BlockCaret BeginCursor(this ContentBlock block, int offset = 0)
      => block.BeginCaret(offset);

    public static TextCaret AsTextCursor(this BlockCaret cursor)
    {
      return (TextCaret)cursor;
    }

    /// <summary> Creates a cursor handle from the given content cursor. </summary>
    public static DocumentCursorHandle ToHandle(this TextCaret caret)
      => new DocumentCursorHandle(caret);

    /// <summary> Creates a cursor handle from the given content cursor. </summary>
    public static DocumentCursorHandle ToHandle(this BlockCaret caret)
      => new DocumentCursorHandle(caret);
  }
}