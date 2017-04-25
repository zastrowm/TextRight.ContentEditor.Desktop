using System;
using System.Collections.Generic;
using System.Linq;

using NFluent;

using TextRight.Core.Editing.Actions;
using TextRight.Core.Editing.Actions.Text;
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

      Check.That(BlockAt(0).AsText()).IsEqualTo("ThWord");
      it.VerifyUndo();
    }

    [Fact]
    public void DeleteNextCharacter_WorksAtAllLocationsInTheParagraph()
    {
      string text = "TheWord";

      // we start at 1 and go up to (including Length) because we're deleting the previous character.
      for (int i = 1; i < text.Length + 1; i++)
      {
        var it = DoAll(
          new Func<UndoableAction>[]
          {
            FromCommand<InsertTextCommand, string>(() => BlockAt(0).BeginCursor().ToHandle(), text),
            FromCommand<DeletePreviousCharacterCommand>(() => BlockAt(0).BeginCursor(i).AsTextCursor().ToHandle()),
          });

        var expected = text.Remove(i - 1, 1);

        Check.That(BlockAt(0).AsText()).IsEqualTo(expected);
        it.VerifyUndo();
      }
    }
  }
}