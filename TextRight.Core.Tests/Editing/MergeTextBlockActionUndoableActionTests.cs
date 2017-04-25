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

      Check.That(Document.Root.ChildCount).IsEqualTo(1);
      Check.That(BlockAt(0)).InheritsFrom<TextBlock>();

      Check.That(BlockAt(0).As<TextBlock>().AsText()).IsEqualTo("Paragraph 1Paragraph 2");

      it.VerifyUndo();
    }

    [Fact]
    public void MergeAt_EndOfFirstBlock_MergesIntoOne()
    {
      var it = DoAll(new Func<UndoableAction>[]
                     {
                       FromCommand<MergeTextBlocksCommand>(() => BlockAt(0).EndCursor().ToHandle()),
                     }
      );

      Check.That(Document.Root.ChildCount).IsEqualTo(1);
      Check.That(BlockAt(0)).InheritsFrom<TextBlock>();

      Check.That(BlockAt(0).As<TextBlock>().AsText()).IsEqualTo("Paragraph 1Paragraph 2");

      it.VerifyUndo();
    }
  }
}