using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using TextRight.Core.ObjectModel.Blocks.Text;
using Xunit;

namespace TextRight.Core.Tests.ObjectModel.Blocks
{
  public class TextBlockContentTests
  {
    public TextBlockContent Content;

    public TextBlockContentTests()
    {
      Content = new TextBlockContent();
      Content.Insert(Content.GetCaretAtStart(), "123456789");
    }

    public static TheoryData<int, int> VerifyExtractionEverywhereData()
    {
      IEnumerable<(int, int)> Generate()
      {
        for (int start = 0; start < 9; start++)
        {
          for (int end = start; end < 9; end++)
          {
            yield return (start, end);
          }
        }
      }

      var data = new TheoryData<int, int>();

      foreach (var item in Generate())
      {
        data.Add(item.Item1, item.Item2);
      }

      return data;
    }

    [Theory]
    [MemberData(nameof(VerifyExtractionEverywhereData))]
    public void VerifyExtractionEverywhere(int start, int end)
    {
      var originalText = Content.AsText();

      var extracted = Content.ExtractContent(Content.CursorFromGraphemeIndex(start),
                                             Content.CursorFromGraphemeIndex(end));

      var removedText = originalText.Substring(start, end - start);
      var modifiedText = originalText.Remove(start, end - start);

      DidYouKnow.That(Content.AsText()).Should().Be(modifiedText);
      DidYouKnow.That(extracted.AsText()).Should().Be(removedText);
    }

    [Theory]
    [MemberData(nameof(VerifyExtractionEverywhereData))]
    public void VerifyCloneContentEverywhere(int start, int end)
    {
      var originalText = Content.AsText();

      var extracted = Content.CloneContent(Content.CursorFromGraphemeIndex(start),
                                           Content.CursorFromGraphemeIndex(end));

      var extractedText = originalText.Substring(start, end - start);

      DidYouKnow.That(Content.AsText()).Should().Be(originalText);
      DidYouKnow.That(extracted.AsText()).Should().Be(extractedText);
    }

    public static TheoryData<int> Generate0to9Indices()
    {
      var data = new TheoryData<int>();

      foreach (var item in Enumerable.Range(0, 10))
      {
        data.Add(item);
      }

      return data;
    }

    [Theory]
    [MemberData(nameof(Generate0to9Indices))]
    public void ExtractParameterOrderDoesNotMatter(int insertionIndex)
    {
      var caret = Content.CursorFromGraphemeIndex(insertionIndex);

      var originalText = Content.AsText();

      var content = new TextBlockContent();
      content.Insert(content.GetCaretAtStart(), "ABCDEFGHIJ");

      var newPosition = Content.Insert(caret, content);

      // make sure the inserted text is correct by comparing the text we get by simply inserted the
      // same text into a string. 
      var expectedFinalText = originalText.Insert(insertionIndex, content.AsText());
      DidYouKnow.That(Content.AsText()).Should().Be(expectedFinalText);

      // make sure the caret returned is correct by comparing it to the text that should come after
      // the caret. 
      var expectedAfterText = originalText.Substring(0, insertionIndex) + "ABCDEFGHIJ";
      DidYouKnow.That(Content.CloneContent(Content.GetCaretAtStart(), newPosition).AsText())
                .Should().Be(expectedAfterText);
    }
  }
}