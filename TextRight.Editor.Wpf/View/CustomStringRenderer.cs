using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.TextFormatting;
using TextRight.Core.ObjectModel.Blocks.Text;
using TextRight.Core.Utilities;

namespace TextRight.Editor.Wpf.View
{
  /// <summary> Responsible for rendering data within a textblock. </summary>
  public class CustomStringRenderer
  {
    private readonly TextBlock _block;
    private readonly List<StyledStyledTextSpanView> _spans;
    private readonly List<TextLineContainer> _cachedLines;
    private bool _isDirty = true;
    private double _restrictedWidth;
    private double _height;

    public CustomStringRenderer(TextBlock block, List<StyledStyledTextSpanView> spans)
    {
      _block = block;
      _spans = spans;
      _restrictedWidth = 100;
      _cachedLines = new List<TextLineContainer>();
    }

    /// <summary> The maximum width of the lines in this renderer </summary>
    public double MaxWidth { get; private set; }

    public bool SetMaxWidth(double width)
    {
      // ReSharper disable CompareOfFloatsByEqualityOperator
      var restrictedWidth = width == Double.PositiveInfinity ? 0 : width;

      if (_restrictedWidth != restrictedWidth)
      {
        _isDirty = true;
        _restrictedWidth = restrictedWidth;
        return true;
      }

      return false;
      // ReSharper restore CompareOfFloatsByEqualityOperator
    }

    internal void Render(DrawingContext drawingContext)
    {
      RecalculateIfDirty();

      foreach (var line in _cachedLines)
      {
        // Draw the formatted text into the drawing context.
        line.Line.Draw(drawingContext, line.Point, InvertAxes.None);
      }
    }

    /// <summary> If we're dirty, throw out the old data and recalculate it all. </summary>
    private void RecalculateIfDirty()
    {
      if (_isDirty)
      {
        foreach (var it in _cachedLines)
        {
          it.Line.Dispose();
        }

        _cachedLines.Clear();
        _height = GetLinesToDraw(_cachedLines);
        _isDirty = false;
      }
    }

    public double GetHeight()
    {
      RecalculateIfDirty();
      return _height;
    }

    /// <summary> Measures the character at the given index for the given fragment. </summary>
    /// <param name="fragment"> The fragment that owns the character. </param>
    /// <param name="characterIndex"> The index of the character to measure. </param>
    /// <returns> The size of the character. </returns>
    public MeasuredRectangle MeasureCharacter(StyledStyledTextSpanView fragment, int characterIndex)
    {
      this.RecalculateIfDirty();

      int totalLength = 0;
      foreach (var item in _cachedLines)
      {
        totalLength += item.Line.Length;
        if (characterIndex < totalLength)
        {
          var bounds = item.Line.GetTextBounds(characterIndex, 1)[0];
          return new MeasuredRectangle()
                 {
                   X = bounds.Rectangle.X,
                   Y = bounds.Rectangle.Y + item.Point.Y,
                   Width = bounds.Rectangle.Width,
                   Height = bounds.Rectangle.Height
                 };
        }
      }

      return MeasuredRectangle.Invalid;
    }

    private double GetLinesToDraw(List<TextLineContainer> linesToDraw)
    {
      var textStore = new CustomTextSource(_block);

      // Create a TextFormatter object.
      TextFormatter formatter = TextFormatter.Create(TextFormattingMode.Display);

      int textStorePosition = 0;

      int textLength = _spans.Sum(s => ((StyledTextFragment)s.DocumentItem).Length);
      Point linePosition = new Point();

      double maxWidth = 0;

      // Format each line of text from the text store and draw it.
      while (textStorePosition < textLength)
      {
        // Create a textline from the text store using the TextFormatter object.
        TextLine myTextLine = formatter.FormatLine(
          textStore,
          textStorePosition,
          _restrictedWidth,
          new GenericTextParagraphProperties(textLength == 0),
          null);

        linesToDraw.Add(new TextLineContainer(linePosition, myTextLine, textStorePosition));

        // Update the index position in the text store.
        textStorePosition += myTextLine.Length;

        // Update the line position coordinate for the displayed line.
        linePosition.Y += myTextLine.Height;

        maxWidth = Math.Max(maxWidth, myTextLine.Width);
      }

      MaxWidth = maxWidth;

      return linePosition.Y;
    }

