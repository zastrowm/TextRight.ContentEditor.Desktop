using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TextRight.Core.ObjectModel.Blocks.Text;
using TextRight.Core.Utilities;

namespace TextRight.Editor.View.Blocks
{
  /// <summary> Responsible for rendering a <see cref="TextBlockContent"/>. </summary>
  public interface ITextBlockRenderer
  {
    /// <summary> The content that is being rendered. </summary>
    TextBlockContent Content { get; }

    /// <summary> Measure the grapheme that follows the given caret location. </summary>
    /// <remarks>
    ///  Returns <see cref="MeasuredRectangle.Invalid"/> if
    ///  <paramref name="caret"/>.<see cref="TextCaret.IsAtBlockEnd"/> is true.
    /// </remarks>
    /// <param name="caret"> The caret location after which the grapheme should be measured. </param>
    /// <returns> A MeasuredRectangle representing the bounds of the grapheme. </returns>
    MeasuredRectangle MeasureGraphemeFollowing(TextCaret caret);
  }
}