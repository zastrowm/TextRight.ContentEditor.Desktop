using System;
using System.Collections.Generic;
using System.Linq;
using TextRight.Core.ObjectModel;
using TextRight.Core.ObjectModel.Blocks.Text;
using TextRight.Editor.Text;

namespace TextRight.Editor
{
  /// <summary>
  ///   The editor view for a <see cref="TextBlock"/>
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