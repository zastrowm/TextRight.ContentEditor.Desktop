using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TextRight.ContentEditor.Core.ObjectModel.Blocks;

namespace TextRight.ContentEditor.Core.Tests
{
  public static class Extensions
  {
    public static void Add(this BlockCollection collection, Block block)
    {
      collection.Append(block);
    }

    public static void Add(this TextBlock block, StyledTextFragment fragment)
    {
      block.AppendSpan(fragment);
    }

    /// <summary>
    ///  By default, a BlockCollection has a single child. That interferes with
    ///  our added elements, so remove the elements that were auto-added.
    /// </summary>
    public static void RemoveFirstChilds(this BlockCollection collection)
    {
      collection.RemoveBlock(collection.FirstBlock);

      foreach (var block in collection.Children)
      {
        (block as BlockCollection)?.RemoveFirstChilds();
      }
    }
  }
}