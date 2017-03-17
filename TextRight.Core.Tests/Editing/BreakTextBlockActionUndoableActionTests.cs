using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using TextRight.Core.Editing.Actions;
using TextRight.Core.Editing.Actions.Text;
using TextRight.Core.Editing.Commands.Text;
using TextRight.Core.ObjectModel.Blocks.Text;

namespace TextRight.Core.Tests.Editing
{
  public class BreakTextBlockActionUndoableActionTests : UndoBasedTest
  {
    [Test]
    public void BreakAtEndOfBlock_BreaksIntoTwo()
    {
      var it = DoAll(new Func<UndoableAction>[]
                     {
                       FromCommand<InsertTextCommand, string>(() => BlockAt(0).EndCursor().ToHandle(), "Start of text"),
                       FromCommand<BreakTextBlockCommand>(() => BlockAt(0).EndCursor().ToHandle()),
                     }
      );

      Assert.That(Document.Root.ChildCount, Is.EqualTo(2));
      Assert.That(BlockAt(0), Is.InstanceOf<TextBlock>());
      Assert.That(BlockAt(1), Is.InstanceOf<TextBlock>());

      Assert.That(BlockAt(0).As<TextBlock>().AsText(), Is.EqualTo("Start of text"));
      Assert.That(BlockAt(1).As<TextBlock>().AsText(), Is.EqualTo(""));

      it.VerifyUndo();
    }

    [Test]
    public void BreakAtBeginning_BreaksIntoTwo()
    {
      var it = DoAll(new Func<UndoableAction>[]
                     {
                       FromCommand<InsertTextCommand, string>(() => BlockAt(0).EndCursor().ToHandle(), "Start of text"),
                       FromCommand<BreakTextBlockCommand>(() => BlockAt(0).BeginCursor().ToHandle()),
                     });

      Assert.That(Document.Root.ChildCount, Is.EqualTo(2));
      Assert.That(BlockAt(0), Is.InstanceOf<TextBlock>());
      Assert.That(BlockAt(1), Is.InstanceOf<TextBlock>());

      Assert.That(BlockAt(0).As<TextBlock>().AsText(), Is.EqualTo(""));
      Assert.That(BlockAt(1).As<TextBlock>().AsText(), Is.EqualTo("Start of text"));

      it.VerifyUndo();
    }

    [Test]
    public void BreakInMiddle_BreaksIntoTwo()
    {
      var it = DoAll(new Func<UndoableAction>[]
                     {
                       FromCommand<InsertTextCommand, string>(() => BlockAt(0).EndCursor().ToHandle(), "Start of text"),
                       FromCommand<BreakTextBlockCommand>(() => BlockAt(0).BeginCursor(5).ToHandle()),
                     });

      Assert.That(Document.Root.ChildCount, Is.EqualTo(2));
      Assert.That(BlockAt(0), Is.InstanceOf<TextBlock>());
      Assert.That(BlockAt(1), Is.InstanceOf<TextBlock>());

      Assert.That(BlockAt(0).As<TextBlock>().AsText(), Is.EqualTo("Start"));
      Assert.That(BlockAt(1).As<TextBlock>().AsText(), Is.EqualTo(" of text"));

      it.VerifyUndo();
    }

    [Test]
    public void Undo_RestoresInitialState()
    {
      DoAllAndThenUndo(new Func<UndoableAction>[]
                       {
                         // Block 1
                         FromCommand<InsertTextCommand, string>(() => BlockAt(0).EndCursor().ToHandle(), "The text"),
                         FromCommand<InsertTextCommand, string>(() => BlockAt(0).EndCursor().ToHandle(), "More text"),
                         FromCommand<BreakTextBlockCommand>(() => BlockAt(0).EndCursor().ToHandle()),
                         // Block 2
                         FromCommand<InsertTextCommand, string>(() => BlockAt(1).BeginCursor().ToHandle(), "More text"),
                         FromCommand<InsertTextCommand, string>(() => BlockAt(1).BeginCursor(1).ToHandle(), "The text"),
                         FromCommand<BreakTextBlockCommand>(() => BlockAt(1).EndCursor().ToHandle()),
                         // Block 3
                         FromCommand<InsertTextCommand, string>(() => BlockAt(2).BeginCursor().ToHandle(), "More text"),
                         FromCommand<InsertTextCommand, string>(() => BlockAt(2).BeginCursor(1).ToHandle(), "The text"),
                       }
      );
    }
  }
}