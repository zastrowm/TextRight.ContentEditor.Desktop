using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using JetBrains.Annotations;

namespace TextRight.Core.ObjectModel.Blocks.Text
{
  /// <summary> Handles extracting content from a <see cref="TextBlockContent"/>. </summary>
  internal class TextBlockContentExtractor
  {
    private readonly bool _removeContentOnExtraction;
    private readonly TextBlockContent _originContent;


    public TextBlockContentExtractor(TextBlockContent originContent, bool removeContentOnExtraction)
    {
      _originContent = originContent;
      _removeContentOnExtraction = removeContentOnExtraction;
    }

    /// <see cref="TextBlockContent.ExtractContent"/>
    public TextBlockContent Extract(TextCaret caretStart,
                                    TextCaret caretEnd)
    {
      VerifyExtractParameters(caretStart, caretEnd);

      NormalizePositioning(ref caretStart, ref caretEnd);

      // zero-width; this check is needed, as the normalization process might shift the end to be
      // before the start when both are pointing at the end of a fragment (the start is normalized to
      // point at the beginning of the next fragment instead). 
      if (caretStart == caretEnd)
        return new TextBlockContent();

      var start = GetStart(caretStart);
      var end = new SpanAndOffset(caretEnd.Span, caretEnd.Offset);

      if (caretStart.IsAtBlockStart && caretEnd.IsAtBlockEnd)
      {
        return ExtractAllContent();
      }
      else if (start.Span == end.Span)
      {
        if (start.Offset == end.Offset)
          return new TextBlockContent();

        return ExtractWithinSpan(start, end);
      }
      else
      {
        return ExtractBetweenFragments(start, end);
      }
    }

    private TextBlockContent ExtractAllContent()
    {
      var newContent = new TextBlockContent();
      newContent.AppendAll(_originContent.Spans.Select(s => s.Clone()));

      if (_removeContentOnExtraction)
      {
        _originContent.RemoveAll(_originContent.Spans);
      }

      return newContent;
    }

    [AssertionMethod]
    private void VerifyExtractParameters(TextCaret caretStart,
                                         TextCaret caretEnd)
    {
      if (!caretStart.IsValid || caretStart.Span.Owner != _originContent)
        throw new ArgumentException("Start cursor is not pointing at this content", nameof(caretStart));
      if (!caretEnd.IsValid || caretEnd.Span.Owner != _originContent)
        throw new ArgumentException("End cursor is not pointing at this content", nameof(caretEnd));
    }

    private TextBlockContent ExtractWithinSpan(SpanAndOffset start, SpanAndOffset end)
    {
      TextSpan singleSpan;

      int totalSize = end.Offset - start.Offset;
      if (totalSize == start.Span.NumberOfChars)
      {
        singleSpan = start.Span.Clone();

        if (_removeContentOnExtraction)
        {
          start.Span.Owner.RemoveSpan(start.Span);
        }
      }
      else
      {
        singleSpan = CloneInnerContent(start.Span, start.Offset, end.Offset);
      }

      var clonedContent = new TextBlockContent();
      clonedContent.AppendSpan(singleSpan);
      return clonedContent;
    }

    private TextSpan CloneInnerContent(TextSpan span, int startIndex, int endIndex)
    {
      var newSpan = span.Clone();
      newSpan.RemoveCharacters(endIndex, newSpan.NumberOfChars - endIndex);
      newSpan.RemoveCharacters(0, startIndex);

      if (_removeContentOnExtraction)
      {
        span.RemoveCharacters(startIndex, endIndex - startIndex);
      }

      return newSpan;
    }

    private TextBlockContent ExtractBetweenFragments(SpanAndOffset start, SpanAndOffset end)
    {
      Debug.Assert(start.Span != end.Span);

      var clone = new TextBlockContent();

      // need to save this in case we remove the start from the original
      var current = start.Span.Next;

      // start fragment

      if (start.Offset == 0)
      {
        if (_removeContentOnExtraction)
        {
          _originContent.RemoveSpan(start.Span);
        }
        clone.AppendSpan(start.Span.Clone());
      }
      else
      {
        var startContent = CloneInnerContent(start.Span, start.Offset, start.Span.NumberOfChars);
        clone.AppendSpan(startContent);
      }

      // in-between

      while (current != end.Span)
      {
        var next = current.Next;
        clone.AppendSpan(current.Clone());

        if (_removeContentOnExtraction)
        {
          _originContent.RemoveSpan(current);
        }

        current = next;
      }

      // end fragment

      if (end.Offset == end.Span.NumberOfChars)
      {
        if (_removeContentOnExtraction)
        {
          _originContent.RemoveSpan(end.Span);
        }
        clone.AppendSpan(end.Span.Clone());
      }
      else
      {
        var endContent = CloneInnerContent(end.Span, 0, end.Offset);
        clone.AppendSpan(endContent);
      }

      return clone;
    }

    /// <summary> Makes sure that <paramref name="caretStart"/> comes before <paramref name="caretEnd"/>. </summary>
    private void NormalizePositioning(ref TextCaret caretStart, ref TextCaret caretEnd)
    {
      int comparer = caretStart.Span.Index.CompareTo(caretEnd.Span.Index);

      if (comparer == 0)
      {
        // they're pointing at the same span, so try grapheme
        comparer = caretStart.Offset.GraphemeOffset.CompareTo(caretEnd.Offset.GraphemeOffset);
      }

      // start comes after end, so reverse them
      if (comparer > 0)
      {
        var temp = caretStart;
        caretStart = caretEnd;
        caretEnd = temp;
      }
    }

    private SpanAndOffset GetStart(TextCaret start)
    {
      if (start.IsAtBlockStart)
        return new SpanAndOffset(start.Span);
      else if (start.Offset.GraphemeOffset == start.Span.NumberOfChars && start.Span.Next != null)
        return new SpanAndOffset(start.Span.Next);
      else
        return new SpanAndOffset(start.Span, start.Offset);
    }

    private struct SpanAndOffset
    {
      /// <summary> The span for which this was created </summary>
      public readonly TextSpan Span;

      /// <summary> The offset in characters. </summary>
      public readonly int Offset;

      public SpanAndOffset(TextSpan span)
        : this(span, 0)
      {
      }

      public SpanAndOffset(TextSpan span, TextOffset offset)
        : this(span, offset.CharOffset)
      {
      }

      private SpanAndOffset(TextSpan span, int offset)
      {
        Span = span;
        Offset = offset;
      }
    }
  }
}