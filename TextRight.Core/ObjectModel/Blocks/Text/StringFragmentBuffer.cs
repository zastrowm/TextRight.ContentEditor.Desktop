using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TextRight.Core.ObjectModel.Blocks.Text
{
  /// <summary> A FragmentBuffer which is backed by a simple string. </summary>
  internal class StringFragmentBuffer : IFragmentBuffer
  {
    private string _text;

    public StringFragmentBuffer(string text)
    {
      _text = text;
    }

    /// <inheritdoc />
    public void InsertText(int position, string textToInsert)
    {
      _text = _text.Insert(position, textToInsert);
    }

    /// <inheritdoc />
    public void DeleteText(int position, int numberOfCharacters)
    {
      _text = _text.Remove(position, numberOfCharacters);
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

      if (Length != buffer?.Length)
        return false;

      // TODO optimize
      return GetText() == buffer.GetText();
    }

    /// <inheritdoc />
    public TextUnit GetCharacterAt(int position)
      => new TextUnit(_text[position]);

    /// <inheritdoc />
    public int Length
      => _text.Length;
  }
}