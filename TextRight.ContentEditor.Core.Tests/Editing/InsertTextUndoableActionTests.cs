﻿using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using TextRight.ContentEditor.Core.Editing.Actions;
using TextRight.ContentEditor.Core.ObjectModel.Blocks;

namespace TextRight.ContentEditor.Core.Tests.Editing
{
  public class InsertTextUndoableActionTests : UndoBasedTest
  {
    [Test]
    public void VerifyItWorks()
    {
      var it = DoAll(new Func<UndoableAction>[]
                     {
                       () => new InsertTextUndoableAction(Context.Caret, "The text"),
                     });

      var textBlock = Document.Root.FirstBlock as TextBlock;
      Assert.That(textBlock, Is.Not.Null);

      Assert.That(textBlock.AsText(), Is.EqualTo("The text"));
      Assert.That(Context.Cursor.Block, Is.EqualTo(textBlock));
      Assert.That(Context.Cursor.IsAtEnd, Is.True);

      it.VerifyUndo();
    }

    [Test]
    public void InsertAtBeginning_InsertsTextAtBeginning()
    {
      var it = DoAll(new Func<UndoableAction>[]
                     {
                       () => new InsertTextUndoableAction(BlockAt(0).BeginCursor().ToHandle(), "Word"),
                       () => new InsertTextUndoableAction(BlockAt(0).BeginCursor().ToHandle(), "Prefix"),
                     });

      Assert.That(Document.Root.ChildCount, Is.EqualTo(1));
      Assert.That(BlockAt(0), Is.InstanceOf<TextBlock>());

      Assert.That(BlockAt(0).As<TextBlock>().AsText(), Is.EqualTo("PrefixWord"));

      it.VerifyUndo();
    }

    [Test]
    public void InsertAtEnd_InsertsTextToEnd()
    {
      var it = DoAll(new Func<UndoableAction>[]
                     {
                       () => new InsertTextUndoableAction(BlockAt(0).BeginCursor().ToHandle(), "Word"),
                       () => new InsertTextUndoableAction(BlockAt(0).EndCursor().ToHandle(), "Suffix"),
                     });

      Assert.That(Document.Root.ChildCount, Is.EqualTo(1));
      Assert.That(BlockAt(0), Is.InstanceOf<TextBlock>());

      Assert.That(BlockAt(0).As<TextBlock>().AsText(), Is.EqualTo("WordSuffix"));

      it.VerifyUndo();
    }

    [Test]
    public void InsertAtMiddle_InsertsTextInMiddle()
    {
      var it = DoAll(new Func<UndoableAction>[]
                     {
                       () => new InsertTextUndoableAction(BlockAt(0).BeginCursor().ToHandle(), "Word"),
                       () => new InsertTextUndoableAction(BlockAt(0).BeginCursor(2).ToHandle(), "Mid"),
                     });

      Assert.That(Document.Root.ChildCount, Is.EqualTo(1));
      Assert.That(BlockAt(0), Is.InstanceOf<TextBlock>());

      Assert.That(BlockAt(0).As<TextBlock>().AsText(), Is.EqualTo("WoMidrd"));

      it.VerifyUndo();
    }

    [Test]
    [TestCase(true)]
    [TestCase(false)]
    public void Undo_RestoresInitialState(bool withMerge)
    {
      DoAllAndThenUndo(new Func<UndoableAction>[]
                       {
                         () => new InsertTextUndoableAction(BlockAt(0).EndCursor().ToHandle(), "The text"),
                         () => new InsertTextUndoableAction(BlockAt(0).EndCursor().ToHandle(), "More text"),
                         () => new InsertTextUndoableAction(BlockAt(0).BeginCursor().ToHandle(), "More text"),
                         () => new InsertTextUndoableAction(BlockAt(0).BeginCursor(10).ToHandle(), "More text"),
                         () => new InsertTextUndoableAction(BlockAt(0).BeginCursor(1).ToHandle(), "The text"),
                         () => new InsertTextUndoableAction(BlockAt(0).EndCursor().ToHandle(), "More text"),
                         () => new InsertTextUndoableAction(BlockAt(0).EndCursor(-1).ToHandle(), "More text"),
                         () => new InsertTextUndoableAction(BlockAt(0).BeginCursor(10).ToHandle(), "More text"),
                       },
                       withMerge: withMerge
        );
    }

    [Test]
    public void Undo_RestoresInitalState_AnywhereInParagraps()
    {
      for (int offset = 0; offset < 10; offset++)
      {
        var frozenOffset = offset;
        DoAllAndThenUndo(new Func<UndoableAction>[]
                         {
                           // Block 1
                           () => new InsertTextUndoableAction(BlockAt(0).EndCursor().ToHandle(), "012345679"),
                           () =>
                             new InsertTextUndoableAction(BlockAt(0).BeginCursor(frozenOffset).ToHandle(), "012345679"),
                         }
          );
      }
    }

    [Test]
    public void VerifyMergeWith_ModifiesOriginalAction()
    {
      var originalAction = new InsertTextUndoableAction(BlockAt(0).EndCursor().ToHandle(), "The text");
      originalAction.Do(Context);

      var mergeWithAction = new InsertTextUndoableAction(BlockAt(0).EndCursor().ToHandle(), "And More");

      Assert.That(originalAction.TryMerge(Context, mergeWithAction), Is.True);
      Assert.That(originalAction.Text, Is.EqualTo("The textAnd More"));
    }

    [Test]
    public void VerifyMergeWith_DoesNotActsOnDocument()
    {
      var originalAction = new InsertTextUndoableAction(BlockAt(0).EndCursor().ToHandle(), "The text");
      originalAction.Do(Context);

      var mergeWithAction = new InsertTextUndoableAction(BlockAt(0).EndCursor().ToHandle(), "And More");
      originalAction.TryMerge(Context, mergeWithAction);

      // the document should not be modified
      Assert.That(BlockAt<TextBlock>(0).AsText(), Is.EqualTo("The text"));
    }
  }
}