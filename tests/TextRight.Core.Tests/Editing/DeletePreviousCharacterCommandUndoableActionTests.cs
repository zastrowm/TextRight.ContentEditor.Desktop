using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using TextRight.Core.Actions;
using TextRight.Core.Commands.Text;
using Xunit;

namespace TextRight.Core.Tests
{
  public class DeletePreviousCharacterCommandUndoableActionTests : UndoBasedTest
  {
    [Fact]
    public void VerifyItWorks()
    {
      var it = DoAll(
        new Func<UndoableAction>[]
        {
          FromCommand<InsertTextCommand, string>(() => BlockAt(0).BeginCursor().ToHandle(), "TheWord"),
          FromCommand<DeletePreviousCharacterCommand>(() => BlockAt(0).BeginCursor(3).AsTextCursor().ToHandle()),
        });

      DidYouKnow.That(BlockAt(0).AsText()).Should().Be("ThWord");
      it.VerifyUndo();
    }
    
    [Theory]
    [MemberData(nameof(DeleteNextCharacterCommandUndoableActionTests.DeleteCharacterTestCases),
                MemberType = typeof(DeleteNextCharacterCommandUndoableActionTests))]
    public void DeleteNextCharacter_WorksAtAllLocationsInTheParagraph(string text, int deleteIndex, string expected)
    {
      var it = DoAll(
        new Func<UndoableAction>[]
        {
          FromCommand<InsertTextCommand, string>(() => BlockAt(0).BeginCursor().ToHandle(), text),
          () =>
          {
            var caretLocation = BlockAt(0).BeginCaret(deleteIndex + 1);
            return new DeletePreviousCharacterCommand.DeletePreviousCharacterAction(caretLocation.AsTextCursor());
          },
        });

      DidYouKnow.That(BlockAt(0).AsText()).Should().Be(expected);
      it.VerifyUndo();
    }
  }
}