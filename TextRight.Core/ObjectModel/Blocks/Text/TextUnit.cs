using System;
using System.Collections.Generic;
using System.Linq;

namespace TextRight.Core.ObjectModel.Blocks.Text
{
  /// <summary> Represents a single grapheme that will be rendered. </summary>
  public struct TextUnit
  {
    // TODO make this actually grapheme based instead of character based

    public static TextUnit Default { get; }
      = new TextUnit();

    public TextUnit(char character)
    {
      Character = character;
    }

    public char Character { get; }

    public bool IsWhitespace
      => char.IsWhiteSpace(Character);

    public string Text
      => Character.ToString();
  }
}