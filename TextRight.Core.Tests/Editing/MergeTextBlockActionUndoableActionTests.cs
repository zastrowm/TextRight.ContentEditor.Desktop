using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using TextRight.Core.Actions;
using TextRight.Core.Commands.Text;
using TextRight.Core.ObjectModel.Blocks.Text;
using Xunit;

namespace TextRight.Core.Tests
{
  public class MergeTextBlockActionUndoableActionTests : UndoBasedTest
  {
    /// <inheritdoc />
    public override IReadOnlyList<Func<UndoableAction>> InitializeDocument()
    {
      return new Func<UndoableAction>[]
             {
               FromCommand<InsertTextCommand, string>(() => BlockAt(0).EndCursor().ToHandle(), "Paragraph 1"),
               FromCommand<BreakTextBlockCommand>(() => BlockAt(0).EndCursor().ToHandle()),
               FromCommand<InsertTextCommand, string>(() => BlockAt(1).EndCursor().ToHandle(), "Paragraph 2"),
             };
    }

    [Fact]
    public void MergeAt_BeginningOfSecondBlock_MergesIntoOne()
    {
      var it = DoAll(new Func<UndoableAction>[]
                     {
                       FromCommand<MergeTextBlocksCommand>(() => BlockAt(1).BeginCursor().ToHandle()),
                     }
      );

      DidYouKnow.That(Document.Root.ChildCount).Should().Be(1);
      DidYouKnow.That(BlockAt(0)).Should().BeAssignableTo<TextBlock>();

      DidYouKnow.That(BlockAt(0).As<TextBlock>().AsText()).Should().Be("Paragraph 1Paragraph 2");

      it.VerifyUndo();
    }

    [Fact]
    public void MergeAt_EndOfFirstBlock_MergesIntoOne()
    {
      var it = DoAll(new Func<UndoableAction>[]
                     {
                       FromCommand<MergeTextBlocksCommand>(() => BlockAt(0).EndCursor().ToHandle()),
                     });

      DidYouKnow.That(Document.Root.ChildCount).Should().Be(1);
      DidYouKnow.That(BlockAt(0)).Should().BeAssignableTo<TextBlock>();

      DidYouKnow.That(BlockAt(0).As<TextBlock>().AsText()).Should().Be("Paragraph 1Paragraph 2");

      it.VerifyUndo();
    }
  }
}