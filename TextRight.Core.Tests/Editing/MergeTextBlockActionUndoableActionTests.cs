using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using TextRight.ContentEditor.Core.Editing;
using TextRight.ContentEditor.Core.Editing.Actions;
using TextRight.ContentEditor.Core.ObjectModel.Blocks;

namespace TextRight.ContentEditor.Core.Tests.Editing
{
  public class MergeTextBlockActionUndoableActionTests : UndoBasedTest
  {
    /// <inheritdoc />
    public override IReadOnlyList<Func<UndoableAction>> InitializeDocument()
    {
      return new Func<UndoableAction>[]
             {
               () => new InsertTextUndoableAction(BlockAt(0).EndCursor().ToHandle(), "Paragraph 1"),
               () => new BreakTextBlockAction(BlockAt(0).EndCursor().ToHandle()),
               () => new InsertTextUndoableAction(BlockAt(1).EndCursor().ToHandle(), "Paragraph 2"),
             };
    }

    [Test]
    public void MergeAt_BeginningOfSecondBlock_MergesIntoOne()
    {
      var it = DoAll(new Func<UndoableAction>[]
                     {
                       () => new MergeTextBlockAction(
                         BlockAt(0).As<TextBlock>(),
                         BlockAt(1).As<TextBlock>(),
                         BlockAt(1).BeginCursor().ToHandle()),
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
                       () => new MergeTextBlockAction(
                         BlockAt(0).As<TextBlock>(),
                         BlockAt(1).As<TextBlock>(),
                         BlockAt(0).EndCursor().ToHandle()),
                     }
        );

      Assert.That(Document.Root.ChildCount, Is.EqualTo(1));
      Assert.That(BlockAt(0), Is.InstanceOf<TextBlock>());

      Assert.That(BlockAt(0).As<TextBlock>().AsText(), Is.EqualTo("Paragraph 1Paragraph 2"));

      it.VerifyUndo();
    }
  }
}