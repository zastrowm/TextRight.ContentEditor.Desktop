using System;
using System.Collections.Generic;
using System.Linq;
using TextRight.Core.ObjectModel.Blocks.Text;

namespace TextRight.Editor.Wpf.View.Text
{
  /// <summary> Utility methods to be  </summary>
  public static class TextBlockUtils
  {
    public static (StyledTextFragment fragment, int numCharsBeforeFragment) GetFragmentFromBlockCharacterIndex(int index, TextBlock textBlock)
    {
      var fragment = textBlock.Content.FirstFragment;
      int numberOfCharactersBeforeFragment = 0;

      while (fragment.Next != null && index < numberOfCharactersBeforeFragment + fragment.Length)
      {
        numberOfCharactersBeforeFragment += fragment.Length;
      }

      return (fragment, numberOfCharactersBeforeFragment);
    }

    /// <summary> Gets the index of the character pointed to by the given cursor. </summary>
    /// <param name="cursor"> The cursor whose character index should be retrieved. </param>
    /// <returns>
    ///  The index of the charactered pointed to by the cursor, relative to the entire block.  Thus if
    ///  two fragments have a size of 10 characters each, and the cursor is pointing to the second
    ///  character of the second fragment, this method would return 12.
    /// </returns>
    public static int GetCharacterIndex(TextBlockValueCursor cursor)
    {
      var currentFragment = cursor.Fragment.Parent.Content.FirstFragment;

      int total = 0;

      while (currentFragment != null)
      {
        if (currentFragment == cursor.Fragment)
        {
          return total + cursor.OffsetIntoSpan;
        }

        total += currentFragment.Length;
        currentFragment = currentFragment.Next;
      }

      return -1;
    }
  }
}