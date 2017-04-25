using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using NFluent;

using TextRight.Core.Editing.Actions.Text;
using TextRight.Core.ObjectModel.Blocks.Collections;
using TextRight.Core.ObjectModel.Blocks.Text;

using Xunit;

namespace TextRight.Core.Tests.ObjectModel.Blocks
{
  public class BlockCollectionTests
  {
    private BlockCollection _collection;

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

      Check.That(_collection.ChildCount).IsEqualTo(2);
    }

    [Fact]
    public void VerifyBreakInMiddleOfParagraph_SplitsIntoTwo()
    {
      var cursor = _collection.NthBlock(0).GetCursor();
      cursor.MoveToBeginning();
      cursor.Move(7); // should be after "is"

      var newBlock = TextBlockHelperMethods.TryBreakBlock(cursor);

      Check.That(_collection.ChildCount).IsEqualTo(3);
      Check.That(_collection.NthBlock(0).AsText()).IsEqualTo("This is");
      Check.That(_collection.NthBlock(1).AsText()).IsEqualTo(" line #1");
      Check.That(_collection.NthBlock(2).AsText()).IsEqualTo("This is line #2");

      Check.That(newBlock).IsEqualTo(_collection.NthBlock(1));
    }

    [Fact]
    public void BreakInBeginning_MakesNewPreviousEmptyBlock()
    {
      var cursor = _collection.NthBlock(0).GetCursor();
      cursor.MoveToBeginning();

      var newBlock = TextBlockHelperMethods.TryBreakBlock(cursor);

      Check.That(_collection.ChildCount).IsEqualTo(3);
      Check.That(_collection.NthBlock(0).AsText()).IsEqualTo("");
      Check.That(_collection.NthBlock(1).AsText()).IsEqualTo("This is line #1");
      Check.That(_collection.NthBlock(2).AsText()).IsEqualTo("This is line #2");

      Check.That(newBlock).IsEqualTo(_collection.NthBlock(1));
    }

    [Fact]
    public void BreakAtEnd_MakesNewNextEmptyBlock()
    {
      var cursor = _collection.NthBlock(0).GetCursor();
      cursor.MoveToEnd();

      var newBlock = TextBlockHelperMethods.TryBreakBlock(cursor);

      Check.That(_collection.ChildCount).IsEqualTo(3);
      Check.That(_collection.NthBlock(0).AsText()).IsEqualTo("This is line #1");
      Check.That(_collection.NthBlock(1).AsText()).IsEqualTo("");
      Check.That(_collection.NthBlock(2).AsText()).IsEqualTo("This is line #2");

      Check.That(newBlock).IsEqualTo(_collection.NthBlock(1));
    }
  }
}