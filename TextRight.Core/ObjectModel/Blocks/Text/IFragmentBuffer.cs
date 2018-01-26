using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TextRight.Core.ObjectModel.Blocks.Text
{
  /// <summary> Buffer for the text representation of a <see cref="TextSpan"/>. </summary>
  internal interface IFragmentBuffer
  {
    /// <summary> Inserts the given text at the given position. </summary>
    /// <param name="position"> The offset at which the text should be inserted. </param>
    /// <param name="textToInsert"> The text to insert. </param>
    void InsertText(int position, string textToInsert);

    /// <summary> Removes the given number of characters. </summary>
    /// <param name="position"> The offset at which the characters should be removed. </param>
    /// <param name="numberOfCharacters"> The number of characters to remove. </param>
    void DeleteText(int position, int numberOfCharacters);

    /// <summary> Gets the character at the given position. </summary>
    /// <param name="position"> The index within the span for the character to retrieve. </param>
    /// <returns> The character at the given index. </returns>
    TextUnit GetCharacterAt(TextOffset position);

    /// <summary> The number of characters in the TextSpan. </summary>
    int NumberOfChars { get; }

    /// <summary> Appends the text in this span to the given string builder. </summary>
    /// <param name="builder"> The builder to which the text in this fragment should be appended. </param>
    void AppendTo(StringBuilder builder);

    /// <summary> Retrieves the text within the buffer. </summary>
    string GetText();

    /// <summary> Gets the hashcode for the current buffer. </summary>
    int GetHashCodeImplementation();

    /// <summary> Checks if the text within this buffer is equal to the text in the other buffer. </summary>
    bool Equals(IFragmentBuffer buffer);

    /// <summary> Gets the offset that represents the first grapheme in the buffer. </summary>
    TextOffset GetFirstOffset();

    /// <summary> Gets the offset that represents the last grapheme in the buffer. </summary>
    TextOffset GetLastOffset();

    /// <summary> Gets the total number of graphemes in the buffer. </summary>
    int GraphemeLength { get; }

    /// <summary> Moves the offset forward by one grapheme. </summary>
    /// <param name="offset"> The current offset. </param>
    /// <returns>
    ///  The next offset that represents the next grapheme, or null if no such offset exists.
    /// </returns>
    TextOffset? GetNextOffset(TextOffset offset);

    /// <summary> Moves the offset backward by one grapheme. </summary>
    /// <param name="offset"> The current offset. </param>
    /// <returns>
    ///  The previous offset that represents the previous grapheme, or null if no such offset exists.
    /// </returns>
    TextOffset? GetPreviousOffset(TextOffset offset);

    /// <summary> Gets the offset to the given grapheme. </summary>
    /// <param name="graphemeIndex"> The offset to the given grapheme. </param>
    /// <returns>
    ///  The offset that represents the given grapheme, or null if no such grapheme exists at the
    ///  given index.
    /// </returns>
    TextOffset? GetOffsetToGraphemeIndex(int graphemeIndex);
  }
}