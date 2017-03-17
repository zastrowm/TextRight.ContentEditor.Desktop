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

    [Test]
    public void MergeAt_BeginningOfSecondBlock_MergesIntoOne()
    {
      var it = DoAll(new Func<UndoableAction>[]
                     {
                       FromCommand<MergeTextBlocksCommand>(() => BlockAt(1).BeginCursor().ToHandle()),
                     }
      );

      Assert.That(Document.Root.ChildCount, Is.EqualTo(1));
      Assert.That(BlockAt(0), Is.InstanceOf<TextBlock>());

      Assert.That(BlockAt(0).As<TextBlock>().AsText(), Is.EqualTo("Paragraph 1Paragraph 2"));

      it.VerifyUndo();
    }

    [Test]
    public void MergeAt_EndOfFirstBlock_MergesIntoOne()
    {
      var it = DoAll(new Func<UndoableAction>[]
                     {
                       FromCommand<MergeTextBlocksCommand>(() => BlockAt(0).EndCursor().ToHandle()),
                     }
      );

      Assert.That(Document.Root.ChildCount, Is.EqualTo(1));
      Assert.That(BlockAt(0), Is.InstanceOf<TextBlock>());

      Assert.That(BlockAt(0).As<TextBlock>().AsText(), Is.EqualTo("Paragraph 1Paragraph 2"));

      it.VerifyUndo();
    }
  }
}