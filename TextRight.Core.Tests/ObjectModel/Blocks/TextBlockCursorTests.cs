using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using NFluent;

using TextRight.Core.ObjectModel.Blocks.Text;

using Xunit;

namespace TextRight.Core.Tests.ObjectModel.Blocks
{
  public class TextBlockContentTests
  {
    public StyledTextFragment a,
                              b,
                              c,
                              d,
                              e;

    public TextBlockContent Content;

    public TextBlockContentTests()
    {
      Content = new TextBlockContent();
      Content.AppendSpan((a = new StyledTextFragment("123", "a")));
      Content.AppendSpan((b = new StyledTextFragment("456", "b")));
      Content.AppendSpan((c = new StyledTextFragment("789", "c")));
    }

    public static TheoryData<int, int> VerifyExtractionEverywhereData()
    {
      IEnumerable<(int, int)> Generate()
      {
        for (int start = 0; start < 9; start++)
        {
          for (int end = start; end < 9; end++)
          {
            yield return (start, end);
          }
        }
      }

      var data = new TheoryData<int, int>();

      foreach (var item in Generate())
      {
        data.Add(item.Item1, item.Item2);
      }

      return data;
    }

    [Theory]
    [MemberData(nameof(VerifyExtractionEverywhereData))]
    public void VerifyExtractionEverywhere(int start, int end)
    {
      var originalText = Content.AsText();

      var extracted = Content.ExtractContent(Content.CursorFromCharacterIndex(start),
                                             Content.CursorFromCharacterIndex(end));

      var removedText = originalText.Substring(start, end - start);
      var modifiedText = originalText.Remove(start, end - start);

      Check.That(Content.AsText()).IsEqualTo(modifiedText);
      Check.That(extracted.AsText().Equals(removedText));
    }
  }

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

      Check.That(cursor.CharacterBefore).IsEqualTo('\0');
      Check.That(cursor.CharacterAfter).IsEqualTo('1');
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

      Check.That(cursor.CharacterBefore).IsEqualTo(beforeChar);
      Check.That(cursor.CharacterAfter).IsEqualTo(afterChar);
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

      Check.That(cursor.CharacterBefore).IsEqualTo(beforeChar);
      Check.That(cursor.CharacterAfter).IsEqualTo(afterChar);
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

      Check.That(spans.Length).IsEqualTo(2);
      Check.That(spans[0]).IsEqualTo(b);
      Check.That(spans[1]).IsEqualTo(c);

      Check.That(Block.Content.ChildCount).IsEqualTo(1);
      Check.That(Block.Content.Fragments.First()).IsEqualTo(a);
    }

    [Fact]
    public void DetachingAtMiddleOfSpan_DetachesIntoNewFragment()
    {
      var cursor = GetCursor(2);
      var spans = cursor.ExtractToEnd();

      Check.That(spans.Length).IsEqualTo(3);
      Check.That(spans[0].GetText()).IsEqualTo("3");
      Check.That(spans[1]).IsEqualTo(b);
      Check.That(spans[2]).IsEqualTo(c);

      Check.That(Block.Content.ChildCount).IsEqualTo(1);
      Check.That(Block.Content.Fragments.First()).IsEqualTo(a);
      Check.That(Block.Content.Fragments.First().GetText()).IsEqualTo("12");
    }

    [Fact]
    public void DetachingAtBeginningOfFirstSpan_DetachesAllFragments()
    {
      var cursor = GetCursor(0);
      var spans = cursor.ExtractToEnd();

      Check.That(spans.Length).IsEqualTo(3);
      Check.That(spans[0].GetText()).IsEqualTo("123");
      Check.That(spans[1]).IsEqualTo(b);
      Check.That(spans[2]).IsEqualTo(c);

      Check.That(Block.Content.ChildCount).IsEqualTo(1);
      Check.That(Block.Content.Fragments.First().GetText()).IsEqualTo("");
    }

    [Fact]
    public void DetachingAtEndOfLastSpan_DetachesNoFragments()
    {
      var cursor = GetCursor(9);
      var spans = cursor.ExtractToEnd();

      Check.That(spans.Length).IsEqualTo(1);
      Check.That(spans[0].GetText()).IsEqualTo("");

      Check.That(Block.Content.ChildCount).IsEqualTo(3);
      Check.That(Block.Content.Fragments.ElementAt(0)).IsEqualTo(a);
      Check.That(Block.Content.Fragments.ElementAt(0).GetText()).IsEqualTo("123");
      Check.That(Block.Content.Fragments.ElementAt(1)).IsEqualTo(b);
      Check.That(Block.Content.Fragments.ElementAt(1).GetText()).IsEqualTo("456");
      Check.That(Block.Content.Fragments.ElementAt(2)).IsEqualTo(c);
      Check.That(Block.Content.Fragments.ElementAt(2).GetText()).IsEqualTo("789");
    }
  }
}