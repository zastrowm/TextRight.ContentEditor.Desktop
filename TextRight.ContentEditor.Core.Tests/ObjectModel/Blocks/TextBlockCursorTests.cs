using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;
using TextRight.ContentEditor.Core.ObjectModel.Blocks;

namespace TextRight.ContentEditor.Core.Tests.ObjectModel.Blocks
{
  public class TextBlockCursorTests
  {
    public StyledTextFragment a, b, c, d, e;

    public TextBlock Block;

    [SetUp]
    public void Setup()
    {
      Block = new TextBlock()
              {
                (a = new StyledTextFragment("123")),
                (b = new StyledTextFragment("456")),
                (c = new StyledTextFragment("789")),
              };

      // The first TextSpan was auto added
      Block.RemoveSpan(Block.First());
    }

    [Test]
    public void BeginningPointsToBeginning()
    {
      var cursor = (TextBlock.TextBlockCursor)Block.GetCursor();
      cursor.MoveToBeginning();

      Assert.That(cursor.CharacterBefore, Is.EqualTo('\0'));
      Assert.That(cursor.CharacterAfter, Is.EqualTo('1'));
    }

    [TestCase(0, '\0', '1')]
    [TestCase(1, '1', '2')]
    [TestCase(2, '2', '3')]
    [TestCase(3, '3', '\0', Description = "End of first span")]
    [TestCase(4, '4', '5')]
    [TestCase(5, '5', '6')]
    [TestCase(6, '6', '\0', Description = "End of second span")]
    [TestCase(7, '7', '8')]
    [TestCase(8, '8', '9')]
    [TestCase(9, '9', '\0', Description = "End of third span")]
    public void MoveForwardWorks(int amountToMove, char beforeChar, char afterChar)
    {
      var cursor = (TextBlock.TextBlockCursor)Block.GetCursor();
      cursor.MoveToBeginning();

      for (int i = 0; i < amountToMove; i++)
      {
        cursor.MoveForward();
      }

      Assert.That(cursor.CharacterBefore, Is.EqualTo(beforeChar));
      Assert.That(cursor.CharacterAfter, Is.EqualTo(afterChar));
    }

    [TestCase(9, '\0', '1')]
    [TestCase(8, '1', '2')]
    [TestCase(7, '2', '3')]
    [TestCase(6, '3', '\0', Description = "End of first span")]
    [TestCase(5, '4', '5')]
    [TestCase(4, '5', '6')]
    [TestCase(3, '6', '\0', Description = "End of second span")]
    [TestCase(2, '7', '8')]
    [TestCase(1, '8', '9')]
    [TestCase(0, '9', '\0', Description = "End of third span")]
    public void MoveBackwardWorks(int amountToMove, char beforeChar, char afterChar)
    {
      var cursor = (TextBlock.TextBlockCursor)Block.GetCursor();
      cursor.MoveToEnd();

      for (int i = 0; i < amountToMove; i++)
      {
        cursor.MoveBackward();
      }

      Assert.That(cursor.CharacterBefore, Is.EqualTo(beforeChar));
      Assert.That(cursor.CharacterAfter, Is.EqualTo(afterChar));
    }

    private TextBlock.TextBlockCursor GetCursor(int amount)
    {
      var cursor = (TextBlock.TextBlockCursor)Block.GetCursor();
      cursor.MoveToBeginning();

      while (amount > 0)
      {
        cursor.MoveForward();
        amount -= 1;
      }
      return cursor;
    }

    [Test]
    public void DetachingAtEndOfFirstSpan_DetachesIntoTwoGroups()
    {
      var cursor = GetCursor(3);
      var spans = cursor.ExtractToEnd();

      Assert.That(spans.Length, Is.EqualTo(2));
      Assert.That(spans[0], Is.EqualTo(b));
      Assert.That(spans[1], Is.EqualTo(c));

      Assert.That(Block.ChildCount, Is.EqualTo(1));
      Assert.That(Block.First(), Is.EqualTo(a));
    }

    [Test]
    public void DetachingAtMiddleOfSpan_DetachesIntoNewFragment()
    {
      var cursor = GetCursor(2);
      var spans = cursor.ExtractToEnd();

      Assert.That(spans.Length, Is.EqualTo(3));
      Assert.That(spans[0].Text, Is.EqualTo("3"));
      Assert.That(spans[1], Is.EqualTo(b));
      Assert.That(spans[2], Is.EqualTo(c));

      Assert.That(Block.ChildCount, Is.EqualTo(1));
      Assert.That(Block.First(), Is.EqualTo(a));
      Assert.That(Block.First().Text, Is.EqualTo("12"));
    }

    [Test]
    public void DetachingAtBeginningOfFirstSpan_DetachesAllFragments()
    {
      var cursor = GetCursor(0);
      var spans = cursor.ExtractToEnd();

      Assert.That(spans.Length, Is.EqualTo(3));
      Assert.That(spans[0].Text, Is.EqualTo("123"));
      Assert.That(spans[1], Is.EqualTo(b));
      Assert.That(spans[2], Is.EqualTo(c));

      Assert.That(Block.ChildCount, Is.EqualTo(1));
      Assert.That(Block.First(), Is.EqualTo(a));
      Assert.That(Block.First().Text, Is.EqualTo(""));
    }

    [Test]
    public void DetachingAtEndOfLastSpan_DetachesNoFragments()
    {
      var cursor = GetCursor(9);
      var spans = cursor.ExtractToEnd();

      Assert.That(spans.Length, Is.EqualTo(0));

      Assert.That(Block.ChildCount, Is.EqualTo(3));
      Assert.That(Block.ElementAt(0), Is.EqualTo(a));
      Assert.That(Block.ElementAt(0).Text, Is.EqualTo("123"));
      Assert.That(Block.ElementAt(1), Is.EqualTo(b));
      Assert.That(Block.ElementAt(1).Text, Is.EqualTo("456"));
      Assert.That(Block.ElementAt(2), Is.EqualTo(c));
      Assert.That(Block.ElementAt(2).Text, Is.EqualTo("789"));
    }
  }
}