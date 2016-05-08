using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TextRight.ContentEditor.Core.Editing.Actions;
using TextRight.ContentEditor.Core.ObjectModel.Blocks;

namespace TextRight.ContentEditor.Core.Tests
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
    public static void Add(this TextBlock block, StyledTextFragment fragment)
    {
      block.AppendSpan(fragment, false);
    }

    /// <summary> Set the text of the TextBlock to be equal to the given text. </summary>
    public static TextBlock WithText(this TextBlock block, string text)
    {
      block.RemoveSpan(block.First());
      block.AppendSpan(new StyledTextFragment(text));
      return block;
    }

    /// <summary>
    ///  Convert a block into the text that is contained within the block.
    /// </summary>
    public static string AsText(this Block block)
    {
      var textBlock = (TextBlock)block;

      var builder = new StringBuilder();

      foreach (var fragment in textBlock)
      {
        builder.Append(fragment.Text);
      }

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
    public static Block NthBlock(this BlockCollection collection, int index)
    {
      return collection.Children.Skip(index).First();
    }

    /// <summary> Gets a cursor to the end of the block. </summary>
    public static IBlockContentCursor EndCursor(this Block block, int offset = 0)
    {
      var cursor = block.GetCursor().ToEnd();
      cursor.Move(offset);
      return cursor;
    }

    /// <summary> Gets a cursor to the beginning of the block. </summary>
    public static IBlockContentCursor BeginCursor(this Block block, int offset = 0)
    {
      var cursor = block.GetCursor().ToBeginning();
      cursor.Move(offset);
      return cursor;
    }

    /// <summary> Creates a cursor handle from the given content cursor. </summary>
    public static DocumentCursorHandle ToHandle(this IBlockContentCursor cursor)
      => new DocumentCursorHandle(cursor);

    /// <summary>
    ///  Move the block cursor the correct number of units forward or backward.
    /// </summary>
    public static void Move(this IBlockContentCursor cursor, int amount)
    {
      while (amount > 0)
      {
        cursor.MoveForward();
        amount--;
      }

      while (amount < 0)
      {
        cursor.MoveBackward();
        amount++;
      }
    }
  }
}