using System;
using System.Collections.Generic;
using System.Linq;
using TextRight.Core.ObjectModel.Blocks.Text;

namespace TextRight.Editor.View.Blocks
{
  public struct FragmentAndCharCount
  {
    public StyledTextFragment Fragment;
    public int NumCharsBeforeFragment;

    public FragmentAndCharCount(StyledTextFragment fragment, int numCharsBeforeFragment)
    {
      this.Fragment = fragment;
      this.NumCharsBeforeFragment = numCharsBeforeFragment;
    }

    public void Deconstruct(out StyledTextFragment fragment, out int numCharsBeforeFragment)
    {
      fragment = Fragment;
      numCharsBeforeFragment = NumCharsBeforeFragment;
    }
  }

  /// <summary> Utility methods to be  </summary>
  public static class TextBlockUtils
  {
    // TODO base off graphemes
    public static FragmentAndCharCount GetFragmentFromBlockCharacterIndex(int index, TextBlock textBlock)
    {
      var fragment = textBlock.Content.FirstFragment;
      int numberOfCharactersBeforeFragment = 0;

      while (fragment.Next != null && index < numberOfCharactersBeforeFragment + fragment.NumberOfChars)
      {
        numberOfCharactersBeforeFragment += fragment.NumberOfChars;
      }

      return new FragmentAndCharCount(fragment, numberOfCharactersBeforeFragment);
    }

    /// <summary> Gets the index of the character pointed to by the given cursor. </summary>
    /// <param name="cursor"> The cursor whose character index should be retrieved. </param>
    /// <returns>
    ///  The index of the charactered pointed to by the cursor, relative to the entire block.  Thus if
    ///  two fragments have a size of 10 characters each, and the cursor is pointing to the second
    ///  character of the second fragment, this method would return 12.
    /// </returns>
    public static int GetCharacterIndex(TextCaret cursor)
    {
      var currentFragment = cursor.Fragment.Owner.FirstFragment;

      int total = 0;

      while (currentFragment != null)
      {
        if (currentFragment == cursor.Fragment)
        {
          return total + cursor.Offset.GraphemeOffset;
        }

        total += currentFragment.NumberOfChars;
        currentFragment = currentFragment.Next;
      }

      return -1;
    }
  }
}