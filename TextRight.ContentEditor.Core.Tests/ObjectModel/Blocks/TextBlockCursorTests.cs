using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;
using TextRight.ContentEditor.Desktop.ObjectModel.Blocks;

namespace TextRight.ContentEditor.Desktop.Tests.ObjectModel.Blocks
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
  }
}