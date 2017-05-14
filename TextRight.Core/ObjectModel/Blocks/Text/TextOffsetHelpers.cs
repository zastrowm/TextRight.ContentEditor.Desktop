using System;
using System.Collections.Generic;
using System.Linq;

namespace TextRight.Core.ObjectModel.Blocks.Text
{
  internal static class TextOffsetHelpers
  {
    /// <summary> Creates a TextOffset that exists after all content in the buffer. </summary>
    /// <param name="buffer"> The buffer for which the text offset should be created. </param>
    /// <returns> The offset representing the content after the buffer. </returns>
    internal static TextOffset CreateAfterTextOffset(IFragmentBuffer buffer)
    {
      return new TextOffset(
        buffer.NumberOfChars,
        buffer.GraphemeLength,
        0);
    }
  }
}