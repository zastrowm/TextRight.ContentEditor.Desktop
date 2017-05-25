using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using TextRight.Core.Editing.Actions;
using TextRight.Core.Editing.Commands.Text;
using Xunit;

namespace TextRight.Core.Tests.Editing
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
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    [InlineData(4)]
    [InlineData(5)]
    [InlineData(6)]
    [InlineData(7)]
      // we start at 1 and go up to (including Length) because we're deleting the previous character.
    public void DeleteNextCharacter_WorksAtAllLocationsInTheParagraph(int index)
    {
      string text = "TheWord";

      var it = DoAll(
        new Func<UndoableAction>[]
        {
          FromCommand<InsertTextCommand, string>(() => BlockAt(0).BeginCursor().ToHandle(), text),
          FromCommand<DeletePreviousCharacterCommand>(() => BlockAt(0).BeginCursor(index).AsTextCursor().ToHandle()),
        });

      var expected = text.Remove(index - 1, 1);

      DidYouKnow.That(BlockAt(0).AsText()).Should().Be(expected);
      it.VerifyUndo();
    }
  }
}