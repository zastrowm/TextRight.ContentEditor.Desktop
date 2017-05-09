using System;
using System.Collections.Generic;
using System.Linq;
using TextRight.Core.ObjectModel.Blocks.Text;
using TextRight.Core.Utilities;
using TextRight.Editor.View.Blocks;

namespace TextRight.Editor.View.Tests
{
  internal class SimpleTextRenderer : ITextBlockRenderer
  {
    private readonly string[] _lines;

    public SimpleTextRenderer(params string[] lines)
    {
      _lines = lines;
      Content = new TextBlockContent();
      Content.AppendAll(lines.Select(l => new StyledTextFragment(l)));
    }

    public TextBlockContent Content { get; }

    public MeasuredRectangle MeasureGraphemeFollowing(TextCaret caret)
    {
      int index = TextBlockUtils.GetCharacterIndex(caret);

      int offsetToLine = 0;
      int lineIndex;
      for (lineIndex = 0; lineIndex < _lines.Length; lineIndex++)
      {
        if (index < _lines[lineIndex].Length + offsetToLine)
          break;

        offsetToLine += _lines[lineIndex].Length;
      }

      return new MeasuredRectangle()
             {
               Height = 10,
               Width = 10,
               X = (index - offsetToLine) * 10,
               Y = lineIndex * 10
             };
    }
  }
}