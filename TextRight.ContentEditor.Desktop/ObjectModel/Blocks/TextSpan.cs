using System;
using System.Collections.Generic;
using System.Linq;

namespace TextRight.ContentEditor.Desktop.ObjectModel.Blocks
{
  /// <summary>
  ///  Contains a span of single run of text that is styled or has some sort of other data
  ///  associated with it.
  /// </summary>
  public class TextSpan
  {
    /// <summary> Default constructor. </summary>
    public TextSpan(string text)
    {
      Text = text;
      Index = -1;
    }

    /// <summary> The index of the span within a TextBlock. </summary>
    public int Index { get; internal set; }

    /// <summary> The TextBlock that owns the span. </summary>
    public TextBlock Parent { get; internal set; }

    /// <summary> The text in the span. </summary>
    public string Text { get; internal set; }

    /// <summary> The number of characters in the TextSpan. </summary>
    public int Length
      => Text.Length;
  }
}