using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using TextRight.Core.Utilities;

namespace TextRight.Core.ObjectModel.Blocks.Text
{
  /// <summary> Hosts the view for the TextSpan. </summary>
  public interface IStyledTextSpanView : IDocumentItemView
  {
    /// <summary> Invoked when the TextSpan's text changes. </summary>
    /// <param name="fragment"> The span whose text has changed. </param>
    void TextUpdated(StyledTextFragment fragment);

    /// <summary> Measures the text at the given location. </summary>
    /// <param name="offset"> The offset at which the text should be measured. </param>
    MeasuredRectangle Measure(int offset);

    /// <summary> Notifies the view that the representation is detached from the original TextBlock. </summary>
    void Detach();
  }

  /// <summary>
  ///  Contains a span of single run of text that is styled or has some sort of other data
  ///  associated with it.
  /// </summary>
  [DebuggerDisplay("StyledTextFragment(text={GetText()}, Index={Index})")]
  public class StyledTextFragment : IViewableObject<IStyledTextSpanView>,
                                    IEquatable<StyledTextFragment>,
                                    IDocumentItem<IStyledTextSpanView>
  {
    private readonly string _styleId;
    internal IFragmentBuffer _buffer;

    /// <summary> Default constructor. </summary>
    public StyledTextFragment(string text, string styleId = null)
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
    public StyledTextFragment Next { get; internal set; }

    /// <summary> The sibling fragment that this fragment follows. </summary>
    public StyledTextFragment Previous { get; internal set; }

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
    /// <param name="fragment"> The fragment to compare against. </param>
    /// <returns>
    ///  True if the styles are the same and the fragment could be merged, false otherwise.
    /// </returns>
    public bool IsSameStyleAs(StyledTextFragment fragment)
    {
      return fragment._styleId == _styleId;
    }

    /// <summary> Makes a deep copy of this instance. </summary>
    /// <returns> A copy of this instance. </returns>
    public StyledTextFragment Clone()
    {
      return new StyledTextFragment(_buffer.GetText());
    }

    /// <summary>
    ///  The object that receives all notifications of changes from this instance.
    /// </summary>
    public IStyledTextSpanView Target { get; set; }

    /// <inheritdoc />
    IDocumentItemView IDocumentItem.DocumentItemView
      => Target;

    /// <summary> Appends the text in this span to the given string builder. </summary>
    /// <param name="builder"> The builder to which the text in this fragment should be appended. </param>
    public void AppendTo(StringBuilder builder)
    {
      _buffer.AppendTo(builder);
    }

    /// <summary> Detach the fragment from any views. </summary>
    public void Detach()
    {
      Target?.Detach();
    }

    /// <inheritdoc />
    public bool Equals(StyledTextFragment other)
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
      return Equals((StyledTextFragment)obj);
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
      Target?.TextUpdated(this);
    }

    /// <summary> Removes the given number of characters. </summary>
    /// <param name="offsetIntoSpan"> The offset at which the characters should be removed. </param>
    /// <param name="numberOfCharactersToRemove"> The number of characters to remove. </param>
    public void RemoveCharacters(int offsetIntoSpan, int numberOfCharactersToRemove)
    {
      _buffer.DeleteText(offsetIntoSpan, numberOfCharactersToRemove);
      Target?.TextUpdated(this);
    }

    /// <summary> Retrieves the text within the fragment. </summary>
    public string GetText()
    {
      return _buffer.GetText();
    }
  }
}