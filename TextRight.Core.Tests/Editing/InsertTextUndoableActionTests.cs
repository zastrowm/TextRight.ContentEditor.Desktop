using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
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
      DidYouKnow.That(textBlock).Should().NotBeNull();

      DidYouKnow.That(textBlock.AsText()).Should().Be("The text");
      DidYouKnow.That(Context.Cursor.Block).Should().Be(textBlock);
      DidYouKnow.That(Context.Cursor.IsAtEnd).Should().BeTrue();

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

      DidYouKnow.That(Document.Root.ChildCount).Should().Be(1);
      DidYouKnow.That(BlockAt(0)).Should().BeAssignableTo<TextBlock>();

      DidYouKnow.That(BlockAt(0).As<TextBlock>().AsText()).Should().Be("PrefixWord");

      it.VerifyUndo();
    }

    [Fact]
    public void InsertAtEnd_InsertsTextToEnd()
    {
      var it = DoAll(new Func<UndoableAction>[]
                     {
                       FromCommand<InsertTextCommand, string>(() => BlockAt(0).BeginCursor().ToHandle(), "Word"), FromCommand<InsertTextCommand, string>(() => BlockAt(0).EndCursor().ToHandle(), "Suffix"),
                     });

      DidYouKnow.That(Document.Root.ChildCount).Should().Be(1);
      DidYouKnow.That(BlockAt(0)).Should().BeAssignableTo<TextBlock>();

      DidYouKnow.That(BlockAt(0).As<TextBlock>().AsText()).Should().Be("WordSuffix");

      it.VerifyUndo();
    }

    [Fact]
    public void InsertAtMiddle_InsertsTextInMiddle()
    {
      var it = DoAll(new Func<UndoableAction>[]
                     {
                       FromCommand<InsertTextCommand, string>(() => BlockAt(0).BeginCursor().ToHandle(), "Word"), FromCommand<InsertTextCommand, string>(() => BlockAt(0).BeginCursor(2).ToHandle(), "Mid"),
                     });

      DidYouKnow.That(Document.Root.ChildCount).Should().Be(1);
      DidYouKnow.That(BlockAt(0)).Should().BeAssignableTo<TextBlock>();

      DidYouKnow.That(BlockAt(0).As<TextBlock>().AsText()).Should().Be("WoMidrd");

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

      DidYouKnow.That(originalAction.TryMerge(Context, mergeWithAction)).Should().BeTrue();
      DidYouKnow.That(originalAction.Text).Should().Be("The textAnd More");
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
      DidYouKnow.That(BlockAt<TextBlock>(0).AsText()).Should().Be("The text");
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

      DidYouKnow.That(first.TryMerge(Context, second)).Should().BeFalse();
    }

    [Fact]
    public void TryMerge_WithSelf_Fails()
    {
      BlockAt<TextBlock>(0).GetTextCursor().ToBeginning().InsertText("This is text");

      var self = new InsertTextCommand.InsertTextUndoableAction(BlockAt<TextBlock>(0).BeginCursor(1).ToHandle(), "One");

      DidYouKnow.That(self.TryMerge(Context, self)).Should().BeFalse();
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

      DidYouKnow.That(first.TryMerge(Context, second)).Should().BeTrue();
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
          BlockAt<TextBlock>(0).BeginCaret(insertion.Text.Length).AsTextCursor());

      DidYouKnow.That(insertion.TryMerge(Context, second)).
        Should().BeTrue();

      DidYouKnow.That(insertion.Text).Should()
        .Be("Inserted Strin");
    }

    [Fact]
    public void TryMerge_WithBackspace_DoesNotWorkWhenInsertionIsEmpty()
    {
      var fakeInsertion =
        new InsertTextCommand.InsertTextUndoableAction(BlockAt<TextBlock>(0).BeginCursor().ToHandle(),
                                                       "1234");
      fakeInsertion.Do(Context);
      Context.UndoStack.Clear();

      var insertion =
        new InsertTextCommand.InsertTextUndoableAction(BlockAt<TextBlock>(0).BeginCursor().ToHandle(), "");
      insertion.Do(Context);
      var second =
        new DeletePreviousCharacterCommand.DeletePreviousCharacterAction(
          BlockAt<TextBlock>(0).BeginCaret(4).AsTextCursor());

      DidYouKnow.That(insertion.TryMerge(Context, second)).Should().BeFalse();
    }
  }
}