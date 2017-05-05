using System;
using System.Collections.Generic;
using System.Linq;
using TextRight.Core.ObjectModel.Blocks.Text;
using TextRight.Core.Utilities;

namespace TextRight.Editor.View.Blocks
{
  /// <summary>
  ///  Extension methods for interacting with a <see cref="TextBlockValueCursor"/>.
  /// </summary>
  public static class TextBlockValueCursorExtensions
  {
    /// <summary> Gets the position of a caret. </summary>
    /// <param name="cursor"> The caret to measure. </param>
    /// <param name="associatedRenderer"> The renderer that should be queried for measuring. </param>
    /// <returns>
    ///  A MeasuredRectangle representing the caret position of the <see cref="cursor"/> that should
    ///  be rendered if the text was to be inserted into the document at the given location.
    /// </returns>
    public static MeasuredRectangle MeasureCaret(this TextBlockValueCursor cursor, ITextBlockRenderer associatedRenderer)
    {
      if (cursor.IsAtEndOfBlock && cursor.IsAtBeginningOfBlock)
      {
        // if it's empty, there is no character to measure
        return cursor.Block.GetBounds().FlattenLeft();
      }

      // When we're in the middle of a line, whether we measure the previous or the next character
      // doesn't because we flatten in the opposite direction, and thus the position should end up in
      // the position.
      // 
      // If we are on the line break, it's special:
      //   - If the previous character was a space, we want to measure the next character so that
      //   because that's where a non-whitespace character is going to show up if the user types.
      //   - If the previous character was not a space, then we want to measure the previous
      //   character, as that's where the character will most likely show up if the user types.
      // Of course, if we're at the end of the block, we can't measure the next character, so we have
      // to measure the previous character. 


      bool shouldMeasureNext;

      // if we're at the beginning of the block, we have to measure the next character, as there is
      // no "previous" character
      if (cursor.IsAtBeginningOfBlock)
        shouldMeasureNext = true;
      else if (!cursor.IsAtEndOfBlock && cursor.CharacterBefore.IsWhitespace)
        shouldMeasureNext = true;
      else
        shouldMeasureNext = false;

      if (shouldMeasureNext)
      {
        var rect = associatedRenderer.MeasureGraphemeBehind(cursor);
        return rect.FlattenLeft();
      }
      else
      {
        // we measure the previous character by moving backwards then measuring
        cursor = cursor.MoveBackward();
        var rect = associatedRenderer.MeasureGraphemeBehind(cursor);
        return rect.FlattenRight();
      }
    }
  }
}