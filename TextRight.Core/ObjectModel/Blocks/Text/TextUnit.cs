using System;
using System.Collections.Generic;
using System.Linq;

namespace TextRight.Core.ObjectModel.Blocks.Text
{
  /// <summary> Represents a single grapheme that will be rendered. </summary>
  public readonly struct TextUnit
  {
    public static TextUnit Default { get; }
      = new TextUnit();

    public TextUnit(string text)
    {
      Text = text;
    }

    public string Text { get; }

    // TODO remove
    public char Character
      => Text?[0] ?? '\0';

    public bool IsWhitespace
      => char.IsWhiteSpace(Character);
  }
}