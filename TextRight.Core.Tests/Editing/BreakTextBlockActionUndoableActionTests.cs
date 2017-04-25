using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using NFluent;
using TextRight.Core.Editing.Actions;
using TextRight.Core.Editing.Commands.Text;
using TextRight.Core.ObjectModel.Blocks.Text;
using Xunit;

namespace TextRight.Core.Tests.Editing
{
  public class BreakTextBlockActionUndoableActionTests : UndoBasedTest
  {
    [Fact]
    public void BreakAtEndOfBlock_BreaksIntoTwo()
    {
      var it = DoAll(new Func<UndoableAction>[]
                     {
                       FromCommand<InsertTextCommand, string>(() => BlockAt(0).EndCursor().ToHandle(), "Start of text"),
                       FromCommand<BreakTextBlockCommand>(() => BlockAt(0).EndCursor().ToHandle()),
                     }
      );

      Check.That(Document.Root.ChildCount).IsEqualTo(2);
      Check.That(BlockAt(0)).InheritsFrom<TextBlock>();
      Check.That(BlockAt(1)).InheritsFrom<TextBlock>();

      Check.That(BlockAt(0).As<TextBlock>().AsText()).IsEqualTo("Start of text");
      Check.That(BlockAt(1).As<TextBlock>().AsText()).IsEqualTo("");

      it.VerifyUndo();
    }

    [Fact]
    public void BreakAtBeginning_BreaksIntoTwo()
    {
      var it = DoAll(new Func<UndoableAction>[]
                     {
                       FromCommand<InsertTextCommand, string>(() => BlockAt(0).EndCursor().ToHandle(), "Start of text"),
                       FromCommand<BreakTextBlockCommand>(() => BlockAt(0).BeginCursor().ToHandle()),
                     });

      Check.That(Document.Root.ChildCount).IsEqualTo(2);
      Check.That(BlockAt(0)).InheritsFrom<TextBlock>();
      Check.That(BlockAt(1)).InheritsFrom<TextBlock>();

      Check.That(BlockAt(0).As<TextBlock>().AsText()).IsEqualTo("");
      Check.That(BlockAt(1).As<TextBlock>().AsText()).IsEqualTo("Start of text");

      it.VerifyUndo();
    }

    [Fact]
    public void BreakInMiddle_BreaksIntoTwo()
    {
      var it = DoAll(new Func<UndoableAction>[]
                     {
                       FromCommand<InsertTextCommand, string>(() => BlockAt(0).EndCursor().ToHandle(), "Start of text"),
                       FromCommand<BreakTextBlockCommand>(() => BlockAt(0).BeginCursor(5).ToHandle()),
                     });

      Check.That(Document.Root.ChildCount).IsEqualTo(2);
      Check.That(BlockAt(0)).InheritsFrom<TextBlock>();
      Check.That(BlockAt(1)).InheritsFrom<TextBlock>();

      Check.That(BlockAt(0).As<TextBlock>().AsText()).IsEqualTo("Start");
      Check.That(BlockAt(1).As<TextBlock>().AsText()).IsEqualTo(" of text");

      it.VerifyUndo();
    }

    [Fact]
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