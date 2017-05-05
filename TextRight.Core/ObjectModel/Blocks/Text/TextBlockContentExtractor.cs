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
      var end = new FragmentAndOffset(endCursor.Fragment, endCursor.OffsetIntoSpan);

      if (startCursor.IsAtBeginningOfBlock && endCursor.IsAtEndOfBlock)
      {
        return ExtractAllContent(content);
      }
      else if (start.Fragment == end.Fragment)
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
      var fragments = content.Fragments.ToList();
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
      if (!startCursor.IsValid || startCursor.Fragment.Owner != content)
        throw new ArgumentException("Start cursor is not pointing at this content", nameof(startCursor));
      if (!endCursor.IsValid || endCursor.Fragment.Owner != content)
        throw new ArgumentException("End cursor is not pointing at this content", nameof(endCursor));
      if (startCursor.Fragment == endCursor.Fragment && startCursor.OffsetIntoSpan > endCursor.OffsetIntoSpan)
        throw new ArgumentException("End cursor does not come after the start cursor", nameof(endCursor));

      var current = startCursor.Fragment;
      while (current != endCursor.Fragment)
      {
        current = current.Next;
        if (current == null)
          throw new ArgumentException("End cursor does not come after the start cursor", nameof(endCursor));
      }
    }

    private static TextBlockContent ExtractWithinFragment(FragmentAndOffset end, FragmentAndOffset start)
    {
      StyledTextFragment singleFragment;

      int totalSize = end.Offset - start.Offset;
      if (totalSize == start.Fragment.Length)
      {
        start.Fragment.Owner.RemoveSpan(start.Fragment);
        singleFragment = start.Fragment;
      }
      else
      {
        singleFragment = CloneInnerContent(start.Fragment, start.Offset, end.Offset);
      }

      var clonedContent = new TextBlockContent();
      clonedContent.AppendSpan(singleFragment);
      return clonedContent;
    }

    private static StyledTextFragment CloneInnerContent(StyledTextFragment fragment, int startIndex, int endIndex)
    {
      var cloned = fragment.Clone();
      cloned.RemoveCharacters(endIndex, cloned.Length - endIndex);
      cloned.RemoveCharacters(0, startIndex);

      fragment.RemoveCharacters(startIndex, endIndex - startIndex);
      return cloned;
    }

    private static TextBlockContent ExtractBetweenFragments(FragmentAndOffset start, FragmentAndOffset end)
    {
      Debug.Assert(start.Fragment != end.Fragment);

      var clone = new TextBlockContent();
      var original = start.Fragment.Owner;

      // need to save this in case we remove the start from the original
      var current = start.Fragment.Next;

      // start fragment

      if (start.Offset == 0)
      {
        original.RemoveSpan(start.Fragment);
        clone.AppendSpan(start.Fragment);
      }
      else
      {
        var startContent = CloneInnerContent(start.Fragment, start.Offset, start.Fragment.Length);
        clone.AppendSpan(startContent);
      }

      // in-between

      while (current != end.Fragment)
      {
        var next = current.Next;
        original.RemoveSpan(current);
        clone.AppendSpan(current);
        current = next;
      }

      // end fragment

      if (end.Offset == end.Fragment.Length)
      {
        original.RemoveSpan(end.Fragment);
        clone.AppendSpan(end.Fragment);
      }
      else
      {
        var endContent = CloneInnerContent(end.Fragment, 0, end.Offset);
        clone.AppendSpan(endContent);
      }

      return clone;
    }

    private static FragmentAndOffset GetStart(TextCaret start)
    {
      if (start.IsAtBeginningOfBlock)
        return new FragmentAndOffset(start.Fragment, 0);
      else if (start.OffsetIntoSpan == start.Fragment.Length && start.Fragment.Next != null)
        return new FragmentAndOffset(start.Fragment.Next, 0);
      else
        return new FragmentAndOffset(start.Fragment, start.OffsetIntoSpan);
    }

    private struct FragmentAndOffset
    {
      public readonly StyledTextFragment Fragment;
      public readonly int Offset;

      public FragmentAndOffset(StyledTextFragment fragment, int offset)
      {
        Fragment = fragment;
        Offset = offset;
      }
    }
  }
}