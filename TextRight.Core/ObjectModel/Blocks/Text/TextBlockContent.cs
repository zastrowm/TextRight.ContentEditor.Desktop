using System;
using System.Collections.Generic;
using System.Linq;
using TextRight.Core.ObjectModel.Blocks.Text.View;
using TextRight.Core.ObjectModel.Serialization;
using TextRight.Core.Utilities;

namespace TextRight.Core.ObjectModel.Blocks.Text
{
  /// <summary>
  ///  Contains various <see cref="TextSpan"/> parts that is presumed to be part of a
  ///  larger block.
  /// </summary>
  public sealed class TextBlockContent : EventEmitter, IDocumentItem<ITextBlockContentView>
  {
    private readonly List<TextSpan> _spans;

    /// <summary> TextBlockContent constructor. </summary>
    public TextBlockContent()
    {
      _spans = new List<TextSpan>();
      AppendSpan(new TextSpan(""));
    }

    public TextBlock Owner { get; internal set; }

    /// <inheritdoc />
    protected override EventEmitter ParentEmitter
      => Owner;

    /// <summary> The number of fragments contained in this block. </summary>
    public int ChildCount
      => _spans.Count;

    /// <summary> The first fragment in the block. </summary>
    public TextSpan FirstSpan
      => _spans[0];

    /// <summary> The last fragment in the block. </summary>
    public TextSpan LastSpan
      => _spans[_spans.Count - 1];

    /// <summary> All of the fragments contained in this textblock. </summary>
    public IEnumerable<TextSpan> Spans
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
      var current = FirstSpan;

      while (graphemeIndex > numberOfGraphemes + current.GraphemeLength)
      {
        numberOfGraphemes += current.NumberOfChars;
        current = current.Next ?? throw new Exception("Invalid index for cursor");
      }

      return TextCaret.FromOffset(current, graphemeIndex - numberOfGraphemes);
    }

    /// <summary> Appends the given span to the TextBlock. </summary>
    /// <param name="span"> The span to add. </param>
    /// <param name="autoMerge"> True to automatically merge similar fragments together. </param>
    public void AppendSpan(TextSpan span, bool autoMerge = true)
    {
      // FYI early exit

      if (_spans.Count == 1 && FirstSpan.NumberOfChars == 0)
      {
        // if we had a single empty span, we treat that as a placeholder that didn't really mean anything other than
        // "we have no content"
        _spans.Clear();
      }
      else if (autoMerge && _spans.Count > 0)
      {
        var lastSpan = _spans[_spans.Count - 1];
        if (lastSpan.IsSameStyleAs(span))
        {
          lastSpan.InsertText(span.GetText(), lastSpan.NumberOfChars);
          return;
        }
      }

      span.Index = _spans.Count;
      span.Owner = this;
      _spans.Add(span);
      UpdateChildrenNumbering(Math.Max(span.Index - 1, 0));

      FireEvent(new TextSpanInsertedEventArgs(span.Previous, span, span.Next));
    }

    /// <summary> Appends all fragments to the text block.  </summary>
    /// <param name="fragments"> The fragments to add to the text block. </param>
    public void AppendAll(IEnumerable<TextSpan> fragments)
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
    /// <param name="span"> The span to remove. </param>
    public void RemoveSpan(TextSpan span)
    {
      var originalIndex = span.Index;
      var removedArgs = new TextSpanRemovedEventArgs(span.Previous, span, span.Next);

      _spans.RemoveAt(span.Index);
      ClearFragment(span);

      // renumber all of the subsequent blocks
      var startIterateIndex = originalIndex - 1;
      if (startIterateIndex < 0)
      {
        startIterateIndex = 0;
      }

      UpdateChildrenNumbering(startIterateIndex);

      if (_spans.Count == 0)
      {
        AppendSpan(new TextSpan(""));
      }

      // TODO remove child from element tree
      FireEvent(removedArgs);
    }

    /// <summary> Remove all of the given fragments from this text block. </summary>
    /// <param name="fragments"> The fragments to remove from the text block. </param>
    public void RemoveAll(IEnumerable<TextSpan> fragments)
    {
      var frags = fragments.OrderBy(f => f.Index).ToList();

      for (var i = frags.Count - 1; i >= 0; i--)
      {
        RemoveSpan(frags[i]);
      }
    }

    private static void ClearFragment(TextSpan span)
    {
      span.Owner = null;
      span.Index = -1;
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
    public TextSpan GetSpanAtIndex(int spanIndex)
    {
      if (spanIndex < 0 || spanIndex >= _spans.Count)
        throw new ArgumentOutOfRangeException(nameof(spanIndex), spanIndex, $"Number of spans: {_spans.Count}");

      return _spans[spanIndex];
    }

    /// <summary> Extracts the textual content of this block into a separate content object. </summary>
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
                     select new TextSpan(text);

      AppendAll(allSpans);
    }

    /// <inheritdoc />
    IDocumentItemView IDocumentItem.DocumentItemView
      => Target;

    /// <inheritdoc />
    public ITextBlockContentView Target { get; set; }

    /// <summary> Notifies listeners that the given fragment has changed. </summary>
    internal void NotifyChanged(TextSpan span)
    {
      FireEvent(new TextFragmentChangedEventArgs(span));
    }

    /// <summary> EventArgs for when a StyledTextFragment is inserted into a <see cref="TextBlockContent"/> </summary>
    public class TextSpanInsertedEventArgs : EventEmitterArgs<ITextBlockContentEventListener>
    {
      public TextSpanInsertedEventArgs(
        TextSpan previousSpan,
        TextSpan insertedSpan,
        TextSpan nextSpan)
      {
        PreviousSpan = previousSpan;
        InsertedSpan = insertedSpan;
        NextSpan = nextSpan;
      }

      public TextSpan PreviousSpan { get; }
      public TextSpan InsertedSpan { get; }
      public TextSpan NextSpan { get; }

      /// <inheritdoc />
      protected override void Handle(object sender, ITextBlockContentEventListener listener) 
        => listener.NotifyFragmentInserted(PreviousSpan, InsertedSpan, NextSpan);
    }

    /// <summary> EventArgs for when a StyledTextFragment is removed from a <see cref="TextBlockContent"/> </summary>
    public class TextSpanRemovedEventArgs : EventEmitterArgs<ITextBlockContentEventListener>
    {
      public TextSpanRemovedEventArgs(
        TextSpan previousSpan,
        TextSpan removeSpan,
        TextSpan nextSpan)
      {
        PreviousSpan = previousSpan;
        RemoveSpan = removeSpan;
        NextSpan = nextSpan;
      }

      public TextSpan PreviousSpan { get; }
      public TextSpan RemoveSpan { get; }
      public TextSpan NextSpan { get; }

      /// <inheritdoc />
      protected override void Handle(object sender, ITextBlockContentEventListener listener)
        => listener.NotifyFragmentInserted(PreviousSpan, RemoveSpan, NextSpan);
    }

    /// <summary> EventArgs for when the text inside of a StyledTextFragment is changed. </summary>
    public class TextFragmentChangedEventArgs : EventEmitterArgs<ITextBlockContentEventListener>
    {
      public TextFragmentChangedEventArgs(TextSpan changedSpan)
      {
        ChangedSpan = changedSpan;
      }

      public TextSpan ChangedSpan { get; }

      /// <inheritdoc />
      protected override void Handle(object sender, ITextBlockContentEventListener listener)
        => listener.NotifyTextChanged(ChangedSpan);
    }
  }
}