using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using TextRight.Core.Actions;
using TextRight.Core.Commands.Text;
using TextRight.Core.ObjectModel.Blocks.Text;
using Xunit;

namespace TextRight.Core.Tests
{
  public class DeleteNextCharacterCommandUndoableActionTests : UndoBasedTest
  {
    [Fact]
    public void VerifyItWorks()
    {
      var it = DoAll(
        new Func<UndoableAction>[]
        {
          FromCommand<InsertTextCommand, string>(() => BlockAt(0).BeginCursor().ToHandle(), "TheWord"),
          () => new DeleteNextCharacterCommand.DeleteNextCharacterAction(BlockAt(0).BeginCaret(3).AsTextCursor()),
        });

      DidYouKnow.That(BlockAt(0).AsText()).Should().Be("Theord");
      it.VerifyUndo();
    }

    public static IEnumerable<object[]> DeleteCharacterTestCases()
    {
      string text = "TheజోWord";
      var graphemes = GraphemeHelper.GetGraphemes(text).ToArray();

      // we start at 0 and go up to length (but not including) because we're deleting the next
      // character. 
      for (int i = 0; i < graphemes.Length; i++)
      {
        var deleteIndex = i;
        var expected = string.Join("", graphemes.Where((_,  index) => index != deleteIndex));
        yield return new object[]
                     {
                       text,
                       deleteIndex,
                       expected
                     };
      }
    }

    [Theory]
    [MemberData(nameof(DeleteCharacterTestCases))]
    public void DeleteNextCharacter_WorksAtAllLocationsInTheParagraph(string text, int deleteIndex, string expected)
    {
      var it = DoAll(
        new Func<UndoableAction>[]
        {
          FromCommand<InsertTextCommand, string>(() => BlockAt(0).BeginCursor().ToHandle(), text),
          () => new DeleteNextCharacterCommand.DeleteNextCharacterAction(BlockAt(0).BeginCaret(deleteIndex).AsTextCursor()),
        });

      DidYouKnow.That(BlockAt(0).AsText()).Should().Be(expected);
      it.VerifyUndo();
    }
  }
}