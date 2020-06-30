using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace TextRight.Core.ObjectModel.Blocks.Text
{
  /// <summary> A FragmentBuffer which is backed by a simple string. </summary>
  internal class StringFragmentBuffer
  {
    private string _text;

    /// <summary>
    ///  The starting index (into <see cref="_text"/>) where each subsequent grapheme begins.
    /// </summary>
    private int[] _graphemeOffsets;

    public StringFragmentBuffer(string text)
    {
      _text = text;

      Recalculate();
    }

    /// <summary> The number of characters in the TextSpan. </summary>
    public int NumberOfChars
      => _text.Length;

    /// <summary> Gets the total number of graphemes in the buffer. </summary>
    public int GraphemeLength
      => _graphemeOffsets.Length;

    /// <summary> Inserts the given text at the given position. </summary>
    /// <param name="position"> The offset at which the text should be inserted. </param>
    /// <param name="textToInsert"> The text to insert. </param>
    public void InsertText(int position, string textToInsert)
    {
      _text = _text.Insert(position, textToInsert);

      Recalculate();
    }

    /// <summary> Reparse the string for Graphemes. </summary>
    private void Recalculate()
    {
      _graphemeOffsets = StringInfo.ParseCombiningCharacters(_text) ?? Array.Empty<int>();
    }

    /// <summary>
    ///   Removes all of the text from the given buffer.
    /// </summary>
    public void DeleteAllText()
    {
      _text = "";
      
      Recalculate();
    }

    /// <summary> Removes the given number of characters. </summary>
    /// <param name="position"> The offset at which the characters should be removed. </param>
    /// <param name="numberOfCharacters"> The number of characters to remove. </param>
    public void DeleteText(TextOffset start, TextOffset end)
    {
      _text = _text.Remove(start.CharOffset, end.CharOffset - start.CharOffset);

      Recalculate();
    }

    /// <summary> Appends the text in this span to the given string builder. </summary>
    /// <param name="builder"> The builder to which the text in this fragment should be appended. </param>
    public void AppendTo(StringBuilder builder)
    {
      builder.Append(_text);
    }

    /// <summary> Retrieves the text within the buffer. </summary>
    public string GetText()
      => _text;
    
    /// <summary> Retrieves the text within the buffer between two offsets. </summary>
    public string GetText(in TextOffset start, in TextOffset end)
      => _text.Substring(start.CharOffset, end.CharOffset - start.CharOffset);

    /// <summary> Checks if the text within this buffer is equal to the text in the other buffer. </summary>
    public bool Equals(StringFragmentBuffer buffer)
    {
      if (this == buffer)
        return true;

      if (NumberOfChars != buffer?.NumberOfChars)
        return false;

      // TODO optimize
      return GetText() == buffer.GetText();
    }

    /// <summary> Gets the offset that represents the first grapheme in the buffer. </summary>
    public TextOffset GetFirstOffset() 
      => GetOffsetToGraphemeIndex(0).GetValueOrDefault();

    /// <summary> Gets the offset that represents the last grapheme in the buffer. </summary>
    public TextOffset GetLastOffset()
    {
      if (_graphemeOffsets.Length > 0)
        return GetOffsetToGraphemeIndex(_graphemeOffsets.Length - 1).GetValueOrDefault();
      else
        return GetFirstOffset();
    }

    /// <summary> Moves the offset forward by one grapheme. </summary>
    /// <param name="offset"> The current offset. </param>
    /// <returns>
    ///  The next offset that represents the next grapheme, or null if no such offset exists.
    /// </returns>
    public TextOffset? GetNextOffset(TextOffset offset) 
      => GetOffsetToGraphemeIndexNoEmpty(offset.GraphemeOffset + 1);

    /// <summary> Moves the offset backward by one grapheme. </summary>
    /// <param name="offset"> The current offset. </param>
    /// <returns>
    ///  The previous offset that represents the previous grapheme, or null if no such offset exists.
    /// </returns>
    public TextOffset? GetPreviousOffset(TextOffset offset) 
      => GetOffsetToGraphemeIndexNoEmpty(offset.GraphemeOffset - 1);

    private TextOffset? GetOffsetToGraphemeIndexNoEmpty(int graphemeIndex)
    {
      var offset = GetOffsetToGraphemeIndex(graphemeIndex);
      if (offset == null || !offset.Value.HasContent)
        return null;

      return offset;
    }

    /// <summary> Gets the offset to the given grapheme. </summary>
    /// <param name="graphemeIndex"> The offset to the given grapheme. </param>
    /// <returns>
    ///  The offset that represents the given grapheme, or null if no such grapheme exists at the
    ///  given index.
    /// </returns>
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

    /// <summary> Gets the offset to the given character index. </summary>
    /// <param name="characterIndex"> The offset to the given character. </param>
    /// <returns>
    ///  The offset that represents the given character index, or null if no such grapheme exists at the
    ///  given index.
    /// </returns>
    public TextOffset? GetOffsetToCharacterIndex(int characterIndex)
    {
      int graphemeIndex = Array.BinarySearch(_graphemeOffsets, characterIndex);
      return GetOffsetToGraphemeIndex(graphemeIndex);
    }

    /// <summary> Gets the character at the given position. </summary>
    /// <param name="offset"> The index within the span for the character to retrieve. </param>
    /// <returns> The character at the given index. </returns>
    public TextUnit GetCharacterAt(TextOffset offset)
      => new TextUnit(_text[offset.CharOffset]);
  }
}