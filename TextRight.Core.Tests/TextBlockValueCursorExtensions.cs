using System;
using System.Collections.Generic;
using System.Linq;
using TextRight.Core.ObjectModel.Blocks.Text;

namespace TextRight.Core.Tests
{
  public static class TextBlockValueCursorExtensions
  {
    public static TextBlockValueCursor MoveCursorForwardBy(this TextBlockValueCursor cursor, int index)
    {
      int i = index;
      while (i > 0)
      {
        cursor = cursor.MoveForward();
        i -= 1;
      }

      return cursor;
    }

    public static TextBlockValueCursor MoveCursorBackwardBy(this TextBlockValueCursor cursor, int index)
    {
      int i = index;
      while (i > 0)
      {
        cursor = cursor.MoveBackward();
        i -= 1;
      }

      return cursor;
    }
  }
}