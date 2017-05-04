using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using TextRight.Core.ObjectModel.Blocks.Text;
using Xunit;

namespace TextRight.Core.Tests.ObjectModel.Blocks
{
  public class TextBlockCursorTests
  {
    public StyledTextFragment a,
                              b,
                              c,
                              d,
                              e;

    public TextBlock Block;

    public TextBlockCursorTests()
    {
      Block = new ParagraphBlock();
      Block.Add((a = new StyledTextFragment("123", "a")));
      Block.Add((b = new StyledTextFragment("456", "b")));
      Block.Add((c = new StyledTextFragment("789", "c")));
    }

    [Fact]
    public void BeginningPointsToBeginning()
    {
      var cursor = (TextBlockCursor)Block.GetCursor();
      cursor.MoveToBeginning();

      DidYouKnow.That(cursor.CharacterBefore).Should().Be('\0');
      DidYouKnow.That(cursor.CharacterAfter).Should().Be('1');
    }

    [Theory]
    [InlineData(0, '\0', '1')]
    [InlineData(1, '1', '2')]
    [InlineData(2, '2', '3')]
    [InlineData(3, '3', '\0', "End of first span")]
    [InlineData(4, '4', '5')]
    [InlineData(5, '5', '6')]
    [InlineData(6, '6', '\0', "End of second span")]
    [InlineData(7, '7', '8')]
    [InlineData(8, '8', '9')]
    [InlineData(9, '9', '\0', "End of third span")]
    public void MoveForwardWorks(int amountToMove, char beforeChar, char afterChar, string desc = null)
    {
      var cursor = (TextBlockCursor)Block.GetCursor();
      cursor.MoveToBeginning();

      for (int i = 0; i < amountToMove; i++)
      {
        cursor.MoveForward();
      }

      DidYouKnow.That(cursor.CharacterBefore).Should().Be(beforeChar);
      DidYouKnow.That(cursor.CharacterAfter).Should().Be(afterChar);
    }

    [Theory]
    [InlineData(9, '\0', '1')]
    [InlineData(8, '1', '2')]
    [InlineData(7, '2', '3')]
    [InlineData(6, '3', '\0', "End of first span")]
    [InlineData(5, '4', '5')]
    [InlineData(4, '5', '6')]
    [InlineData(3, '6', '\0', "End of second span")]
    [InlineData(2, '7', '8')]
    [InlineData(1, '8', '9')]
    [InlineData(0, '9', '\0', "End of third span")]
    public void MoveBackwardWorks(int amountToMove, char beforeChar, char afterChar, string desc = null)
    {
      var cursor = (TextBlockCursor)Block.GetCursor();
      cursor.MoveToEnd();

      for (int i = 0; i < amountToMove; i++)
      {
        cursor.MoveBackward();
      }

      DidYouKnow.That(cursor.CharacterBefore).Should().Be(beforeChar);
      DidYouKnow.That(cursor.CharacterAfter).Should().Be(afterChar);
    }

    private TextBlockCursor GetCursor(int amount)
    {
      var cursor = (TextBlockCursor)Block.GetCursor();
      cursor.MoveToBeginning();

      while (amount > 0)
      {
        cursor.MoveForward();
        amount -= 1;
      }

      return cursor;
    }

    [Fact]
    public void DetachingAtEndOfFirstSpan_DetachesIntoTwoGroups()
    {
      var cursor = GetCursor(3);
      var spans = cursor.ExtractToEnd();

      DidYouKnow.That(spans.Length).Should().Be(2);
      DidYouKnow.That(spans[0]).Should().Be(b);
      DidYouKnow.That(spans[1]).Should().Be(c);

      DidYouKnow.That(Block.Content.ChildCount).Should().Be(1);
      DidYouKnow.That(Block.Content.Fragments.First()).Should().Be(a);
    }

    [Fact]
    public void DetachingAtMiddleOfSpan_DetachesIntoNewFragment()
    {
      var cursor = GetCursor(2);
      var spans = cursor.ExtractToEnd();

      DidYouKnow.That(spans.Length).Should().Be(3);
      DidYouKnow.That(spans[0].GetText()).Should().Be("3");
      DidYouKnow.That(spans[1]).Should().Be(b);
      DidYouKnow.That(spans[2]).Should().Be(c);

      DidYouKnow.That(Block.Content.ChildCount).Should().Be(1);
      DidYouKnow.That(Block.Content.Fragments.First()).Should().Be(a);
      DidYouKnow.That(Block.Content.Fragments.First().GetText()).Should().Be("12");
    }

    [Fact]
    public void DetachingAtBeginningOfFirstSpan_DetachesAllFragments()
    {
      var cursor = GetCursor(0);
      var spans = cursor.ExtractToEnd();

      DidYouKnow.That(spans.Length).Should().Be(3);
      DidYouKnow.That(spans[0].GetText()).Should().Be("123");
      DidYouKnow.That(spans[1]).Should().Be(b);
      DidYouKnow.That(spans[2]).Should().Be(c);

      DidYouKnow.That(Block.Content.ChildCount).Should().Be(1);
      DidYouKnow.That(Block.Content.Fragments.First().GetText()).Should().Be("");
    }

    [Fact]
    public void DetachingAtEndOfLastSpan_DetachesNoFragments()
    {
      var cursor = GetCursor(9);
      var spans = cursor.ExtractToEnd();

      DidYouKnow.That(spans.Length).Should().Be(1);
      DidYouKnow.That(spans[0].GetText()).Should().Be("");

      DidYouKnow.That(Block.Content.ChildCount).Should().Be(3);
      DidYouKnow.That(Block.Content.Fragments.ElementAt(0)).Should().Be(a);
      DidYouKnow.That(Block.Content.Fragments.ElementAt(0).GetText()).Should().Be("123");
      DidYouKnow.That(Block.Content.Fragments.ElementAt(1)).Should().Be(b);
      DidYouKnow.That(Block.Content.Fragments.ElementAt(1).GetText()).Should().Be("456");
      DidYouKnow.That(Block.Content.Fragments.ElementAt(2)).Should().Be(c);
      DidYouKnow.That(Block.Content.Fragments.ElementAt(2).GetText()).Should().Be("789");
    }
  }
}