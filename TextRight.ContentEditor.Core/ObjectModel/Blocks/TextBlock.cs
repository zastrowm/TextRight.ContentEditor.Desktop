﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace TextRight.ContentEditor.Core.ObjectModel.Blocks
{
  /// <summary> Hosts the view for the TextBlock. </summary>
  public interface ITextBlockView
  {
  }

  /// <summary>
  ///  A block that contains a collection of TextSpans making up a single paragraph of text.
  /// </summary>
  public partial class TextBlock : Block,
                                   IViewableObject<ITextBlockView>,
                                   IEnumerable<StyledTextFragment>
  {
    private readonly List<StyledTextFragment> _spans;

    /// <summary> Default constructor. </summary>
    public TextBlock()
    {
      _spans = new List<StyledTextFragment>();
      AppendSpan(new StyledTextFragment(""));
    }

    /// <summary> The number of fragments contained in this block. </summary>
    public int ChildCount
      => _spans.Count;

    /// <summary> Appends the given span to the TextBlock. </summary>
    /// <param name="fragment"> The span to add. </param>
    public void AppendSpan(StyledTextFragment fragment)
    {
      fragment.Index = _spans.Count;
      fragment.Parent = this;
      _spans.Add(fragment);

      UpdateChildrenNumbering(Math.Max(fragment.Index - 1, 0));

      // TODO add child to element tree
    }

    /// <summary> Removes the given span from the text block. </summary>
    /// <param name="fragment"> The span to remove. </param>
    public void RemoveSpan(StyledTextFragment fragment)
    {
      var originalIndex = fragment.Index;

      _spans.RemoveAt(fragment.Index);
      ClearFragment(fragment);

      // renumber all of the subsequent blocks
      var startIterateIndex = originalIndex - 1;
      if (startIterateIndex < 0)
      {
        startIterateIndex = 0;
      }

      UpdateChildrenNumbering(startIterateIndex);

      // TODO remove child from element tree
    }

    private static void ClearFragment(StyledTextFragment fragment)
    {
      fragment.Parent = null;
      fragment.Index = -1;
    }

    /// <summary> Updates the children numbering starting at the given index. </summary>
    /// <param name="startIndex"> The start index. </param>
    private void UpdateChildrenNumbering(int startIndex = 0)
    {
      // OPTIMIZE to get rid of the two ifs inside here
      for (var i = startIndex; i < _spans.Count; i++)
      {
        var currentSpan = _spans[i];

        currentSpan.Previous = i > 0
          ? _spans[i - 1]
          : null;

        currentSpan.Index = i;

        currentSpan.Next = i < _spans.Count - 1
          ? _spans[i + 1]
          : null;
      }
    }

    /// <inheritdoc/>
    public override IBlockContentCursor GetCursor()
      => new TextBlockCursor(this);

    /// <inheritdoc/>
    public override BlockType BlockType
      => BlockType.TextBlock;

    /// <inheritdoc/>
    public IEnumerator<StyledTextFragment> GetEnumerator()
      => _spans.GetEnumerator();

    /// <inheritdoc/>
    IEnumerator IEnumerable.GetEnumerator()
      => GetEnumerator();

    /// <summary> Retrieves the span at the given index. </summary>
    /// <param name="spanIndex"> The zero-based index of the span to retrieve. </param>
    /// <returns> The span at the given index. </returns>
    private StyledTextFragment GetSpanAtIndex(int spanIndex)
    {
      if (spanIndex < 0 || spanIndex >= _spans.Count)
        throw new ArgumentOutOfRangeException(nameof(spanIndex), spanIndex, $"Number of spans: {_spans.Count}");

      return _spans[spanIndex];
    }

    /// <summary> Extracts the content starting at the cursor and continuing to the end of the block. </summary>
    /// <param name="cursor"> The position at which extraction should start. </param>
    /// <returns> The fragments that have been extracted. </returns>
    private StyledTextFragment[] ExtractContentToEnd(TextBlockCursor cursor)
    {
      if (cursor.IsAtEnd)
        return Array.Empty<StyledTextFragment>();

      StyledTextFragment startFragment = cursor.Fragment;
      int offsetIntoFragment = cursor.OffsetIntoSpan;

      StyledTextFragment[] elements;
      int index;

      bool isAtEndOfFragment = offsetIntoFragment == startFragment.Length;

      if (isAtEndOfFragment)
      {
        // we only need everything passed the fragment
        int expectedCount = ChildCount - (startFragment.Index + 1);

        elements = new StyledTextFragment[expectedCount];
        startFragment = startFragment.Next;
        index = 0;
      }
      else
      {
        // we only need everything passed the fragment plus part of the current fragment
        int expectedCount = ChildCount - startFragment.Index;

        elements = new StyledTextFragment[expectedCount];

        string leftHalf = startFragment.Text.Substring(0, offsetIntoFragment);
        string rightHalf = startFragment.Text.Substring(offsetIntoFragment);

        startFragment.Text = leftHalf;

        elements[0] = new StyledTextFragment(rightHalf);
        startFragment = startFragment.Next;

        index = 1;
      }

      // needs to be before we clear each fragment, otherwise the index will end up 
      // being -1
      int indexOfFirstRemovedSpan = startFragment?.Index ?? Int32.MaxValue;

      while (startFragment != null)
      {
        ClearFragment(startFragment);

        elements[index] = startFragment;
        startFragment = startFragment.Next;
        index += 1;
      }

      // remove all of the spans from this block now
      for (int i = _spans.Count - 1; i >= indexOfFirstRemovedSpan; i--)
      {
        var fragment = _spans[i];
        fragment.Detach();
        _spans.RemoveAt(i);
      }

      UpdateChildrenNumbering(Math.Max(indexOfFirstRemovedSpan - 1, 0));

      Debug.Assert(index == elements.Length);

      return elements;
    }

    /// <summary> The view associated with the TextBlock. </summary>
    public ITextBlockView Target { get; set; }
  }
}