using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using TextRight.Core.Actions;
using TextRight.Core.Blocks;
using TextRight.Core.Editing.Actions;
using TextRight.Core.Editing.Actions.Text;
using TextRight.Core.Editing.Commands.Text;
using TextRight.Core.ObjectModel.Blocks.Text;

namespace TextRight.Core.Tests.Editing
{
  public class ConvertParagraphIntoHeadingActionTests : UndoBasedTest
  {
    public override IReadOnlyList<Func<UndoableAction>> InitializeDocument()
    {
      return new Func<UndoableAction>[]
             {
               FromCommand<InsertTextCommand, string>(() => Context.Caret, "This is the text"),
             };
    }

    [Test]
    public void Action_Works()
    {
      var it = DoAll(new Func<UndoableAction>[]
                     {
                       () => new ConvertTextBlockIntoHeadingAction(Context.Caret.Cursor, 0),
                     });

      var heading = Document.Root.FirstBlock.As<HeadingBlock>();
      Assert.That(heading, Is.Not.Null);
      Assert.That(heading.HeadingLevel, Is.EqualTo(0));
      Assert.That(heading.AsText(), Is.EqualTo("This is the text"));

      it.VerifyUndo();
    }

    [Test]
    public void ExecutingOnHeading_SimplyChangesLevel()
    {
      DoAll(new Func<UndoableAction>[]
            {
              () => new ConvertTextBlockIntoHeadingAction(Context.Caret.Cursor, 0),
            });

      var heading = Document.Root.FirstBlock.As<HeadingBlock>();

      DoAll(new Func<UndoableAction>[]
            {
              () => new ConvertTextBlockIntoHeadingAction(Context.Caret.Cursor, 3),
            });

      Assert.That(heading, Is.SameAs(Document.Root.FirstBlock));
      Assert.That(heading.HeadingLevel, Is.EqualTo(3));
    }

    [Test]
    public void ExecutingOnHeading_IsUndoable()
    {
      var it = DoAll(new Func<UndoableAction>[]
                     {
                       () => new ConvertTextBlockIntoHeadingAction(Context.Caret.Cursor, 0),
                       () => new ConvertTextBlockIntoHeadingAction(Context.Caret.Cursor, 3),
                     });

      it.VerifyUndo();
    }
  }
}