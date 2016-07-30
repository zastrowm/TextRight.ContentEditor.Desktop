using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using TextRight.ContentEditor.Core.Editing;
using TextRight.ContentEditor.Core.ObjectModel.Cursors;
using TextRight.ContentEditor.Core.ObjectModel.Serialization;

namespace TextRight.ContentEditor.Core.ObjectModel.Blocks
{
  /// <summary>
  ///  A block that contains a collection of TextSpans making up a single paragraph of text.
  /// </summary>
  public abstract class TextBlock : ContentBlock,
                                    IEnumerable<StyledTextFragment>,
                                    IEquatable<TextBlock>
  {
    private readonly List<StyledTextFragment> _spans;

    /// <summary> Default constructor. </summary>
    internal TextBlock()
    {
      _spans = new List<StyledTextFragment>();
      AppendSpan(new StyledTextFragment(""));
    }

    /// <summary> The number of fragments contained in this block. </summary>
    public int ChildCount
      => _spans.Count;

    /// <summary> The first fragment in the block. </summary>
    public StyledTextFragment FirstFragment
      => _spans[0];

    /// <summary> The last fragment in the block. </summary>
    public StyledTextFragment LastFragment
      => _spans[_spans.Count - 1];

    /// <inheritdoc />
    public override ICursorPool CursorPool
      => TextBlockCursor.CursorPool;

    /// <inheritdoc />
    protected override IBlockContentCursor CreateCursorOverride()
    {
      return new TextBlockCursor(this);
    }

    /// <summary> Gets the unique values of the given TextBlock class. </summary>
    /// <returns> The attributes unique to the TextBlock type/instance. </returns>
    public abstract TextBlockAttributes GetAttributes();

    /// <summary> Appends the given span to the TextBlock. </summary>
    /// <param name="fragment"> The span to add. </param>
    /// <param name="autoMerge"> True to automatically merge similar fragments together. </param>
    public void AppendSpan(StyledTextFragment fragment, bool autoMerge = true)
    {
      // FYI early exit
      if (autoMerge && _spans.Count > 0)
      {
        var lastSpan = _spans[_spans.Count - 1];
        if (lastSpan.IsSameStyleAs(fragment))
        {
          lastSpan.Text += fragment.Text;
          return;
        }
      }

      fragment.Index = _spans.Count;
      fragment.Parent = this;
      _spans.Add(fragment);
      UpdateChildrenNumbering(Math.Max(fragment.Index - 1, 0));

      OnFragmentInserted(fragment.Previous, fragment, fragment.Next);
    }

    /// <summary> Invoked when a new fragment is inserted into the textblock. </summary>
    /// <param name="previous"> The fragment that precedes the inserted fragment, can be null. </param>
    /// <param name="fragment"> The fragment that was inserted. </param>
    /// <param name="next"> The fragment that follows the inserted fragment, can be null. </param>
    protected abstract void OnFragmentInserted(StyledTextFragment previous,
                                               StyledTextFragment fragment,
                                               StyledTextFragment next);

    /// <summary> Appends all fragments to the text block.  </summary>
    /// <param name="fragments"> The fragments to add to the text block. </param>
    public void AppendAll(IEnumerable<StyledTextFragment> fragments)
    {
      bool autoMerge = true;

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

    public TextBlockCursor GetTextCursor()
      => new TextBlockCursor(this);

    /// <inheritdoc/>
    public override Block Clone()
    {
      var clone = SuperClone();
      clone._spans.Clear();
      clone.AppendAll(_spans.Select(s => s.Clone()));
      return clone;
    }

    /// <summary>
    ///  Clones the block, returning a copy of the block so that <see cref="TextBlock"/> can add the
    ///  text to the block.
    /// </summary>
    /// <returns> A TextBlock. </returns>
    protected abstract TextBlock SuperClone();

    /// <inheritdoc />
    protected override void Serialize(SerializeNode node)
    {
      foreach (var span in _spans)
      {
        var subSpanNode = new SerializeNode("temp/fragment");
        subSpanNode.AddData<string>("Body", span.Text);
        node.Children.Add(subSpanNode);
      }
    }

    /// <inheritdoc />
    protected override void Deserialize(SerializeNode node)
    {
      throw new NotImplementedException();
    }

    /// <inheritdoc/>
    public IEnumerator<StyledTextFragment> GetEnumerator()
      => _spans.GetEnumerator();

    /// <inheritdoc/>
    IEnumerator IEnumerable.GetEnumerator()
      => GetEnumerator();

    /// <summary> Retrieves the span at the given index. </summary>
    /// <param name="spanIndex"> The zero-based index of the span to retrieve. </param>
    /// <returns> The span at the given index. </returns>
    public StyledTextFragment GetSpanAtIndex(int spanIndex)
    {
      if (spanIndex < 0 || spanIndex >= _spans.Count)
        throw new ArgumentOutOfRangeException(nameof(spanIndex), spanIndex, $"Number of spans: {_spans.Count}");

      return _spans[spanIndex];
    }

    /// <summary> Extracts the content starting at the cursor and continuing to the end of the block. </summary>
    /// <param name="cursor"> The position at which extraction should start. </param>
    /// <returns> The fragments that have been extracted. </returns>
    public StyledTextFragment[] ExtractContentToEnd(TextBlockCursor cursor)
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

    /// <summary> Move the fragments and text from this block into the other block. </summary>
    /// <param name="otherBlock"> The block into which the content should be moved. </param>
    public void MoveTextInto(TextBlock otherBlock)
    {
      // TODO we could optimize this
      using (var copy = GetCursorCopy())
      {
        var cursor = (TextBlockCursor)copy.Cursor;
        cursor.MoveToBeginning();
        var extracted = cursor.ExtractToEnd();
        otherBlock.AppendAll(extracted);
      }
    }

    /// <inheritdoc />
    public override IBlockContentCursor GetCaretFromBottom(CaretMovementMode caretMovementMode)
    {
      var cursor = (TextBlockCursor)GetCursor();
      cursor.MoveToEnd();

      switch (caretMovementMode.CurrentMode)
      {
        case CaretMovementMode.Mode.Position:
          MoveCursorToPosition(cursor, caretMovementMode.Position);
          break;
        case CaretMovementMode.Mode.End:
          // already done
          break;
        case CaretMovementMode.Mode.Home:
          BlockCursorMover.BackwardMover.MoveCaretTowardsLineEdge(cursor);
          break;
        default:
          throw new ArgumentOutOfRangeException();
      }

      return cursor;
    }

    /// <inheritdoc />
    public override IBlockContentCursor GetCaretFromTop(CaretMovementMode caretMovementMode)
    {
      var cursor = (TextBlockCursor)GetCursor();

      cursor.MoveToBeginning();

      switch (caretMovementMode.CurrentMode)
      {
        case CaretMovementMode.Mode.Position:
          MoveCursorToPosition(cursor, caretMovementMode.Position);
          break;
        case CaretMovementMode.Mode.Home:
          // already done
          break;
        case CaretMovementMode.Mode.End:
          BlockCursorMover.ForwardMover.MoveCaretTowardsLineEdge(cursor);
          break;
        default:
          throw new ArgumentOutOfRangeException();
      }

      return cursor;
    }

    private void MoveCursorToPosition(TextBlockCursor cursor, double position)
    {
      // TODO find better way of doing this The problem is that we don't know
      // which way to go, so as a hack, we go both ways and rely on the
      // implementation of MoveToPosition to ultimately choose the closest one. 
      BlockCursorMover.ForwardMover.MoveTowardsLineOffset(cursor, position);
      BlockCursorMover.BackwardMover.MoveTowardsLineOffset(cursor, position);
    }

    /// <inheritdoc />
    public bool Equals(TextBlock other)
    {
      if (ReferenceEquals(null, other))
        return false;
      if (ReferenceEquals(this, other))
        return true;

      return _spans.SequenceEqual(other._spans);
    }

    /// <inheritdoc />
    public override bool Equals(object obj)
    {
      if (ReferenceEquals(null, obj))
        return false;
      if (ReferenceEquals(this, obj))
        return true;
      if (obj.GetType() != GetType())
        return false;
      return Equals((TextBlock)obj);
    }

    /// <inheritdoc />
    public override int GetHashCode()
    {
      return _spans.GetHashCode();
    }
  }
}