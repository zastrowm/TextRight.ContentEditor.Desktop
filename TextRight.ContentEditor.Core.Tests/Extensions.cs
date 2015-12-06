using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TextRight.ContentEditor.Desktop.ObjectModel.Blocks;

namespace TextRight.ContentEditor.Desktop.Tests
{
  public static class Extensions
  {
    public static void Add(this BlockCollection collection, Block block)
    {
      collection.Append(block);
    }

    public static void Add(this TextBlock block, TextSpan span)
    {
      block.AppendSpan(span);
    }
  }
}