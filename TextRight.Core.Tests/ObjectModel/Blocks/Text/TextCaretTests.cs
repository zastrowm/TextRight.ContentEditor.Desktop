using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using TextRight.Core.ObjectModel.Blocks.Text;
using TextRight.Core.Tests.Framework;
using Xunit;

namespace TextRight.Core.Tests.ObjectModel.Blocks.Text
{
  public class TextCaretTests
  {
    [Fact]
    public void CharactersAreReportedCorrectly_AtBeginning()
    {
      var text = "0123456789";
      var content = CreateContent(text);

      var cursor = content.GetCaretAtBeginning();

      DidYouKnow.That(cursor.IsAtBeginningOfBlock).Should().BeTrue();
      DidYouKnow.That(cursor.IsAtBeginningOfFragment).Should().BeTrue();

      DidYouKnow.That(cursor.IsAtEndOfBlock).Should().BeFalse();
      DidYouKnow.That(cursor.IsAtEndOfFragment).Should().BeFalse();

      DidYouKnow.That(cursor.Offset.GraphemeOffset).Should().Be(0);
      DidYouKnow.That(cursor.CharacterAfter.Character).Should().Be('0');
      DidYouKnow.That(cursor.GetCharacterBefore().Character).Should().Be('\0');
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
      DidYouKnow.That(cursor.IsAtEndOfFragment).Should().BeTrue();

      DidYouKnow.That(cursor.Offset.GraphemeOffset).Should().Be(10);
      DidYouKnow.That(cursor.CharacterAfter.Character).Should().Be('\0');
      DidYouKnow.That(cursor.GetCharacterBefore().Character).Should().Be('9');
    }

    private static TextBlockContent CreateContent(params string[] texts)
    {
      var content = new TextBlockContent();
      content.AppendAll(texts.Select((t, i) => new StyledTextFragment(t, $"Style_{i}")));
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

      var cursor = content.GetCaretAtBeginning().MoveCursorForwardBy(index);

      DidYouKnow.That(cursor.Offset.GraphemeOffset).Should().Be(index);
      DidYouKnow.That(cursor.CharacterAfter.Character).Should().Be(expectedRightCharacter);
      DidYouKnow.That(cursor.GetCharacterBefore().Character).Should().Be(expectedLeftCharacter);
    }

    [Fact]
    public void CharactersAreReportedCorrectly_AfterMoving10()
    {
      var text = "0123456789";
      var content = CreateContent(text);

      var cursor = content.GetCaretAtBeginning().MoveCursorForwardBy(10);

      DidYouKnow.That(cursor).Should().Be(content.GetCursorToEnd());
      DidYouKnow.That(cursor.Offset.GraphemeOffset).Should().Be(10);
      DidYouKnow.That(cursor.CharacterAfter.Character).Should().Be('\0');
      DidYouKnow.That(cursor.GetCharacterBefore().Character).Should().Be('9');
    }

    private const char NUL = '\0';

    private const bool Tru = true;
    private const bool Fls = false;

    public class MovementTestData : SerializableTestData<MovementTestData>
    {
      public int AmountToMove;
      public char ExpectedBeforeChar;
      public char ExpectedAfterChar;
      public bool IsAtFragmentStart;
      public bool IsAtFragmentEnd;
      public bool IsAtBlockStart;
      public bool IsAtBlockEnd;

      public override string ToString()
        => $"MoveBy: {AmountToMove}, Character: {ExpectedAfterChar}";

      public static MovementTestData CreateData(int amountToMove,
                                                char expectedBeforeChar,
                                                char expectedAfterChar,
                                                bool isAtFragmentStart,
                                                bool isAtFragmentEnd,
                                                bool isAtBlockStart,
                                                bool isAtBlockEnd)
        => new MovementTestData()
           {
             AmountToMove = amountToMove,
             ExpectedBeforeChar = expectedBeforeChar,
             ExpectedAfterChar = expectedAfterChar,
             IsAtFragmentStart = isAtFragmentStart,
             IsAtFragmentEnd = isAtFragmentEnd,
             IsAtBlockStart = isAtBlockStart,
             IsAtBlockEnd = isAtBlockEnd,
           };
    }

