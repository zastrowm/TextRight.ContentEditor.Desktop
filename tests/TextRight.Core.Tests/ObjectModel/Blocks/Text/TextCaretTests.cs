using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using TextRight.Core.ObjectModel.Blocks.Text;
using TextRight.Core.Tests.Framework;
using Xunit;
using Xunit.Abstractions;

namespace TextRight.Core.Tests.ObjectModel.Blocks.Text
{
  public class TextCaretTests
  {
    private readonly ITestOutputHelper _testOutputHelper;

    public TextCaretTests(ITestOutputHelper testOutputHelper)
    {
      _testOutputHelper = testOutputHelper;
    }

    [Fact]
    public void CharactersAreReportedCorrectly_AtBeginning()
    {
      var text = "0123456789";
      var content = CreateContent(text);

      var cursor = content.GetCaretAtStart();

      DidYouKnow.That(cursor.IsAtBlockStart).Should().BeTrue();
      DidYouKnow.That(cursor.IsAtBlockEnd).Should().BeFalse();

      DidYouKnow.That(cursor.Offset.GraphemeOffset).Should().Be(0);
      DidYouKnow.That(cursor.CharacterAfter.Text).Should().Be("0");
      TextUnit tempQualifier = cursor.GetCharacterBefore();
      DidYouKnow.That(tempQualifier.Text).Should().Be(Nul);
    }

    [Fact]
    public void CharactersAreReportedCorrectly_AtEnd()
    {
      var text = "0123456789";
      var content = CreateContent(text);

      var cursor = content.GetCaretAtEnd();

      DidYouKnow.That(cursor.IsAtBlockStart).Should().BeFalse();
      DidYouKnow.That(cursor.IsAtBlockEnd).Should().BeTrue();

      DidYouKnow.That(cursor.Offset.GraphemeOffset).Should().Be(10);
      DidYouKnow.That(cursor.CharacterAfter.Text).Should().Be(null);
      TextUnit tempQualifier = cursor.GetCharacterBefore();
      DidYouKnow.That(tempQualifier.Text).Should().Be("9");
    }

    private static TextBlockContent CreateContent(string text)
    {
      var content = new TextBlockContent();
      content.Insert(content.GetCaretAtStart(), text);
      return content;
    }

    [Theory]
    [RangedData(1, 9)]
    public void CharactersAreReportedCorrectly_ForMiddleIndices(int index)
    {
      var text = "0123456789";
      var expectedRightCharacter = text[index].ToString();
      var expectedLeftCharacter = text[index - 1].ToString();

      var content = CreateContent(text);

      var cursor = content.GetCaretAtStart().MoveCursorForwardBy(index);

      DidYouKnow.That(cursor.Offset.GraphemeOffset).Should().Be(index);
      DidYouKnow.That(cursor.CharacterAfter.Text).Should().Be(expectedRightCharacter);
      TextUnit tempQualifier = cursor.GetCharacterBefore();
      DidYouKnow.That(tempQualifier.Text).Should().Be(expectedLeftCharacter);
    }

    [Fact]
    public void CharactersAreReportedCorrectly_AfterMoving10()
    {
      var text = "0123456789";
      var content = CreateContent(text);

      var cursor = content.GetCaretAtStart().MoveCursorForwardBy(10);

      DidYouKnow.That(cursor).Should().Be(content.GetCaretAtEnd());
      DidYouKnow.That(cursor.Offset.GraphemeOffset).Should().Be(10);
      DidYouKnow.That(cursor.CharacterAfter.Text).Should().Be(null);
      TextUnit tempQualifier = cursor.GetCharacterBefore();
      DidYouKnow.That(tempQualifier.Text).Should().Be("9");
    }

    // odd naming to keep them all 3 characters for alignment below
    private const string Nul = null;
    private const bool Tru = true;
    private const bool Fls = false;

    public class MovementTestData : SerializableTestData<MovementTestData>
    {
      public int AmountToMove;
      public string ExpectedBeforeChar;
      public string ExpectedAfterChar;
      public bool IsAtBlockStart;
      public bool IsAtBlockEnd;

      public override string ToString()
        => $"MoveBy: {AmountToMove}, Character: {ExpectedAfterChar}";

      public static MovementTestData CreateData(int amountToMove,
                                                string expectedBeforeChar,
                                                string expectedAfterChar,
                                                bool isAtBlockStart,
                                                bool isAtBlockEnd)
        => new MovementTestData()
           {
             AmountToMove = amountToMove,
             ExpectedBeforeChar = expectedBeforeChar,
             ExpectedAfterChar = expectedAfterChar,
             IsAtBlockStart = isAtBlockStart,
             IsAtBlockEnd = isAtBlockEnd,
           };
    }

