using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using NFluent;
using TextRight.Core.Editing.Actions;
using TextRight.Core.Editing.Commands.Text;
using TextRight.Core.ObjectModel.Blocks;
using TextRight.Core.ObjectModel.Blocks.Text;
using Xunit;

namespace TextRight.Core.Tests.Editing
{
  public class InsertTextUndoableActionTests : UndoBasedTest
  {
    [Fact]
    public void VerifyItWorks()
    {
      var it = DoAll(new Func<UndoableAction>[]
                     {
                       FromCommand<InsertTextCommand, string>(() => Context.Caret, "The text"),
                     });

      var textBlock = Document.Root.FirstBlock as TextBlock;
      Check.That(textBlock).IsNotNull();

      Check.That(textBlock.AsText()).IsEqualTo("The text");
      Check.That(Context.Cursor.Block).IsEqualTo(textBlock);
      Check.That(Context.Cursor.IsAtEnd).IsTrue();

      it.VerifyUndo();
    }

    [Fact]
    public void InsertAtBeginning_InsertsTextAtBeginning()
    {
      var it = DoAll(new Func<UndoableAction>[]
                     {
                       FromCommand<InsertTextCommand, string>(() => BlockAt(0).BeginCursor().ToHandle(), "Word"),
                       FromCommand<InsertTextCommand, string>(() => BlockAt(0).BeginCursor().ToHandle(), "Prefix"),
                     });

      Check.That(Document.Root.ChildCount).IsEqualTo(1);
      Check.That(BlockAt(0)).InheritsFrom<TextBlock>();

      Check.That(BlockAt(0).As<TextBlock>().AsText()).IsEqualTo("PrefixWord");

      it.VerifyUndo();
    }

    [Fact]
    public void InsertAtEnd_InsertsTextToEnd()
    {
      var it = DoAll(new Func<UndoableAction>[]
                     {
                       FromCommand<InsertTextCommand, string>(() => BlockAt(0).BeginCursor().ToHandle(), "Word"),
                       FromCommand<InsertTextCommand, string>(() => BlockAt(0).EndCursor().ToHandle(), "Suffix"),
                     });

      Check.That(Document.Root.ChildCount).IsEqualTo(1);
      Check.That(BlockAt(0)).InheritsFrom<TextBlock>();

      Check.That(BlockAt(0).As<TextBlock>().AsText()).IsEqualTo("WordSuffix");

      it.VerifyUndo();
    }

