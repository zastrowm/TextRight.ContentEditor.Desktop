using System;
using System.Collections.Generic;
using System.Linq;

namespace TextRight.Core.ObjectModel.Blocks.Text
{
  /// <summary> Exposes methods for inserting and removing text using a TextCaret. </summary>
  public static class TextCaretTextManipulator
  {
    /// <summary> Deletes text at the given position. </summary>
    public static TextCaret DeleteText(this TextCaret caret, int numberOfGraphemes)
    {
      var endCaret = caret;

      while (numberOfGraphemes > 0)
      {
        var next = endCaret.GetNextPosition();
        if (!next.IsValid)
          break;
        
        endCaret = next;
        numberOfGraphemes--;
      }
      
      // TODO special case when we're deleting the entire fragment
      caret.Content.DeleteText(caret.Offset, endCaret.Offset);

      return TextCaret.FromOffset(caret.Content, caret.Offset.GraphemeOffset);
    }

    /// <summary> Inserts text at the given location. </summary>
    public static TextCaret InsertText(this TextCaret original, string text)
    {
      return original.Content.Insert(original, text);
    }
  }
}