    public static TheoryData<MovementTestData> GetMoveForwardData()
    {
      return new TheoryData<MovementTestData>()
             {
               MovementTestData.CreateData(0, Nul, "1", Tru, Fls),
               MovementTestData.CreateData(1, "1", "2", Fls, Fls),
               MovementTestData.CreateData(2, "2", "3", Fls, Fls),
               MovementTestData.CreateData(3, "3", "4", Fls, Fls),
               MovementTestData.CreateData(4, "4", "5", Fls, Fls),
               MovementTestData.CreateData(5, "5", "6", Fls, Fls),
               MovementTestData.CreateData(6, "6", "7", Fls, Fls),
               MovementTestData.CreateData(7, "7", "8", Fls, Fls),
               MovementTestData.CreateData(8, "8", "9", Fls, Fls),
               MovementTestData.CreateData(9, "9", Nul, Fls, Tru),
             };
    }

    [Theory]
    [MemberData(nameof(GetMoveForwardData))]
    public void MoveForward_ReportsStartAndEndCorrectly(MovementTestData testData)
    {
      var content = CreateContent("123456789");

      var cursor = content.GetCaretAtStart().MoveCursorForwardBy(testData.AmountToMove);

      DidYouKnow.That(cursor.IsAtBlockStart)
                .Should().Be(testData.IsAtBlockStart);
      DidYouKnow.That(cursor.IsAtBlockEnd)
                .Should().Be(testData.IsAtBlockEnd);
    }

    [Theory]
    [MemberData(nameof(GetMoveForwardData))]
    public void MoveForwardWorks(MovementTestData testData)
    {
      _testOutputHelper.WriteLine(testData.ToString());

      var content = CreateContent("123456789");

      var cursor = content.GetCaretAtStart().MoveCursorForwardBy(testData.AmountToMove);

      DidYouKnow.That(cursor.CharacterAfter.Text)
                .Should().Be(testData.ExpectedAfterChar);

      TextUnit tempQualifier = cursor.GetCharacterBefore();
      DidYouKnow.That(tempQualifier.Text)
                .Should().Be(testData.ExpectedBeforeChar);
    }

    public static TheoryData<MovementTestData> GetMoveBackwardData()
    {
      return new TheoryData<MovementTestData>()
             {
               MovementTestData.CreateData(9, Nul, "1", Tru, Fls),
               MovementTestData.CreateData(8, "1", "2", Fls, Fls),
               MovementTestData.CreateData(7, "2", "3", Fls, Fls),
               MovementTestData.CreateData(6, "3", "4", Fls, Fls),
               MovementTestData.CreateData(5, "4", "5", Fls, Fls),
               MovementTestData.CreateData(4, "5", "6", Fls, Fls),
               MovementTestData.CreateData(3, "6", "7", Fls, Fls),
               MovementTestData.CreateData(2, "7", "8", Fls, Fls),
               MovementTestData.CreateData(1, "8", "9", Fls, Fls),
               MovementTestData.CreateData(0, "9", Nul, Fls, Tru),
             };
    }

    [Theory]
    [MemberData(nameof(GetMoveBackwardData))]
    public void MoveBackward_ReportsStartAndEndCorrectly(MovementTestData testData)
    {
      var content = CreateContent("123456789");

      var cursor = content.GetCaretAtEnd().MoveCursorBackwardBy(testData.AmountToMove);

      DidYouKnow.That(cursor.IsAtBlockStart)
                .Should().Be(testData.IsAtBlockStart);
      DidYouKnow.That(cursor.IsAtBlockEnd)
                .Should().Be(testData.IsAtBlockEnd);
    }

    [Theory]
    [MemberData(nameof(GetMoveBackwardData))]
    public void MoveBackwardWorks(MovementTestData testData)
    {
      var content = CreateContent("123456789");

      var cursor = content.GetCaretAtEnd().MoveCursorBackwardBy(testData.AmountToMove);

      DidYouKnow.That(cursor.CharacterAfter.Text)
        .Should().Be(testData.ExpectedAfterChar);

      TextUnit tempQualifier = cursor.GetCharacterBefore();
      DidYouKnow.That(tempQualifier.Text)
                .Should().Be(testData.ExpectedBeforeChar);
    }
  }
}