    public void Invalidate()
    {
      _isDirty = true;
    }

    /// <summary> Container for holding the cached line and point for rendering. </summary>
    private struct TextLineContainer
    {
      public TextLine Line { get; }
      public int CharacterStartIndex { get; }
      public Point Point { get; }

      public TextLineContainer(Point point, TextLine line, int characterStartIndex)
      {
        Line = line;
        CharacterStartIndex = characterStartIndex;
        Point = point;
      }
    }

    private class GenericTextParagraphProperties : TextParagraphProperties
    {
      public GenericTextParagraphProperties(bool isFirst)
      {
        FlowDirection = FlowDirection.LeftToRight;
        TextAlignment = TextAlignment.Left;
        LineHeight = 20;
        FirstLineInParagraph = isFirst;
        DefaultTextRunProperties = new CustomStringRenderer.GenericTextRunProperties();
        TextWrapping = TextWrapping.Wrap;
        TextMarkerProperties = null;
        Indent = 0;
      }

      public override FlowDirection FlowDirection { get; }
      public override TextAlignment TextAlignment { get; }
      public override double LineHeight { get; }
      public override bool FirstLineInParagraph { get; }
      public override TextRunProperties DefaultTextRunProperties { get; }
      public override TextWrapping TextWrapping { get; }
      public override TextMarkerProperties TextMarkerProperties { get; }
      public override double Indent { get; }
    }

    private class CustomTextSource : TextSource
    {
      private readonly TextBlock _block;

      public CustomTextSource(TextBlock block)
      {
        _block = block;
      }

      /// <inheritdoc />
      public override TextRun GetTextRun(int textSourceCharacterIndex)
      {
        int startIndex = 0;
        var fragment = _block.FirstFragment;

        while (fragment != null)
        {
          int endIndex = startIndex + fragment.Length;

          if (endIndex <= textSourceCharacterIndex)
            break;

          if (startIndex <= textSourceCharacterIndex)
            return new TextCharacters(
              fragment.GetText(),
              textSourceCharacterIndex - startIndex,
              endIndex - textSourceCharacterIndex,
              new GenericTextRunProperties()
            );

          startIndex += fragment.Length;
          fragment = fragment.Next;
        }

        return new TextEndOfParagraph(1);
      }


      /// <inheritdoc />
      public override TextSpan<CultureSpecificCharacterBufferRange> GetPrecedingText(int textSourceCharacterIndexLimit)
      {
        // TODO?
        return new TextSpan<CultureSpecificCharacterBufferRange>(0, new CultureSpecificCharacterBufferRange(CultureInfo.CurrentCulture, CharacterBufferRange.Empty));
      }

      /// <inheritdoc />
      public override int GetTextEffectCharacterIndexFromTextSourceCharacterIndex(int textSourceCharacterIndex)
      {
        return 0;
      }
    }

    private class GenericTextRunProperties : TextRunProperties
    {
      public GenericTextRunProperties()
      {
        Typeface = new Typeface("Tahoma");
        FontRenderingEmSize = FontHintingEmSize = 16;
        TextDecorations = new TextDecorationCollection();
        ForegroundBrush = new SolidColorBrush() { Color = Colors.Black };
        BackgroundBrush = new SolidColorBrush() { Color = Colors.White };
        CultureInfo = CultureInfo.CurrentCulture;
        TextEffects = new TextEffectCollection();
      }

      public override Typeface Typeface { get; }
      public override double FontRenderingEmSize { get; }
      public override double FontHintingEmSize { get; }
      public override TextDecorationCollection TextDecorations { get; }
      public override Brush ForegroundBrush { get; }
      public override Brush BackgroundBrush { get; }
      public override CultureInfo CultureInfo { get; }
      public override TextEffectCollection TextEffects { get; }
    }
  }
}