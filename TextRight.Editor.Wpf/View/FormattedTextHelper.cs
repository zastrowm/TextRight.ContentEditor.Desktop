using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;

namespace TextRight.Editor.Wpf.View
{
  /// <summary> Wraps a FormattedText object, automatically recreating the text when needed. </summary>
  public class FormattedTextHelper
  {
    private bool _isDirty = true;
    private FormattedText _formattedText;
    private readonly IReadOnlyList<StyledStyledTextSpanView> _spans;
    private double _lastMaxWidth;

    public FormattedTextHelper(IReadOnlyList<StyledStyledTextSpanView> spans)
    {
      _spans = spans;
    }

    /// <summary>
    ///  Mark the FormattedText as invalid and needing to be recreated the next time that
    ///  <see cref="GetFormattedText"/> is invoked.
    /// </summary>
    public void Invalidate()
    {
      _isDirty = true;
    }

    /// <summary>
    ///  Gets the latest, up-to-date FormmatedText object, recreating the FormattedText if it is
    ///  currently Invalid.
    /// </summary>
    /// <returns> The formatted text. </returns>
    internal FormattedText GetFormattedText()
    {
      if (!_isDirty)
        return _formattedText;

      RecreateFormattedText();
      _isDirty = false;
      return _formattedText;
    }

    /// <summary> The font to use by default. </summary>
    public Typeface TextFont
    {
      get { return _textFont; }
      set
      {
        _textFont = value;
        Invalidate();
      }
    }

    private Typeface _textFont;

    /// <summary> The size of the font to use by default. </summary>
    public int TextFontSize
    {
      get { return _textFontSize; }
      set
      {
        _textFontSize = value;
        Invalidate();
      }
    }

    private int _textFontSize;

    private void RecreateFormattedText()
    {
      int startIndex = 0;
      var builder = new StringBuilder();

      foreach (var span in _spans)
      {
        span.CharacterOffsetIntoTextView = startIndex;
        builder.Append(span.Text);
        startIndex += span.Text.Length;
      }

      if (builder.Length == 0)
      {
        // we use a zero-width space so that the paragraph still has some sort of height
        builder.Append("\u200B");
      }

      _formattedText = new FormattedText(builder.ToString(),
                                         CultureInfo.CurrentCulture,
                                         FlowDirection.LeftToRight,
                                         TextFont,
                                         _textFontSize,
                                         Brushes.Black,
                                         null,
                                         TextFormattingMode.Display)
                       {
                         MaxTextWidth = _lastMaxWidth
                       };
    }

    public bool SetSizeConstraint(double width)
    {
      var formattedText = GetFormattedText();

      // ReSharper disable once CompareOfFloatsByEqualityOperator
      var newMaxWidth = width == double.PositiveInfinity
        ? 0
        : width;

      // ReSharper disable once CompareOfFloatsByEqualityOperator
      // ReSharper disable once RedundantCheckBeforeAssignment
      if (newMaxWidth == formattedText.MaxTextWidth)
        return false;

      formattedText.MaxTextWidth = newMaxWidth;
      _lastMaxWidth = newMaxWidth;
      return true;
    }
  }
}