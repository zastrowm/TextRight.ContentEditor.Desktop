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
    public TextSpan a,
                    b,
                    c,
                    d,
                    e;

    public TextBlockContent Content;

    public TextBlockContentTests()
    {
      Content = new TextBlockContent();
      Content.AppendSpan((a = new TextSpan("123", "a")));
      Content.AppendSpan((b = new TextSpan("456", "b")));
      Content.AppendSpan((c = new TextSpan("789", "c")));
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
      var originalSpans = Content.Spans.ToList();

      var extracted = Content.CloneContent(Content.CursorFromGraphemeIndex(start),
                                           Content.CursorFromGraphemeIndex(end));

      var extractedText = originalText.Substring(start, end - start);

      DidYouKnow.That(Content.AsText()).Should().Be(originalText);
      DidYouKnow.That(extracted.AsText()).Should().Be(extractedText);

      DidYouKnow.That(Content.Spans).Should().ContainInOrder(originalSpans);
      DidYouKnow.That(extracted.Spans).Should().NotContain(it => originalSpans.Any(s => ReferenceEquals(s, it)));
    }

    [Theory]
    [MemberData(nameof(VerifyExtractionEverywhereData))]
    public void ExtractParameterOrderDoesNotMatter(int start, int end)
    {
      var extractedInOrder = Content.CloneContent(Content.CursorFromGraphemeIndex(start),
                                                  Content.CursorFromGraphemeIndex(end));
      var extractedInReverse = Content.CloneContent(Content.CursorFromGraphemeIndex(end),
                                                    Content.CursorFromGraphemeIndex(start));

      DidYouKnow.That(extractedInOrder.AsText()).Should().Be(extractedInReverse.AsText());
    }
  }
}