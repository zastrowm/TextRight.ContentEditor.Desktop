using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using TextRight.Core.Editing.Actions;
using TextRight.Core.Editing.Commands.Text;
using Xunit;

namespace TextRight.Core.Tests.Editing
{
  public class DeleteNextCharacterCommandUndoableActionTests : UndoBasedTest
  {
    [Fact]
    public void VerifyItWorks()
    {
      var it = DoAll(
        new Func<UndoableAction>[]
        {
          FromCommand<InsertTextCommand, string>(() => BlockAt(0).BeginCursor().ToHandle(), "TheWord"),
          () => new DeleteNextCharacterCommand.DeleteNextCharacterAction(BlockAt(0).BeginCaret(3).AsTextCursor()),
        });

      DidYouKnow.That(BlockAt(0).AsText()).Should().Be("Theord");
      it.VerifyUndo();
    }

    [Fact]
    public void DeleteNextCharacter_WorksAtAllLocationsInTheParagraph()
    {
      string text = "TheWord";

      // we start at 0 and go up to length (but not including) because we're deleting the next
      // character. 
      for (int i = 0; i < text.Length; i++)
      {
        var it = DoAll(
          new Func<UndoableAction>[]
          {
            FromCommand<InsertTextCommand, string>(() => BlockAt(0).BeginCursor().ToHandle(), text),
            () => new DeleteNextCharacterCommand.DeleteNextCharacterAction(BlockAt(0).BeginCaret(i).AsTextCursor()),
          });

        var expected = text.Remove(i, 1);

        DidYouKnow.That(BlockAt(0).AsText()).Should().Be(expected);
        it.VerifyUndo();
      }
    }
  }
}