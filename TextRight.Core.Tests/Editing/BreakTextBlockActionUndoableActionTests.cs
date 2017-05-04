using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
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

      DidYouKnow.That(Document.Root.ChildCount).Should().Be(2);
      DidYouKnow.That(BlockAt(0)).Should().BeAssignableTo<TextBlock>();
      DidYouKnow.That(BlockAt(1)).Should().BeAssignableTo<TextBlock>();

      DidYouKnow.That(BlockAt(0).As<TextBlock>().AsText()).Should().Be("Start of text");
      DidYouKnow.That(BlockAt(1).As<TextBlock>().AsText()).Should().Be("");

      it.VerifyUndo();
    }

    [Fact]
    public void BreakAtBeginning_BreaksIntoTwo()
    {
      var it = DoAll(new Func<UndoableAction>[]
                     {
                       FromCommand<InsertTextCommand, string>(() => BlockAt(0).EndCursor().ToHandle(), "Start of text"), FromCommand<BreakTextBlockCommand>(() => BlockAt(0).BeginCursor().ToHandle()),
                     });

      DidYouKnow.That(Document.Root.ChildCount).Should().Be(2);
      DidYouKnow.That(BlockAt(0)).Should().BeAssignableTo<TextBlock>();
      DidYouKnow.That(BlockAt(1)).Should().BeAssignableTo<TextBlock>();

      DidYouKnow.That(BlockAt(0).As<TextBlock>().AsText()).Should().Be("");
      DidYouKnow.That(BlockAt(1).As<TextBlock>().AsText()).Should().Be("Start of text");

      it.VerifyUndo();
    }

    [Fact]
    public void BreakInMiddle_BreaksIntoTwo()
    {
      var it = DoAll(new Func<UndoableAction>[]
                     {
                       FromCommand<InsertTextCommand, string>(() => BlockAt(0).EndCursor().ToHandle(), "Start of text"), FromCommand<BreakTextBlockCommand>(() => BlockAt(0).BeginCursor(5).ToHandle()),
                     });

      DidYouKnow.That(Document.Root.ChildCount).Should().Be(2);
      DidYouKnow.That(BlockAt(0)).Should().BeAssignableTo<TextBlock>();
      DidYouKnow.That(BlockAt(1)).Should().BeAssignableTo<TextBlock>();

      DidYouKnow.That(BlockAt(0).As<TextBlock>().AsText()).Should().Be("Start");
      DidYouKnow.That(BlockAt(1).As<TextBlock>().AsText()).Should().Be(" of text");

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