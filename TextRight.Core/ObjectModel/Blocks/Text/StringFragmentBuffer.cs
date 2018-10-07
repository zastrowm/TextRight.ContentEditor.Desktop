using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace TextRight.Core.ObjectModel.Blocks.Text
{
  /// <summary> A FragmentBuffer which is backed by a simple string. </summary>
  internal class StringFragmentBuffer : IFragmentBuffer
  {
    private string _text;
    private int[] _graphemeOffsets;

    public StringFragmentBuffer(string text)
    {
      _text = text;

      OnTextChanged();
    }

    /// <inheritdoc />
    public void InsertText(int position, string textToInsert)
    {
      _text = _text.Insert(position, textToInsert);

      OnTextChanged();
    }

    /// <summary> Reparse the string for Graphemes. </summary>
    private void OnTextChanged()
    {
      _graphemeOffsets = StringInfo.ParseCombiningCharacters(_text) ?? Array.Empty<int>();
    }

    /// <inheritdoc />
    public void DeleteText(int position, int numberOfCharacters)
    {
      _text = _text.Remove(position, numberOfCharacters);

      OnTextChanged();
    }

    /// <inheritdoc />
    public void AppendTo(StringBuilder builder)
    {
      builder.Append(_text);
    }

    /// <inheritdoc />
    public string GetText()
    {
      return _text;
    }

    /// <inheritdoc />
    public int GetHashCodeImplementation()
    {
      return _text.GetHashCode();
    }

    /// <inheritdoc />
    public bool Equals(IFragmentBuffer buffer)
    {
      if (this == buffer)
        return true;

      if (NumberOfChars != buffer?.NumberOfChars)
        return false;

      // TODO optimize
      return GetText() == buffer.GetText();
    }

    /// <inheritdoc />
    public TextOffset GetFirstOffset() 
      => GetOffsetToGraphemeIndex(0).GetValueOrDefault();

    /// <inheritdoc />
    public TextOffset GetLastOffset()
    {
      if (_graphemeOffsets.Length > 0)
        return GetOffsetToGraphemeIndex(_graphemeOffsets.Length - 1).GetValueOrDefault();
      else
        return GetFirstOffset();
    }

    /// <inheritdoc />
    public int GraphemeLength
      => _graphemeOffsets.Length;

    /// <inheritdoc />
    public TextOffset? GetNextOffset(TextOffset offset) 
      => GetOffsetToGraphemeIndexNoEmpty(offset.GraphemeOffset + 1);

    /// <inheritdoc />
    public TextOffset? GetPreviousOffset(TextOffset offset) 
      => GetOffsetToGraphemeIndexNoEmpty(offset.GraphemeOffset - 1);

    private TextOffset? GetOffsetToGraphemeIndexNoEmpty(int graphemeIndex)
    {
      var offset = GetOffsetToGraphemeIndex(graphemeIndex);
      if (offset == null || !offset.Value.HasContent)
        return null;

      return offset;
    }

    /// <inheritdoc />
    public TextOffset? GetOffsetToGraphemeIndex(int graphemeIndex)
    {
      if (graphemeIndex < 0 || graphemeIndex > _graphemeOffsets.Length)
        return null;

      if (graphemeIndex == _graphemeOffsets.Length)
        return new TextOffset(_text.Length, graphemeIndex, 0);

      int graphemeOffset = graphemeIndex;
      int charOffset = _graphemeOffsets[graphemeOffset];
      int length = graphemeOffset == _graphemeOffsets.Length - 1
        ? _text.Length - charOffset
        : _graphemeOffsets[graphemeOffset + 1] - charOffset;

      // TODO
      return new TextOffset(charOffset, graphemeOffset, length);
    }

    /// <inheritdoc />
    public TextOffset? GetOffsetToCharacterIndex(int characterIndex)
    {
      int graphemeIndex = Array.BinarySearch(_graphemeOffsets, characterIndex);
      return GetOffsetToGraphemeIndex(graphemeIndex);
    }

    /// <inheritdoc />
    public TextUnit GetCharacterAt(TextOffset offset)
      => new TextUnit(_text[offset.CharOffset]);

    /// <inheritdoc />
    public int NumberOfChars
      => _text.Length;
  }
}