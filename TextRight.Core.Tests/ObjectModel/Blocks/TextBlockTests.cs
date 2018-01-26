using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using TextRight.Core.Commands.Text;
using TextRight.Core.ObjectModel.Blocks.Collections;
using TextRight.Core.ObjectModel.Blocks.Text;
using Xunit;

namespace TextRight.Core.Tests.ObjectModel.Blocks
{
  public class TextBlockTests
  {
    private TextBlock _block;
    private BlockCollection _collection;
    private TextCaret _caret;

    private void Initialize(string content)
    {
      var index = content.IndexOf("|");
      content = content.Replace("|", "");

      _block = new ParagraphBlock();
      _caret = _block.Content.GetCaretAtStart();
      _caret.InsertText(content);
      _caret = _block.Content.GetCaretAtStart();

      if (index > 0)
      {
        _caret = _caret.MoveCursorForwardBy(index);
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

      DidYouKnow.That(block.Content.Spans.Count()).Should().Be(1);
    }

    [Fact]
    public void Break_AtBeginning_HasNewBlock()
    {
      Initialize("abc|123");
      var nextBlock = (TextBlock)TextBlockHelperMethods.TryBreakBlock(_caret).Block;

      DidYouKnow.That(nextBlock).Should().NotBeNull();

      DidYouKnow.That(_block.Content.Spans.First().GetText()).Should().Be("abc");
      DidYouKnow.That(nextBlock.Content.Spans.First().GetText()).Should().Be("123");
    }
  }
}