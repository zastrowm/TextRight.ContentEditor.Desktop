using System;
using System.Collections.Generic;
using System.Linq;
using TextRight.Core.ObjectModel.Blocks.Text;

namespace TextRight.Editor.Wpf.View.Text
{
  public struct FragmentAndCharCount
  {
    public TextSpan Span;
    public int NumCharsBeforeFragment;

    public FragmentAndCharCount(TextSpan span, int numCharsBeforeFragment)
    {
      this.Span = span;
      this.NumCharsBeforeFragment = numCharsBeforeFragment;
    }

    public void Deconstruct(out TextSpan span, out int numCharsBeforeFragment)
    {
      span = Span;
      numCharsBeforeFragment = NumCharsBeforeFragment;
    }
  }

  /// <summary> Utility methods to be  </summary>
  public static class TextBlockUtils
  {
    // TODO base off graphemes
    public static FragmentAndCharCount GetFragmentFromBlockCharacterIndex(int index, TextBlock textBlock)
    {
      var fragment = textBlock.Content.FirstSpan;
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
      var currentFragment = cursor.Span.Owner.FirstSpan;

      int total = 0;

      while (currentFragment != null)
      {
        if (currentFragment == cursor.Span)
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