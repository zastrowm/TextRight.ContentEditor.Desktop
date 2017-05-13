using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using JetBrains.Annotations;

namespace TextRight.Core.ObjectModel.Blocks.Text
{
  internal static class GraphemeHelper
  {
    [Pure]
    public static int GetGraphemeLength(string text, int index)
    {
      if (text == null)
        throw new ArgumentNullException(nameof(text));

      int textLength = text.Length;

      if (index == textLength)
        return 0;
      if (index < 0 || index > text.Length)
        throw new ArgumentOutOfRangeException(nameof(index));

      int charLen;
      var uc = InternalStringMethods.GetUnicodeCategory(text, index, out charLen);
      int graphemeLength = InternalStringMethods.GetCurrentTextElementLen(text, index, textLength, ref uc, ref charLen);
      return graphemeLength;
    }
  }
}