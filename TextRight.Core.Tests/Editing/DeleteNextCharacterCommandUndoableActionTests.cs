using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;
using TextRight.Core.Editing.Actions;
using TextRight.Core.Editing.Actions.Text;
using TextRight.Core.Editing.Commands.Text;

namespace TextRight.Core.Tests.Editing
{
  internal class DeleteNextCharacterCommandUndoableActionTests : UndoBasedTest
  {
    [Test]
    public void VerifyItWorks()
    {
      var it = DoAll(
        new Func<UndoableAction>[]
        {
          FromCommand<InsertTextCommand, string>(() => BlockAt(0).BeginCursor().ToHandle(), "TheWord"),
          () => new DeleteNextCharacterCommand.DeleteNextCharacterAction(BlockAt(0).BeginCursor(3).AsTextCursor()),
        });

      Assert.That(BlockAt(0).AsText(), Is.EqualTo("Theord"));
      it.VerifyUndo();
    }

    [Test]
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
            () => new DeleteNextCharacterCommand.DeleteNextCharacterAction(BlockAt(0).BeginCursor(i).AsTextCursor()),
          });

        var expected = text.Remove(i, 1);

        Assert.That(BlockAt(0).AsText(), Is.EqualTo(expected));
        it.VerifyUndo();
      }
    }
  }
}