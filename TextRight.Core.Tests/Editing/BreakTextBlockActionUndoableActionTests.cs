using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using FluentAssertions;
using TextRight.Core.Actions;
using TextRight.Core.Commands.Text;
using TextRight.Core.ObjectModel.Blocks.Text;
using Xunit;

namespace TextRight.Core.Tests
{
  public class BreakTextBlockActionUndoableActionTests : UndoBasedTest
  {
    [Theory]
    [InlineData("|Start of text")]
    [InlineData("Start of text|")]
    [InlineData("Start of| text")]
    [InlineData("Start జో s| text")]
    [InlineData("Start |s జో text")]
    [InlineData("Start |జో text")]
    [InlineData("Start జో| text")]
    public void Break_BreaksIntoTwo(string text)
    {
      var parts = text.Split("|", 2);
      var whole = string.Join("", parts);


      int splitIndex = GetGraphemes(text).TakeWhile(t => t != "|").Count();

      var it = DoAll(new Func<UndoableAction>[]
                     {
                       FromCommand<InsertTextCommand, string>(() => BlockAt(0).EndCursor().ToHandle(), whole),
                       FromCommand<BreakTextBlockCommand>(() => BlockAt(0).BeginCursor(splitIndex).ToHandle()),
                     });

      DidYouKnow.That(Document.Root.ChildCount).Should().Be(2);
      DidYouKnow.That(BlockAt(0)).Should().BeAssignableTo<TextBlock>();
      DidYouKnow.That(BlockAt(1)).Should().BeAssignableTo<TextBlock>();

      DidYouKnow.That(BlockAt(0).As<TextBlock>().AsText()).Should().Be(parts[0]);
      DidYouKnow.That(BlockAt(1).As<TextBlock>().AsText()).Should().Be(parts[1]);

      it.VerifyUndo();

      IEnumerable<string> GetGraphemes(string str)
      {
        var enumerator = StringInfo.GetTextElementEnumerator(str);
        enumerator.Reset();
        while (enumerator.MoveNext())
        {
          yield return (string) enumerator.Current;
        }
      }
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