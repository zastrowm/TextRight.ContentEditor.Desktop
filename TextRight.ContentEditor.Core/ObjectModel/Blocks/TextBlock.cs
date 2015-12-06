using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace TextRight.ContentEditor.Desktop.ObjectModel.Blocks
{
  /// <summary> Hosts the view for the TextBlock. </summary>
  public interface ITextBlockView
  {
  }

  /// <summary>
  ///  A block that contains a collection of TextSpans making up a single paragraph of text.
  /// </summary>
  public partial class TextBlock : Block, IEnumerable<StyledTextSpan>
  {
    private readonly List<StyledTextSpan> _spans;

    /// <summary> Default constructor. </summary>
    public TextBlock()
    {
      _spans = new List<StyledTextSpan>();
      AppendSpan(new StyledTextSpan(""));
    }

    /// <summary> Appends the given span to the TextBlock. </summary>
    /// <param name="span"> The span to add. </param>
    public void AppendSpan(StyledTextSpan span)
    {
      span.Index = _spans.Count;
      span.Parent = this;
      _spans.Add(span);

      // TODO add child to element tree
    }

    /// <summary> Removes the given span from the text block. </summary>
    /// <param name="span"> The span to remove. </param>
    public void RemoveSpan(StyledTextSpan span)
    {
      var originalIndex = span.Index;

      _spans.RemoveAt(span.Index);
      span.Parent = null;
      span.Index = -1;

      // renumber all of the subsequent blocks
      var startIterateIndex = originalIndex - 1;
      if (startIterateIndex < 0)
      {
        startIterateIndex = 0;
      }

      UpdateChildrenNumbering(startIterateIndex);

      // TODO remove child from element tree
    }

    /// <summary> Updates the children numbering starting at the given index. </summary>
    /// <param name="startIndex"> The start index. </param>
    private void UpdateChildrenNumbering(int startIndex = 0)
    {
      for (var i = startIndex; i < _spans.Count; i++)
      {
        _spans[i].Index = i;
      }
    }

    /// <inheritdoc/>
    public override IBlockContentCursor GetCursor()
      => new TextBlockCursor(this);

    /// <inheritdoc/>
    public override BlockType BlockType
      => BlockType.TextBlock;

    /// <inheritdoc/>
    public IEnumerator<StyledTextSpan> GetEnumerator()
      => _spans.GetEnumerator();

    /// <inheritdoc/>
    IEnumerator IEnumerable.GetEnumerator()
      => GetEnumerator();

    /// <summary> Retrieves the span at the given index. </summary>
    /// <param name="spanIndex"> The zero-based index of the span to retrieve. </param>
    /// <returns> The span at the given index. </returns>
    private StyledTextSpan GetSpanAtIndex(int spanIndex)
    {
      if (spanIndex < 0 || spanIndex >= _spans.Count)
        throw new ArgumentOutOfRangeException(nameof(spanIndex), spanIndex, $"Number of spans: {_spans.Count}");

      return _spans[spanIndex];
    }
  }
}