using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;
using TextRight.ContentEditor.Core.ObjectModel.Blocks;

namespace TextRight.ContentEditor.Core.Tests.ObjectModel.Blocks
{
  internal class TextBlockTests
  {
    private TextBlock _block;
    private BlockCollection _collection;
    private TextBlock.TextBlockCursor _cursor;

    private void Initialize(string content)
    {
      var index = content.IndexOf("|");
      content = content.Replace("|", "");

      _block = new TextBlock();
      _cursor = (TextBlock.TextBlockCursor)_block.GetCursor();
      _cursor.MoveToBeginning();
      ((ITextContentCursor)_cursor).InsertText(content);
      _cursor.MoveToBeginning();

      if (index > 0)
      {
        _cursor.Move(index);
      }

      _collection = new BlockCollection()
                    {
                      _block
                    };
    }

    [Test]
    public void ByDefault_HasSingleSpan()
    {
      var block = new TextBlock();

      Assert.That(block.Count(), Is.EqualTo(1));
    }

    [Test]
    public void Break_AtBeginning_HasNewBlock()
    {
      Initialize("abc|123");
      var nextBlock = (TextBlock)_collection.TryBreakBlock(_cursor);

      Assert.That(nextBlock, Is.Not.Null);

      Assert.That(_block.First().Text, Is.EqualTo("abc"));
      Assert.That(nextBlock.First().Text, Is.EqualTo("123"));
    }
  }
}