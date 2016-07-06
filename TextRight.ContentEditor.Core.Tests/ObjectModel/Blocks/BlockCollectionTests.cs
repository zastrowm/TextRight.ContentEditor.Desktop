using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;
using TextRight.ContentEditor.Core.ObjectModel.Blocks;

namespace TextRight.ContentEditor.Core.Tests.ObjectModel.Blocks
{
  internal class BlockCollectionTests
  {
    private BlockCollection _collection;

    [SetUp]
    public void Setup()
    {
      _collection = new AddableBlockCollection()
                    {
                      new TextBlock()
                        .WithText("This is line #1"),
                      new TextBlock()
                        .WithText("This is line #2")
                    }
        .RemoveFirstChilds();

      Assert.That(_collection.ChildCount, Is.EqualTo(2));
    }

    [Test]
    public void VerifyBreakInMiddleOfParagraph_SplitsIntoTwo()
    {
      var cursor = _collection.NthBlock(0).GetCursor();
      cursor.MoveToBeginning();
      cursor.Move(7); // should be after "is"

      var newBlock = _collection.TryBreakBlock(cursor);

      Assert.That(_collection.ChildCount, Is.EqualTo(3));
      Assert.That(_collection.NthBlock(0).AsText(), Is.EqualTo("This is"));
      Assert.That(_collection.NthBlock(1).AsText(), Is.EqualTo(" line #1"));
      Assert.That(_collection.NthBlock(2).AsText(), Is.EqualTo("This is line #2"));

      Assert.That(newBlock, Is.EqualTo(_collection.NthBlock(1)));
    }

    [Test]
    public void BreakInBeginning_MakesNewPreviousEmptyBlock()
    {
      var cursor = _collection.NthBlock(0).GetCursor();
      cursor.MoveToBeginning();

      var newBlock = _collection.TryBreakBlock(cursor);

      Assert.That(_collection.ChildCount, Is.EqualTo(3));
      Assert.That(_collection.NthBlock(0).AsText(), Is.EqualTo(""));
      Assert.That(_collection.NthBlock(1).AsText(), Is.EqualTo("This is line #1"));
      Assert.That(_collection.NthBlock(2).AsText(), Is.EqualTo("This is line #2"));

      Assert.That(newBlock, Is.EqualTo(_collection.NthBlock(1)));
    }

    [Test]
    public void BreakAtEnd_MakesNewNextEmptyBlock()
    {
      var cursor = _collection.NthBlock(0).GetCursor();
      cursor.MoveToEnd();

      var newBlock = _collection.TryBreakBlock(cursor);

      Assert.That(_collection.ChildCount, Is.EqualTo(3));
      Assert.That(_collection.NthBlock(0).AsText(), Is.EqualTo("This is line #1"));
      Assert.That(_collection.NthBlock(1).AsText(), Is.EqualTo(""));
      Assert.That(_collection.NthBlock(2).AsText(), Is.EqualTo("This is line #2"));

      Assert.That(newBlock, Is.EqualTo(_collection.NthBlock(1)));
    }
  }
}