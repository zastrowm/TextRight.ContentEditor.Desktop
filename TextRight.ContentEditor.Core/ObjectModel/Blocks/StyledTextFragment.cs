﻿using System;
using System.Collections.Generic;
using System.Linq;
using TextRight.ContentEditor.Core.Utilities;

namespace TextRight.ContentEditor.Core.ObjectModel.Blocks
{
  /// <summary> Hosts the view for the TextSpan. </summary>
  public interface IStyledTextSpanView
  {
    /// <summary> Invoked when the TextSpan's text changes. </summary>
    /// <param name="fragment"> The span whose text has changed. </param>
    void TextUpdated(StyledTextFragment fragment);

    /// <summary> Measures the text at the given location. </summary>
    /// <param name="offset"> The offset at which the text should be measured. </param>
    MeasuredRectangle Measure(int offset);
  }

  /// <summary>
  ///  Contains a span of single run of text that is styled or has some sort of other data
  ///  associated with it.
  /// </summary>
  public class StyledTextFragment : IViewableObject<IStyledTextSpanView>
  {
    /// <summary> Default constructor. </summary>
    public StyledTextFragment(string text)
    {
      Text = text;
      Index = -1;
    }

    /// <summary> The index of the span within a TextBlock. </summary>
    public int Index { get; internal set; }

    /// <summary> The TextBlock that owns the span. </summary>
    public TextBlock Parent { get; internal set; }

    /// <summary> The text in the span. </summary>
    public string Text
    {
      get { return _text; }
      internal set
      {
        _text = value;
        Target?.TextUpdated(this);
      }
    }

    private string _text;

    /// <summary> The number of characters in the TextSpan. </summary>
    public int Length
      => Text.Length;

    /// <summary>
    ///  The object that receives all notifications of changes from this instance.
    /// </summary>
    public IStyledTextSpanView Target { get; set; }
  }
}