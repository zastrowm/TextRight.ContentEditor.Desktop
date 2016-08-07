using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TextRight.ContentEditor.Core.ObjectModel.Blocks
{
  /// <summary> Buffer for the text representation of a <see cref="StyledTextFragment"/>. </summary>
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
    char GetCharacterAt(int position);
    /// <summary> The number of characters in the TextSpan. </summary>
    int Length { get; }

    /// <summary> Appends the text in this span to the given string builder. </summary>
    /// <param name="builder"> The builder to which the text in this fragment should be appended. </param>
    void AppendTo(StringBuilder builder);

    /// <summary> Retrieves the text within the buffer. </summary>
    string GetText();

    /// <summary> Gets the hashcode for the current buffer. </summary>
    int GetHashCodeImplementation();

    /// <summary> Checks if the text within this buffer is equal to the text in the other buffer. </summary>
    bool Equals(IFragmentBuffer buffer);
  }
}