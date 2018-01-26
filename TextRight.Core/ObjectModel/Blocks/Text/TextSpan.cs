using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using TextRight.Core.Utilities;

namespace TextRight.Core.ObjectModel.Blocks.Text
{

  /// <summary> A node within a larger document. </summary>
  public abstract class DocumentNode : IDocumentItem
  {
    /// <summary> The view associated with the item. </summary>
    public IDocumentItemView DocumentItemView { get; set; }
  }

  /// <summary>
  ///  Contains a span of single run of text that is styled or has some sort of other data
  ///  associated with it.
  /// </summary>
  [DebuggerDisplay("StyledTextFragment(text={GetText()}, Index={Index})")]
  public class TextSpan : DocumentNode, IEquatable<TextSpan>
  {
    private readonly string _styleId;
    internal IFragmentBuffer _buffer;

    /// <summary> Default constructor. </summary>
    public TextSpan(string text, string styleId = null)
    {
      _styleId = styleId;
      _buffer = new StringFragmentBuffer(text);
      Index = -1;
    }

    /// <summary> The index of the span within a TextBlock. </summary>
    public int Index { get; internal set; }

    /// <summary> The owner of the fragment. </summary>
    public TextBlockContent Owner { get; internal set; }

    /// <summary> The TextBlock that owns the span. </summary>
    public TextBlock Parent => Owner.Owner;

    /// <summary> The sibling fragment that follows this fragment. </summary>
    public TextSpan Next { get; internal set; }

    /// <summary> The sibling fragment that this fragment follows. </summary>
    public TextSpan Previous { get; internal set; }

    /// <summary> The number of characters in the TextSpan. </summary>
    public int NumberOfChars
      => _buffer.NumberOfChars;

    /// <summary> The total number of graphemes in this fragment. </summary>
    public int GraphemeLength
      => _buffer.GraphemeLength;

    /// <summary> The buffer associated with this fragment.. </summary>
    internal IFragmentBuffer Buffer
      => _buffer;

    /// <summary>
    ///  True if the given fragment has the same style and could be merged with this instance.
    /// </summary>
    /// <param name="span"> The fragment to compare against. </param>
    /// <returns>
    ///  True if the styles are the same and the fragment could be merged, false otherwise.
    /// </returns>
    public bool IsSameStyleAs(TextSpan span)
    {
      return span._styleId == _styleId;
    }

    /// <summary> Makes a deep copy of this instance. </summary>
    /// <returns> A copy of this instance. </returns>
    public TextSpan Clone()
    {
      return new TextSpan(_buffer.GetText());
    }

    /// <summary> Appends the text in this span to the given string builder. </summary>
    /// <param name="builder"> The builder to which the text in this fragment should be appended. </param>
    public void AppendTo(StringBuilder builder)
    {
      _buffer.AppendTo(builder);
    }

    /// <inheritdoc />
    public bool Equals(TextSpan other)
    {
      if (ReferenceEquals(null, other))
        return false;
      if (ReferenceEquals(this, other))
        return true;

      return _buffer.Equals(other._buffer) && Index == other.Index;
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
      return Equals((TextSpan)obj);
    }

    /// <inheritdoc />
    public override int GetHashCode()
    {
      unchecked
      {
        return (_buffer.GetHashCodeImplementation() * 397) ^ Index;
      }
    }

    /// <summary> Inserts the given text at the given position. </summary>
    /// <param name="text"> The text to insert. </param>
    /// <param name="offsetIntoSpan"> The offset at which the text should be inserted. </param>
    public void InsertText(string text, int offsetIntoSpan)
    {
      _buffer.InsertText(offsetIntoSpan, text);
      Owner?.NotifyChanged(this);
    }

    /// <summary> Removes the given number of characters. </summary>
    /// <param name="offsetIntoSpan"> The offset at which the characters should be removed. </param>
    /// <param name="numberOfCharactersToRemove"> The number of characters to remove. </param>
    public void RemoveCharacters(int offsetIntoSpan, int numberOfCharactersToRemove)
    {
      _buffer.DeleteText(offsetIntoSpan, numberOfCharactersToRemove);
      Owner?.NotifyChanged(this);
    }

    /// <summary> Retrieves the text within the fragment. </summary>
    public string GetText()
    {
      return _buffer.GetText();
    }
  }
}