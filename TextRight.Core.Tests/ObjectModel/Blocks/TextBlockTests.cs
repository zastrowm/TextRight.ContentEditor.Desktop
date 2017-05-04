using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using TextRight.Core.Editing.Actions.Text;
using TextRight.Core.ObjectModel.Blocks.Collections;
using TextRight.Core.ObjectModel.Blocks.Text;
using Xunit;

namespace TextRight.Core.Tests.ObjectModel.Blocks
{
  public class TextBlockTests
  {
    private TextBlock _block;
    private BlockCollection _collection;
    private TextBlockCursor _cursor;

    private void Initialize(string content)
    {
      var index = content.IndexOf("|");
      content = content.Replace("|", "");

      _block = new ParagraphBlock();
      _cursor = (TextBlockCursor)_block.GetCursor();
      _cursor.MoveToBeginning();
      ((TextBlockCursor)_cursor).InsertText(content);
      _cursor.MoveToBeginning();

      if (index > 0)
      {
        _cursor.Move(index);
      }

      _collection = new AddableBlockCollection()
                    {
                      _block
                    };
    }

    [Fact]
    public void ByDefault_HasSingleSpan()
    {
      var block = new ParagraphBlock();

      DidYouKnow.That(block.Content.Fragments.Count()).Should().Be(1);
    }

    [Fact]
    public void Break_AtBeginning_HasNewBlock()
    {
      Initialize("abc|123");
      var nextBlock = (TextBlock)TextBlockHelperMethods.TryBreakBlock(_cursor);

      DidYouKnow.That(nextBlock).Should().NotBeNull();

      DidYouKnow.That(_block.Content.Fragments.First().GetText()).Should().Be("abc");
      DidYouKnow.That(nextBlock.Content.Fragments.First().GetText()).Should().Be("123");
    }
  }
}