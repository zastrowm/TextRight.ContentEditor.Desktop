using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

using JetBrains.Annotations;

namespace TextRight.Core.ObjectModel.Blocks.Text
{
  /// <summary> Handles extracting content from a <see cref="TextBlockContent"/>. </summary>
  internal static class TextBlockContentExtractor
  {
    /// <see cref="TextBlockContent.ExtractContent"/>
    public static TextBlockContent Extract(TextBlockContent content,
                                           TextCaret startCursor,
                                           TextCaret endCursor)
    {
      VerifyExtractParameters(content, startCursor, endCursor);

      // zero-width; this check is needed, as the normalization process might shift the end to be
      // before the start when both are pointing at the end of a fragment (the start is normalized to
      // point at the beginning of the next fragment instead). 
      if (startCursor == endCursor)
        return new TextBlockContent();

      var start = GetStart(startCursor);
      var end = new FragmentAndOffset(endCursor.Span, endCursor.Offset.GraphemeOffset);

      if (startCursor.IsAtBlockStart && endCursor.IsAtBlockEnd)
      {
        return ExtractAllContent(content);
      }
      else if (start.Span == end.Span)
      {
        if (start.Offset == end.Offset)
          return new TextBlockContent();

        return ExtractWithinFragment(end, start);
      }
      else
      {
        return ExtractBetweenFragments(start, end);
      }
    }

    private static TextBlockContent ExtractAllContent(TextBlockContent content)
    {
      var fragments = content.Spans.ToList();
      content.RemoveAll(fragments);

      var other = new TextBlockContent();
      other.AppendAll(fragments);
      return other;
    }

    [AssertionMethod]
    private static void VerifyExtractParameters(TextBlockContent content,
                                                TextCaret startCursor,
                                                TextCaret endCursor)
    {
      if (!startCursor.IsValid || startCursor.Span.Owner != content)
        throw new ArgumentException("Start cursor is not pointing at this content", nameof(startCursor));
      if (!endCursor.IsValid || endCursor.Span.Owner != content)
        throw new ArgumentException("End cursor is not pointing at this content", nameof(endCursor));
      if (startCursor.Span == endCursor.Span && startCursor.Offset.GraphemeOffset > endCursor.Offset.GraphemeOffset)
        throw new ArgumentException("End cursor does not come after the start cursor", nameof(endCursor));

      var current = startCursor.Span;
      while (current != endCursor.Span)
      {
        current = current.Next;
        if (current == null)
          throw new ArgumentException("End cursor does not come after the start cursor", nameof(endCursor));
      }
    }

    private static TextBlockContent ExtractWithinFragment(FragmentAndOffset end, FragmentAndOffset start)
    {
      TextSpan singleSpan;

      int totalSize = end.Offset - start.Offset;
      if (totalSize == start.Span.NumberOfChars)
      {
        start.Span.Owner.RemoveSpan(start.Span);
        singleSpan = start.Span;
      }
      else
      {
        singleSpan = CloneInnerContent(start.Span, start.Offset, end.Offset);
      }

      var clonedContent = new TextBlockContent();
      clonedContent.AppendSpan(singleSpan);
      return clonedContent;
    }

    private static TextSpan CloneInnerContent(TextSpan span, int startIndex, int endIndex)
    {
      var cloned = span.Clone();
      cloned.RemoveCharacters(endIndex, cloned.NumberOfChars - endIndex);
      cloned.RemoveCharacters(0, startIndex);

      span.RemoveCharacters(startIndex, endIndex - startIndex);
      return cloned;
    }

    private static TextBlockContent ExtractBetweenFragments(FragmentAndOffset start, FragmentAndOffset end)
    {
      Debug.Assert(start.Span != end.Span);

      var clone = new TextBlockContent();
      var original = start.Span.Owner;

      // need to save this in case we remove the start from the original
      var current = start.Span.Next;

      // start fragment

      if (start.Offset == 0)
      {
        original.RemoveSpan(start.Span);
        clone.AppendSpan(start.Span);
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
        original.RemoveSpan(current);
        clone.AppendSpan(current);
        current = next;
      }

      // end fragment

      if (end.Offset == end.Span.NumberOfChars)
      {
        original.RemoveSpan(end.Span);
        clone.AppendSpan(end.Span);
      }
      else
      {
        var endContent = CloneInnerContent(end.Span, 0, end.Offset);
        clone.AppendSpan(endContent);
      }

      return clone;
    }

    private static FragmentAndOffset GetStart(TextCaret start)
    {
      if (start.IsAtBlockStart)
        return new FragmentAndOffset(start.Span, 0);
      else if (start.Offset.GraphemeOffset == start.Span.NumberOfChars && start.Span.Next != null)
        return new FragmentAndOffset(start.Span.Next, 0);
      else
        return new FragmentAndOffset(start.Span, start.Offset.GraphemeOffset);
    }

    private struct FragmentAndOffset
    {
      public readonly TextSpan Span;
      public readonly int Offset;

      public FragmentAndOffset(TextSpan span, int offset)
      {
        Span = span;
        Offset = offset;
      }
    }
  }
}