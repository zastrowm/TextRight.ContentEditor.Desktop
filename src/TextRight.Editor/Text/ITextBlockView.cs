using System;
using System.Linq;
using System.Collections.Generic;
using TextRight.Core.ObjectModel;
using TextRight.Core.ObjectModel.Blocks;
using TextRight.Core.ObjectModel.Blocks.Text;
using TextRight.Core.ObjectModel.Blocks.Text.View;
using TextRight.Core.Utilities;

namespace TextRight.Editor.Text
{
  /// <summary>
  ///   The view for a <see cref="TextBlock"/>
  /// </summary>
  public interface ITextBlockView : IContentBlockView, IDocumentItemView
  {
    /// <summary> The first line in the renderer. </summary>
    IVisualLine<TextCaret> FirstTextLine { get; }

    /// <summary> The second line in the renderer. </summary>
    IVisualLine<TextCaret> LastTextLine { get; }

    /// <summary> Gets the line on which the caret appears. </summary>
    /// <param name="caret"> The caret for which the associated line should be retrieved. </param>
    /// <returns> The line for. </returns>
    IVisualLine<TextCaret> GetLineFor(TextCaret caret);
  }
}