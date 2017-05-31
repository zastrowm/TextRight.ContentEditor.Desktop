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
  public class BlockCollectionTests
  {
    private readonly BlockCollection _collection;

    public BlockCollectionTests()
    {
      _collection = new AddableBlockCollection()
                    {
                      new ParagraphBlock()
                        .WithText("This is line #1"),
                      new ParagraphBlock()
                        .WithText("This is line #2")
                    }
        .RemoveFirstChilds();

      DidYouKnow.That(_collection.ChildCount).Should().Be(2);
    }

    [Fact]
    public void VerifyBreakInMiddleOfParagraph_SplitsIntoTwo()
    {
      var cursor = _collection.NthBlock(0).GetCursor();
      cursor.MoveToBeginning();
      cursor.Move(7); // should be after "is"

      var newBlock = TextBlockHelperMethods.TryBreakBlock(cursor).Block;

      DidYouKnow.That(_collection.ChildCount).Should().Be(3);
      DidYouKnow.That(_collection.NthBlock(0).AsText()).Should().Be("This is");
      DidYouKnow.That(_collection.NthBlock(1).AsText()).Should().Be(" line #1");
      DidYouKnow.That(_collection.NthBlock(2).AsText()).Should().Be("This is line #2");

      DidYouKnow.That(newBlock).Should().Be(_collection.NthBlock(1));
    }

    [Fact]
    public void BreakInBeginning_MakesNewPreviousEmptyBlock()
    {
      var cursor = _collection.NthBlock(0).GetCursor();
      cursor.MoveToBeginning();

      var newBlock = TextBlockHelperMethods.TryBreakBlock(cursor).Block;

      DidYouKnow.That(_collection.ChildCount).Should().Be(3);
      DidYouKnow.That(_collection.NthBlock(0).AsText()).Should().Be("");
      DidYouKnow.That(_collection.NthBlock(1).AsText()).Should().Be("This is line #1");
      DidYouKnow.That(_collection.NthBlock(2).AsText()).Should().Be("This is line #2");

      DidYouKnow.That(newBlock).Should().Be(_collection.NthBlock(1));
    }

    [Fact]
    public void BreakAtEnd_MakesNewNextEmptyBlock()
    {
      var cursor = _collection.NthBlock(0).GetCursor();
      cursor.MoveToEnd();

      var newBlock = TextBlockHelperMethods.TryBreakBlock(cursor).Block;

      DidYouKnow.That(_collection.ChildCount).Should().Be(3);
      DidYouKnow.That(_collection.NthBlock(0).AsText()).Should().Be("This is line #1");
      DidYouKnow.That(_collection.NthBlock(1).AsText()).Should().Be("");
      DidYouKnow.That(_collection.NthBlock(2).AsText()).Should().Be("This is line #2");

      DidYouKnow.That(newBlock).Should().Be(_collection.NthBlock(1));
    }
  }
}