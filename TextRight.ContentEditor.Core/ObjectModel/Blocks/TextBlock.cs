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

    /// <summary> Appends the given span to the TextBlock. </summary>
    /// <param name="fragment"> The span to add. </param>
    public void AppendSpan(StyledTextFragment fragment)
    {
      fragment.Index = _spans.Count;
      fragment.Parent = this;
      _spans.Add(fragment);

      // TODO add child to element tree
    }

    /// <summary> Removes the given span from the text block. </summary>
    /// <param name="fragment"> The span to remove. </param>
    public void RemoveSpan(StyledTextFragment fragment)
    {
      var originalIndex = fragment.Index;

      _spans.RemoveAt(fragment.Index);
      fragment.Parent = null;
      fragment.Index = -1;

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

    /// <summary> The view associated with the TextBlock. </summary>
    public ITextBlockView Target { get; set; }
  }
}