    [Fact]
    public void InsertAtMiddle_InsertsTextInMiddle()
    {
      var it = DoAll(new Func<UndoableAction>[]
                     {
                       FromCommand<InsertTextCommand, string>(() => BlockAt(0).BeginCursor().ToHandle(), "Word"),
                       FromCommand<InsertTextCommand, string>(() => BlockAt(0).BeginCursor(2).ToHandle(), "Mid"),
                     });

      Check.That(Document.Root.ChildCount).IsEqualTo(1);
      Check.That(BlockAt(0)).InheritsFrom<TextBlock>();

      Check.That(BlockAt(0).As<TextBlock>().AsText()).IsEqualTo("WoMidrd");

      it.VerifyUndo();
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void Undo_RestoresInitialState(bool withMerge)
    {
      DoAllAndThenUndo(new Func<UndoableAction>[]
                       {
                         FromCommand<InsertTextCommand, string>(() => BlockAt(0).EndCursor().ToHandle(), "The text"),
                         FromCommand<InsertTextCommand, string>(() => BlockAt(0).EndCursor().ToHandle(), "More text"),
                         FromCommand<InsertTextCommand, string>(() => BlockAt(0).BeginCursor().ToHandle(), "More text"),
                         FromCommand<InsertTextCommand, string>(
                           () => BlockAt(0).BeginCursor(10).ToHandle(),
                           "More text"),
                         FromCommand<InsertTextCommand, string>(() => BlockAt(0).BeginCursor(1).ToHandle(), "The text"),
                         FromCommand<InsertTextCommand, string>(() => BlockAt(0).EndCursor().ToHandle(), "More text"),
                         FromCommand<InsertTextCommand, string>(() => BlockAt(0).EndCursor(-1).ToHandle(), "More text"),
                         FromCommand<InsertTextCommand, string>(
                           () => BlockAt(0).BeginCursor(10).ToHandle(),
                           "More text"),
                       },
                       withMerge: withMerge
      );
    }

    [Fact]
    public void Undo_RestoresInitalState_AnywhereInParagraps()
    {
      for (int offset = 0; offset < 10; offset++)
      {
        var frozenOffset = offset;
        DoAllAndThenUndo(new Func<UndoableAction>[]
                         {
                           // Block 1
                           FromCommand<InsertTextCommand, string>(() => BlockAt(0).EndCursor().ToHandle(), "012345679"),
                           FromCommand<InsertTextCommand, string>(
                             () => BlockAt(0).BeginCursor(frozenOffset).ToHandle(),
                             "012345679"),
                         }
        );
      }
    }

    [Fact]
    public void VerifyMergeWith_ModifiesOriginalAction()
    {
      var originalAction =
        new InsertTextCommand.InsertTextUndoableAction(BlockAt(0).EndCursor().ToHandle(), "The text");
      originalAction.Do(Context);

      var mergeWithAction =
        new InsertTextCommand.InsertTextUndoableAction(BlockAt(0).EndCursor().ToHandle(), "And More");

      Check.That(originalAction.TryMerge(Context, mergeWithAction)).IsTrue();
      Check.That(originalAction.Text).IsEqualTo("The textAnd More");
    }

    [Fact]
    public void VerifyMergeWith_DoesNotActsOnDocument()
    {
      var originalAction =
        new InsertTextCommand.InsertTextUndoableAction(BlockAt(0).EndCursor().ToHandle(), "The text");
      originalAction.Do(Context);

      var mergeWithAction =
        new InsertTextCommand.InsertTextUndoableAction(BlockAt(0).EndCursor().ToHandle(), "And More");
      originalAction.TryMerge(Context, mergeWithAction);

      // the document should not be modified
      Check.That(BlockAt<TextBlock>(0).AsText()).IsEqualTo("The text");
    }

    [Fact]
    public void VerifyMerge_DoesntWork_WhenDifferentFragments()
    {
      // TODO when we have the ability to add text to the NEXT span, test that

      //// setup
      //var firstBlock = BlockAt<TextBlock>(0);

      //// two fragments
      //((TextBlockCursor)BlockAt<TextBlock>(0).GetCursor().ToBeginning()).InsertText("01234");
      //firstBlock.Add(new StyledTextFragment("2nd Fragment"));

      //var first = new InsertTextUndoableAction(firstBlock.GetCursor().ToBeginning().ToHandle(), "");
      //var last = new InsertTextUndoableAction(firstBlock.GetCursor().ToEnd().ToHandle(), "");
    }

    [Fact]
    public void TryMerge_DoesNotMergeWhenNotNextToEachother()
    {
      BlockAt<TextBlock>(0).GetTextCursor().ToBeginning().InsertText("This is text");

      var first =
        new InsertTextCommand.InsertTextUndoableAction(BlockAt<TextBlock>(0).BeginCursor(1).ToHandle(), "One");
      var second =
        new InsertTextCommand.InsertTextUndoableAction(BlockAt<TextBlock>(0).BeginCursor(2).ToHandle(), "Two");

      Check.That(first.TryMerge(Context, second)).IsFalse();
    }

    [Fact]
    public void TryMerge_WithSelf_Fails()
    {
      BlockAt<TextBlock>(0).GetTextCursor().ToBeginning().InsertText("This is text");

      var self = new InsertTextCommand.InsertTextUndoableAction(BlockAt<TextBlock>(0).BeginCursor(1).ToHandle(), "One");

      Check.That(self.TryMerge(Context, self)).IsFalse();
    }

    [Fact]
    public void TryMerge_WorksOnLargerStrings()
    {
      BlockAt<TextBlock>(0).GetTextCursor().ToBeginning().InsertText("This is text");

      var first =
        new InsertTextCommand.InsertTextUndoableAction(BlockAt<TextBlock>(0).BeginCursor().ToHandle(),
                                                       "This is a long string");
      first.Do(Context);
      var second = new InsertTextCommand.InsertTextUndoableAction(
        BlockAt<TextBlock>(0).BeginCursor(first.Text.Length).ToHandle(),
        "This is also long");

      Check.That(first.TryMerge(Context, second)).IsTrue();
    }

    [Fact]
    public void TryMerge_WithBackspaceAction_Works()
    {
      BlockAt<TextBlock>(0).GetTextCursor().ToBeginning().InsertText("This is text");

      var insertion =
        new InsertTextCommand.InsertTextUndoableAction(BlockAt<TextBlock>(0).BeginCursor().ToHandle(),
                                                       "Inserted String");
      insertion.Do(Context);
      var second =
        new DeletePreviousCharacterCommand.DeletePreviousCharacterAction(
          BlockAt<TextBlock>(0).BeginCursor(insertion.Text.Length).AsTextCursor());

      Check.That(insertion.TryMerge(Context, second)).IsTrue();
      Check.That(insertion.Text).IsEqualTo("Inserted Strin");
    }

    [Fact]
    public void TryMerge_WithBackspace_DoesNotWorkWhenInsertionIsEmpty()
    {
      var insertion =
        new InsertTextCommand.InsertTextUndoableAction(BlockAt<TextBlock>(0).BeginCursor().ToHandle(), "");
      insertion.Do(Context);
      var second =
        new DeletePreviousCharacterCommand.DeletePreviousCharacterAction(
          BlockAt<TextBlock>(0).BeginCursor(insertion.Text.Length).AsTextCursor());

      Check.That(insertion.TryMerge(Context, second)).IsFalse();
    }
  }
}