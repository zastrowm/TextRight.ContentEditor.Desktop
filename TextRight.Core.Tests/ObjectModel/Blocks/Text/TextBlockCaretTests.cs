using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using TextRight.Core.ObjectModel.Blocks.Text;
using TextRight.Core.Tests.Framework;
using Xunit;

namespace TextRight.Core.Tests.ObjectModel.Blocks.Text
{
  public class TextBlockCaretTests
  {
    [Fact]
    public void CharactersAreReportedCorrectly_AtBeginning()
    {
      var text = "0123456789";
      var content = CreateContent(text);

      var cursor = content.GetCursorToBeginning();

      DidYouKnow.That(cursor.IsAtBeginningOfBlock).Should().BeTrue();
      DidYouKnow.That(cursor.IsAtBeginningOfFragment).Should().BeTrue();

      DidYouKnow.That(cursor.IsAtEndOfBlock).Should().BeFalse();
      DidYouKnow.That(cursor.IsAtBeginningOfFragment).Should().BeFalse();

      DidYouKnow.That(cursor.OffsetIntoSpan).Should().Be(0);
      DidYouKnow.That(cursor.CharacterAfter).Should().Be('0');
      DidYouKnow.That(cursor.CharacterBefore).Should().Be('\0');
    }

    [Fact]
    public void CharactersAreReportedCorrectly_AtEnd()
    {
      var text = "0123456789";
      var content = CreateContent(text);

      var cursor = content.GetCursorToEnd();

      DidYouKnow.That(cursor.IsAtBeginningOfBlock).Should().BeFalse();
      DidYouKnow.That(cursor.IsAtBeginningOfFragment).Should().BeFalse();

      DidYouKnow.That(cursor.IsAtEndOfBlock).Should().BeTrue();
      DidYouKnow.That(cursor.IsAtBeginningOfFragment).Should().BeTrue();

      DidYouKnow.That(cursor.OffsetIntoSpan).Should().Be(10);
      DidYouKnow.That(cursor.CharacterAfter).Should().Be('\0');
      DidYouKnow.That(cursor.CharacterBefore).Should().Be('9');
    }

    private static TextBlockContent CreateContent(params string[] texts)
    {
      var content = new TextBlockContent();
      content.AppendAll(texts.Select((t,i) => new StyledTextFragment(t, $"Style_{i}")));
      return content;
    }

    [Theory]
    [RangedData(1, 9)]
    public void CharactersAreReportedCorrectly_ForMiddleIndices(int index)
    {
      var text = "0123456789";
      var expectedRightCharacter = text[index];
      var expectedLeftCharacter = text[index - 1];

      var content = CreateContent(text);

      var cursor = content.GetCursorToBeginning().MoveCursorForwardBy(index);

      DidYouKnow.That(cursor.OffsetIntoSpan).Should().Be(index);
      DidYouKnow.That(cursor.CharacterAfter).Should().Be(expectedRightCharacter);
      DidYouKnow.That(cursor.CharacterBefore).Should().Be(expectedLeftCharacter);
    }

    [Fact]
    public void CharactersAreReportedCorrectly_AfterMoving10()
    {
      var text = "0123456789";
      var content = CreateContent(text);

      var cursor = content.GetCursorToBeginning().MoveCursorForwardBy(10);

      DidYouKnow.That(cursor).Should().Be(content.GetCursorToEnd());
      DidYouKnow.That(cursor.OffsetIntoSpan).Should().Be(10);
      DidYouKnow.That(cursor.CharacterAfter).Should().Be('\0');
      DidYouKnow.That(cursor.CharacterBefore).Should().Be('9');
    }

    [Theory]
    [InlineData(0, '\0', '1')]
    [InlineData(1, '1', '2')]
    [InlineData(2, '2', '3')]
    [InlineData(3, '3', '4', "End of first span")]
    [InlineData(4, '4', '5')]
    [InlineData(5, '5', '6')]
    [InlineData(6, '6', '7', "End of second span")]
    [InlineData(7, '7', '8')]
    [InlineData(8, '8', '9')]
    [InlineData(9, '9', '\0', "End of third span")]
    public void MoveForwardWorks(int amountToMove, char beforeChar, char afterChar, string desc = null)
    {
      var content = CreateContent("123", "456", "789");

      var cursor = content.GetCursorToBeginning().MoveCursorForwardBy(amountToMove);

      DidYouKnow.That(cursor.CharacterBefore).Should().Be(beforeChar);
      DidYouKnow.That(cursor.CharacterAfter).Should().Be(afterChar);
    }

    [Theory]
    [InlineData(9, '\0', '1')]
    [InlineData(8, '1', '2')]
    [InlineData(7, '2', '3')]
    [InlineData(6, '3', '4', "End of first span")]
    [InlineData(5, '4', '5')]
    [InlineData(4, '5', '6')]
    [InlineData(3, '6', '7', "End of second span")]
    [InlineData(2, '7', '8')]
    [InlineData(1, '8', '9')]
    [InlineData(0, '9', '\0', "End of third span")]
    public void MoveBackwardWorks(int amountToMove, char beforeChar, char afterChar, string desc = null)
    {
      var content = CreateContent("123", "456", "789");

      var cursor = content.GetCursorToEnd().MoveCursorBackwardBy(amountToMove);

      DidYouKnow.That(cursor.CharacterBefore).Should().Be(beforeChar);
      DidYouKnow.That(cursor.CharacterAfter).Should().Be(afterChar);
    }
  }
}