    public static TheoryData<MovementTestData> GetMoveForwardData()
    {
      return new TheoryData<MovementTestData>()
             {
               MovementTestData.CreateData(0, NUL, '1', Tru, Fls, Tru, Fls),
               MovementTestData.CreateData(1, '1', '2', Fls, Fls, Fls, Fls),
               MovementTestData.CreateData(2, '2', '3', Fls, Tru, Fls, Fls),
               MovementTestData.CreateData(3, '3', '4', Tru, Fls, Fls, Fls),
               MovementTestData.CreateData(4, '4', '5', Fls, Fls, Fls, Fls),
               MovementTestData.CreateData(5, '5', '6', Fls, Tru, Fls, Fls),
               MovementTestData.CreateData(6, '6', '7', Tru, Fls, Fls, Fls),
               MovementTestData.CreateData(7, '7', '8', Fls, Fls, Fls, Fls),
               MovementTestData.CreateData(8, '8', '9', Fls, Fls, Fls, Fls),
               MovementTestData.CreateData(9, '9', NUL, Fls, Tru, Fls, Tru),
             };
    }

    [Theory]
    [MemberData(nameof(GetMoveForwardData))]
    public void MoveForward_ReportsStartAndEndCorrectly(MovementTestData testData)
    {
      var content = CreateContent("123", "456", "789");

      var cursor = content.GetCaretAtBeginning().MoveCursorForwardBy(testData.AmountToMove);

      DidYouKnow.That(cursor.IsAtBeginningOfFragment)
                .Should().Be(testData.IsAtFragmentStart);

      DidYouKnow.That(cursor.IsAtBeginningOfBlock)
                .Should().Be(testData.IsAtBlockStart);

      DidYouKnow.That(cursor.IsAtEndOfFragment)
                .Should().Be(testData.IsAtFragmentEnd);

      DidYouKnow.That(cursor.IsAtEndOfBlock)
                .Should().Be(testData.IsAtBlockEnd);
    }

    [Theory]
    [MemberData(nameof(GetMoveForwardData))]
    public void MoveForwardWorks(MovementTestData testData)
    {
      Console.WriteLine(testData);

      var content = CreateContent("123", "456", "789");

      var cursor = content.GetCaretAtBeginning().MoveCursorForwardBy(testData.AmountToMove);

      DidYouKnow.That(cursor.CharacterAfter.Character)
                .Should().Be(testData.ExpectedAfterChar);

      DidYouKnow.That(cursor.GetCharacterBefore().Character)
                .Should().Be(testData.ExpectedBeforeChar);
    }

    public static TheoryData<MovementTestData> GetMoveBackwardData()
    {
      return new TheoryData<MovementTestData>()
             {
               MovementTestData.CreateData(9, NUL, '1', Tru, Fls, Tru, Fls),
               MovementTestData.CreateData(8, '1', '2', Fls, Fls, Fls, Fls),
               MovementTestData.CreateData(7, '2', '3', Fls, Tru, Fls, Fls),
               MovementTestData.CreateData(6, '3', '4', Tru, Fls, Fls, Fls),
               MovementTestData.CreateData(5, '4', '5', Fls, Fls, Fls, Fls),
               MovementTestData.CreateData(4, '5', '6', Fls, Tru, Fls, Fls),
               MovementTestData.CreateData(3, '6', '7', Tru, Fls, Fls, Fls),
               MovementTestData.CreateData(2, '7', '8', Fls, Fls, Fls, Fls),
               MovementTestData.CreateData(1, '8', '9', Fls, Fls, Fls, Fls),
               MovementTestData.CreateData(0, '9', NUL, Fls, Tru, Fls, Tru),
             };
    }

    [Theory]
    [MemberData(nameof(GetMoveBackwardData))]
    public void MoveBackward_ReportsStartAndEndCorrectly(MovementTestData testData)
    {
      var content = CreateContent("123", "456", "789");

      var cursor = content.GetCursorToEnd().MoveCursorBackwardBy(testData.AmountToMove);

      DidYouKnow.That(cursor.IsAtBeginningOfFragment)
                .Should().Be(testData.IsAtFragmentStart);

      DidYouKnow.That(cursor.IsAtBeginningOfBlock)
                .Should().Be(testData.IsAtBlockStart);

      DidYouKnow.That(cursor.IsAtEndOfFragment)
                .Should().Be(testData.IsAtFragmentEnd);

      DidYouKnow.That(cursor.IsAtEndOfBlock)
                .Should().Be(testData.IsAtBlockEnd);
    }

    [Theory]
    [MemberData(nameof(GetMoveBackwardData))]
    public void MoveBackwardWorks(MovementTestData testData)
    {
      var content = CreateContent("123", "456", "789");

      var cursor = content.GetCursorToEnd().MoveCursorBackwardBy(testData.AmountToMove);

      DidYouKnow.That(cursor.CharacterAfter.Character)
        .Should().Be(testData.ExpectedAfterChar);

      DidYouKnow.That(cursor.GetCharacterBefore().Character)
        .Should().Be(testData.ExpectedBeforeChar);
    }
  }
}