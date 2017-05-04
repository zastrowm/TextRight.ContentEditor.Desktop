using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TextRight.Core.ObjectModel.Blocks.Text;
using TextRight.Core.Utilities;

namespace TextRight.Editor.View.Blocks
{
  public interface ITextBlockRenderer
  {
    MeasuredRectangle MeasureForward(TextBlockValueCursor cursor);
    MeasuredRectangle MeasureBackward(TextBlockValueCursor cursor);
  }

  public static class TextBlockValueCursorExtensions
  {
    public static MeasuredRectangle MeasurePosition(this TextBlockValueCursor cursor, ITextBlockRenderer associatedRenderer)
    {
      if (cursor.IsAtEndOfBlock && cursor.IsAtBeginningOfBlock)
      {
        // if it's empty, there is no character to measure
        return cursor.Block.GetBounds().FlattenLeft();
      }

      // we want to measure the next character unless the previous character was
      // a space (as the text will most likely appear on the next line anyways) 
      bool shouldMeasureNext = cursor.IsAtBeginningOfBlock
                               || (!cursor.IsAtEndOfBlock && cursor.CharacterBefore == ' ');

      return shouldMeasureNext
        ? associatedRenderer.MeasureForward(cursor).FlattenLeft()
        : associatedRenderer.MeasureBackward(cursor).FlattenRight();
    }
  }
}