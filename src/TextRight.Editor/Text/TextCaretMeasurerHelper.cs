using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using TextRight.Core.ObjectModel.Blocks.Text;
using TextRight.Core.Utilities;

namespace TextRight.Editor.Text
{
  public static class TextCaretMeasurerHelper
  {
    public static MeasuredRectangle Measure(TextCaret caret)
      => caret.Block.GetView<ITextBlockView>().Measure(caret);

    public static MeasuredRectangle Measure(TextCaret caret, ITextCaretMeasurer measurer)
    {
      bool isAtBlockStart = caret.IsAtBlockStart;
      bool isAtBlockEnd = caret.IsAtBlockEnd;

      if (isAtBlockStart && isAtBlockEnd)
      {
        // if it's empty, there is no character to measure
        return measurer.MeasureSelectionBounds().FlattenLeft();
      }

      // we want to measure the next character unless the previous character was
      // a space (as the text will most likely appear on the next line anyways) 
      // TODO account for more whitespace
      bool shouldMeasureNext = isAtBlockStart
                               || (!isAtBlockEnd && caret.GetPreviousPosition().CharacterAfter.Text == " ");

      return shouldMeasureNext
        ? MeasureForward(caret, measurer).FlattenLeft()
        : MeasureBackward(caret, measurer).FlattenRight();
    }
    
    
    private static MeasuredRectangle MeasureForward(TextCaret caret, ITextCaretMeasurer measurer)
    {
      if (caret.IsAtBlockEnd || caret.Block?.Tag == null)
      {
        Debug.Assert(!caret.IsAtBlockEnd, "This usually indicates an error");
        return MeasuredRectangle.Invalid;
      }

      return measurer.MeasureTextPosition(caret);
    }

    private static MeasuredRectangle MeasureBackward(TextCaret caret, ITextCaretMeasurer measurer)
    {
      if (caret.IsAtBlockStart || caret.Block?.Tag == null)
        return MeasuredRectangle.Invalid;

      return measurer.MeasureTextPosition(caret.GetPreviousPosition());
    }
  }
}