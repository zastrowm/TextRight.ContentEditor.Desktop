using System;
using System.Collections.Generic;
using System.Linq;
using TextRight.Core.ObjectModel.Blocks.Text;

namespace TextRight.Core.Tests
{
  public static class TextCaretTestExtensions
  {
    /// <summary> Get the character before the current cursor position. </summary>
    public static TextUnit GetCharacterBefore(this TextCaret caret)
    {
      if (caret.IsAtBeginningOfBlock)
        return TextUnit.Default;

      caret = caret.GetPreviousPosition();
      return caret.CharacterAfter;
    }

    public static TextCaret MoveCursorForwardBy(this TextCaret cursor, int index)
    {
      int i = index;
      while (i > 0)
      {
        cursor = cursor.GetNextPosition();
        i -= 1;
      }

      return cursor;
    }

    public static TextCaret MoveCursorBackwardBy(this TextCaret cursor, int index)
    {
      int i = index;
      while (i > 0)
      {
        cursor = cursor.GetPreviousPosition();
        i -= 1;
      }

      return cursor;
    }
  }
}