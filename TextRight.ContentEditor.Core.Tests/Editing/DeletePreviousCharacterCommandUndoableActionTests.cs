using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using TextRight.ContentEditor.Core.Editing;
using TextRight.ContentEditor.Core.Editing.Actions;

namespace TextRight.ContentEditor.Core.Tests.Editing
{
  internal class DeletePreviousCharacterCommandUndoableActionTests : UndoBasedTest
  {
    [Test]
    public void VerifyItWorks()
    {
      var it = DoAll(
        new Func<UndoableAction>[]
        {
          () => new InsertTextUndoableAction(BlockAt(0).BeginCursor().ToHandle(), "TheWord"),
          () => new DeletePreviousCharacterAction(BlockAt(0).BeginCursor(3).AsTextCursor()),
        });

      Assert.That(BlockAt(0).AsText(), Is.EqualTo("ThWord"));
      it.VerifyUndo();
    }

    [Test]
    public void DeleteNextCharacter_WorksAtAllLocationsInTheParagraph()
    {
      string text = "TheWord";

      // we start at 1 and go up to (including Length) because we're deleting the previous character.
      for (int i = 1; i < text.Length + 1; i++)
      {
        var it = DoAll(
          new Func<UndoableAction>[]
          {
            () => new InsertTextUndoableAction(BlockAt(0).BeginCursor().ToHandle(), text),
            () => new DeletePreviousCharacterAction(BlockAt(0).BeginCursor(i).AsTextCursor()),
          });

        var expected = text.Remove(i - 1, 1);

        Assert.That(BlockAt(0).AsText(), Is.EqualTo(expected));
        it.VerifyUndo();
      }
    }
  }
}