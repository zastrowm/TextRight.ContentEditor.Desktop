using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.TextFormatting;
using TextRight.Core;
using TextRight.Core.Editing;
using TextRight.Core.ObjectModel.Blocks.Text;
using TextRight.Core.Utilities;
using TextRight.Editor.View.Blocks;

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
    private BlockBasedTextSource _textSource;
    private TextFormatter _textFormatter;

    public CustomStringRenderer(TextBlock block, List<StyledStyledTextSpanView> spans)
    {
      _block = block;
      _spans = spans;
      _restrictedWidth = 100;
      _cachedLines = new List<TextLineContainer>();

      _textSource = new BlockBasedTextSource(_block);
      _textFormatter = TextFormatter.Create(TextFormattingMode.Display);
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

    /// <summary> Gets the cursor at the designated point. </summary>
    /// <param name="point"> The point at which the cursor should be pointing.. </param>
    /// <returns> The cursor at the designated point. </returns>
    public TextCaret GetCursor(DocumentPoint point)
    {
      var (currentLine, numberOfCharactersBeforeLine) = GetLineForYPosition(point.Y);

      var characterHit = currentLine.Line.GetCharacterHitFromDistance(point.X);
      int absoluteIndexOfCharacter = characterHit.FirstCharacterIndex;

      var (fragment, numberOfCharactersBeforeFragment) = TextBlockUtils.GetFragmentFromBlockCharacterIndex(absoluteIndexOfCharacter, _block);

      int indexOfCharacterInFragment = absoluteIndexOfCharacter - numberOfCharactersBeforeFragment;

      var cursor = new TextCaret(fragment, indexOfCharacterInFragment);

      // We clicked on a character, but the caret position actually represents the left side of the character.
      // For example, given "|a|", when we click on 'a', the | represents the possible places for the caret
      // to be placed.  If we're closer to the left, choose the current caret position.  If the right is closer
      // to where we clicked, choose the next caret position.
      var nextPosition = cursor.MoveForward();
      if (nextPosition.IsValid)
      {
        var positionPrevious = MeasureCharacter(cursor).FlattenLeft();
        var positionNext = MeasureCharacter(nextPosition).FlattenLeft();

        var distancePrevious = DocumentPoint.MeasureDistanceSquared(positionPrevious.Center, point);
        var distanceNext = DocumentPoint.MeasureDistanceSquared(positionNext.Center, point);

        // prefer left over right, thus the <
        if (distanceNext < distancePrevious)
        {
          cursor = nextPosition;
        }
      }

      return cursor;
    }

    private (TextLineContainer line, int numCharactersBefore) GetLineForYPosition(double y)
    {
      TextLineContainer currentLine = _cachedLines[0];

      int numberOfCharactersBeforeLine = 0;

      foreach (var lineInfo in _cachedLines)
      {
        currentLine = lineInfo;

        if (y < lineInfo.Point.Y + lineInfo.Line.Height)
        {
          break;
        }

        numberOfCharactersBeforeLine += currentLine.CharacterStartIndex;
      }

      return (currentLine, numberOfCharactersBeforeLine);
    }

    public MeasuredRectangle MeasureCharacter(TextCaret cursor)
    {
      RecalculateIfDirty();

      int characterIndex = TextBlockUtils.GetCharacterIndex(cursor);

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
      // Create a TextFormatter object.
      int textStorePosition = 0;

      var textLength = GetTotalTextLength();
      Point currentLinePosition = new Point();

      double maxWidth = 0;
      bool isFirst = true;

      // Format each line of text from the text store and draw it.
      while (textStorePosition < textLength)
      {
        // OPTIMIZE we could just re-format the lines that changed, not everything
        // (if the change was text being added/removed)

        // Create a textline from the text store using the TextFormatter object.
        TextLine myTextLine = _textFormatter.FormatLine(
          _textSource,
          textStorePosition,
          _restrictedWidth,
          new GenericTextParagraphProperties(isFirst),
          null);

        linesToDraw.Add(new TextLineContainer(currentLinePosition, myTextLine, textStorePosition));

        // Update the index position in the text store.
        textStorePosition += myTextLine.Length;

        // Update the line position coordinate for the displayed line.
        currentLinePosition.Y += myTextLine.Height;

        maxWidth = Math.Max(maxWidth, myTextLine.Width);
        isFirst = false;
      }

      MaxWidth = maxWidth;

      return currentLinePosition.Y;
    }

    private int GetTotalTextLength()
    {
      int textLength = 0;
      foreach (var s in _spans)
      {
        textLength += ((StyledTextFragment)s.DocumentItem).Length;
      }
      return textLength;
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
        DefaultTextRunProperties = new StyledTextRunProperties();
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

    private struct FragmentInfo
    {
      public FragmentInfo(StyledTextFragment fragment, int startIndex, int endIndex)
        : this()
      {
        Fragment = fragment;
        StartIndex = startIndex;
        EndIndex = endIndex;
      }

      public StyledTextFragment Fragment { get; }
      public int StartIndex { get; }
      public int EndIndex { get; }
    }

    private class FragmentBasedTextCharacters : TextCharacters
    {
      public FragmentBasedTextCharacters(
        int characterStartIndex,
        FragmentInfo fragmentInfo,
        StyledTextRunProperties properties)
        : base(fragmentInfo.Fragment.GetText(),
               characterStartIndex - fragmentInfo.StartIndex,
               fragmentInfo.EndIndex - characterStartIndex,
               properties)
      {
        FragmentInfo = fragmentInfo;
      }

      public FragmentInfo FragmentInfo { get; }
    }

    /// <summary> Provides formatted text from a TextBlock. </summary>
    private class BlockBasedTextSource : TextSource
    {
      private readonly TextBlock _block;

      public BlockBasedTextSource(TextBlock block)
      {
        _block = block;
      }

      /// <inheritdoc />
      public override TextRun GetTextRun(int desiredCharacterIndex)
      {
        int startIndex = 0;
        var fragment = _block.Content.FirstFragment;

        while (fragment != null)
        {
          int endIndex = startIndex + fragment.Length;

          if (endIndex <= desiredCharacterIndex)
            break;

          if (startIndex <= desiredCharacterIndex)
            return new FragmentBasedTextCharacters(desiredCharacterIndex,
                                                   new FragmentInfo(fragment, startIndex, endIndex),
                                                   new StyledTextRunProperties());

          startIndex += fragment.Length;
          fragment = fragment.Next;
        }

        return new TextEndOfParagraph(1);
      }

      /// <inheritdoc />
      public override TextSpan<CultureSpecificCharacterBufferRange> GetPrecedingText(int textSourceCharacterIndexLimit)
      {
        Debug.Fail("When is this called");
        // TODO?
        return new TextSpan<CultureSpecificCharacterBufferRange>(
          0,
          new CultureSpecificCharacterBufferRange(CultureInfo.CurrentCulture, CharacterBufferRange.Empty));
      }

      /// <inheritdoc />
      public override int GetTextEffectCharacterIndexFromTextSourceCharacterIndex(int textSourceCharacterIndex)
      {
        Debug.Fail("When is this called");
        return 0;
      }
    }
  }
}