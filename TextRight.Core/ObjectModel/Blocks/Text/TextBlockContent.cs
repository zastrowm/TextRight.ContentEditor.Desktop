using System;
using System.Collections.Generic;
using System.Linq;

using TextRight.Core.ObjectModel.Serialization;

namespace TextRight.Core.ObjectModel.Blocks.Text
{
  /// <summary>
  ///  Contains various <see cref="StyledTextFragment"/> parts that is presumed to be part of a
  ///  larger block.
  /// </summary>
  public sealed class TextBlockContent : IDocumentItem<ITextBlockView>
  {
    private readonly List<StyledTextFragment> _spans;

    /// <summary> TextBlockContent constructor. </summary>
    public TextBlockContent()
    {
      _spans = new List<StyledTextFragment>();
      AppendSpan(new StyledTextFragment(""));
    }

    public TextBlock Owner { get; internal set; }

    /// <summary> The number of fragments contained in this block. </summary>
    public int ChildCount
      => _spans.Count;

    /// <summary> The first fragment in the block. </summary>
    public StyledTextFragment FirstFragment
      => _spans[0];

    /// <summary> The last fragment in the block. </summary>
    public StyledTextFragment LastFragment
      => _spans[_spans.Count - 1];

    /// <summary> All of the fragments contained in this textblock. </summary>
    public IEnumerable<StyledTextFragment> Fragments
      => _spans;

    /// <summary> Gets a cursor that is looking at the beginning of this content. </summary>
    public TextCaret GetCaretAtStart()
      => TextCaret.FromBeginning(this);

    /// <summary> Gets a cursor that is looking at the end of this content. </summary>
    public TextCaret GetCaretAtEnd()
      => TextCaret.FromEnd(this);

    /// <summary> Retrieves a cursor that points at the given character. </summary>
    /// <exception cref="Exception"> Thrown when an exception error condition occurs. </exception>
    /// <param name="graphemeIndex"> The index of the grapheme to point at. </param>
    /// <returns> A TextBlockValueCursor that is pointing at the given grapheme. </returns>
    public TextCaret CursorFromGraphemeIndex(int graphemeIndex)
    {
      int numberOfGraphemes = 0;
      var current = FirstFragment;

      while (graphemeIndex > numberOfGraphemes + current.GraphemeLength)
      {
        numberOfGraphemes += current.NumberOfChars;
        current = current.Next ?? throw new Exception("Invalid index for cursor");
      }

      return TextCaret.FromOffset(current, graphemeIndex - numberOfGraphemes);
    }

    /// <summary> Appends the given span to the TextBlock. </summary>
    /// <param name="fragment"> The span to add. </param>
    /// <param name="autoMerge"> True to automatically merge similar fragments together. </param>
    public void AppendSpan(StyledTextFragment fragment, bool autoMerge = true)
    {
      // FYI early exit

      if (_spans.Count == 1 && FirstFragment.NumberOfChars == 0)
      {
        // if we had a single empty span, we treat that as a placeholder that didn't really mean anything other than
        // "we have no content"
        _spans.Clear();
      }
      else if (autoMerge && _spans.Count > 0)
      {
        var lastSpan = _spans[_spans.Count - 1];
        if (lastSpan.IsSameStyleAs(fragment))
        {
          lastSpan.InsertText(fragment.GetText(), lastSpan.NumberOfChars);
          return;
        }
      }

      fragment.Index = _spans.Count;
      fragment.Owner = this;
      _spans.Add(fragment);
      UpdateChildrenNumbering(Math.Max(fragment.Index - 1, 0));

      Owner?.OnFragmentInserted(fragment.Previous, fragment, fragment.Next);
    }

    /// <summary> Appends all fragments to the text block.  </summary>
    /// <param name="fragments"> The fragments to add to the text block. </param>
    public void AppendAll(IEnumerable<StyledTextFragment> fragments)
    {
      bool autoMerge = true;
      // TODO optimize
      foreach (var fragment in fragments)
      {
        AppendSpan(fragment, autoMerge);
        autoMerge = false;
      }
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

      if (_spans.Count == 0)
      {
        AppendSpan(new StyledTextFragment(""));
      }
      // TODO remove child from element tree
    }

    /// <summary> Remove all of the given fragments from this text block. </summary>
    /// <param name="fragments"> The fragments to remove from the text block. </param>
    public void RemoveAll(IEnumerable<StyledTextFragment> fragments)
    {
      var frags = fragments.OrderBy(f => f.Index).ToList();

      for (var i = frags.Count - 1; i >= 0; i--)
      {
        RemoveSpan(frags[i]);
      }
    }

    private static void ClearFragment(StyledTextFragment fragment)
    {
      fragment.Owner = null;
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

    /// <summary> Retrieves the span at the given index. </summary>
    /// <param name="spanIndex"> The zero-based index of the span to retrieve. </param>
    /// <returns> The span at the given index. </returns>
    public StyledTextFragment GetSpanAtIndex(int spanIndex)
    {
      if (spanIndex < 0 || spanIndex >= _spans.Count)
        throw new ArgumentOutOfRangeException(nameof(spanIndex), spanIndex, $"Number of spans: {_spans.Count}");

      return _spans[spanIndex];
    }

    /// <summary> Extracts the textual content of this block into a seperate content object. </summary>
    /// <param name="start"> The position at which extraction should start. </param>
    /// <param name="end"> The position at which the content extraction should end. </param>
    /// <returns> The extracted content. </returns>
    public TextBlockContent ExtractContent(TextCaret start, TextCaret end)
      => TextBlockContentExtractor.Extract(this, start, end);

    public TextBlockContent Clone()
    {
      var clone = new TextBlockContent();
      clone._spans.Clear();
      clone.AppendAll(_spans.Select(s => s.Clone()));
      return clone;
    }

    /// <inheritdoc />
    public void SerializeInto(SerializeNode node)
    {
      foreach (var span in _spans)
      {
        var subSpanNode = new SerializeNode("temp/fragment");
        subSpanNode.AddData<string>("Body", span.GetText());
        node.Children.Add(subSpanNode);
      }
    }

    /// <inheritdoc />
    public void Deserialize(SerializationContext context, SerializeNode node)
    {
      // TODO should we remove the original
      var allSpans = from childNode in node.Children
                     select childNode.GetDataOrDefault<string>("Body")
                     into text
                     select new StyledTextFragment(text);

      AppendAll(allSpans);
    }

    /// <inheritdoc />
    IDocumentItemView IDocumentItem.DocumentItemView
      => Target;

    /// <inheritdoc />
    public ITextBlockView Target
    {
      get => ((IDocumentItem<ITextBlockView>)Owner).Target;
      // TODO
      set => throw new NotImplementedException();
    }
  }
}