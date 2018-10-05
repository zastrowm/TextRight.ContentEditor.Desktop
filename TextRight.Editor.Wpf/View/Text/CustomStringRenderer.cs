using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.TextFormatting;
using TextRight.Core;
using TextRight.Core.ObjectModel.Blocks.Text;
using TextRight.Core.ObjectModel.Blocks.Text.View;
using TextRight.Core.Utilities;
using TextRight.Editor.Wpf.View.Text;

namespace TextRight.Editor.Wpf.View
{
  /// <summary> Responsible for rendering data within a textblock. </summary>
  // TODO optimize so that we actually sometimes keep TextLineRender around
  internal class CustomStringRenderer : IOffsetBasedItem
  {
    private readonly IOffsetBasedItem _parent;
    private readonly TextBlock _block;
    private bool _isDirty = true;
    private double _restrictedWidth;
    private double _height;
    private readonly BlockBasedTextSource _textSource;
    private readonly TextFormatter _textFormatter;

    public CustomStringRenderer(IOffsetBasedItem parent, TextBlock block)
    {
      _parent = parent;
      _block = block;
      _restrictedWidth = 100;
      CachedLines = new List<TextLineContainer>();

      _textSource = new BlockBasedTextSource(_block);
      _textFormatter = TextFormatter.Create(TextFormattingMode.Display);
    }

    public List<TextLineContainer> CachedLines { get; }

    /// <summary> The maximum width of the lines in this renderer </summary>
    public double MaxWidth { get; private set; }

    public int FontSize
    {
      get => _textSource.FontSize;
      set
      {
        _textSource.FontSize = value;
        _isDirty = true;
      }
    }

    public Point Offset 
      => _parent.Offset;

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

    /// <summary />
    public TextLineRender FirstTextLine
      => new TextLineRender(this, 0);

    /// <summary />
    public TextLineRender LastTextLine
      => new TextLineRender(this, CachedLines.Count - 1);

    /// <inheritdoc />
    public ITextLine GetLineFor(TextCaret caret)
    {
      var line = FirstTextLine;

      // NOTE - we need to pass it index into the larger text string.  Not sure if that's the underlying 
      // string or a some other buffer (The TextRun?, the Paragraph?)
      while (line.Next != null && line.Next.GetContainer().Offset.GraphemeOffset <= caret.Offset.GraphemeOffset)
        line = line.Next;

      return line;
    }

    internal void Render(DrawingContext drawingContext)
    {
      RecalculateIfDirty();

      // TODO refactor
      var textLine = FirstTextLine;

      foreach (var cachedLine in CachedLines)
      {
        // Draw the formatted text into the drawing context.
        cachedLine.Line.Draw(drawingContext, cachedLine.Point, InvertAxes.None);

        if (GlobalFlags.ShouldShowDebugTextGraphics)
        {
          textLine.DebugDraw(drawingContext);
        }

        textLine = textLine.Next;
      }
    }

    /// <summary> If we're dirty, throw out the old data and recalculate it all. </summary>
    private void RecalculateIfDirty()
    {
      if (_isDirty)
      {
        foreach (var it in CachedLines)
        {
          it.Line.Dispose();
        }

        CachedLines.Clear();
        _height = GetLinesToDraw(CachedLines);
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

      var cursor = TextCaret.FromOffset(fragment, indexOfCharacterInFragment);

      // We clicked on a character, but the caret position actually represents the left side of the character.
      // For example, given "|a|", when we click on 'a', the | represents the possible places for the caret
      // to be placed.  If we're closer to the left, choose the current caret position.  If the right is closer
      // to where we clicked, choose the next caret position.
      var nextPosition = cursor.GetNextPosition();
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
      TextLineContainer currentLine = CachedLines[0];

      int numberOfCharactersBeforeLine = 0;

      foreach (var lineInfo in CachedLines)
      {
        currentLine = lineInfo;

        if (y < lineInfo.Point.Y + lineInfo.Line.Height)
        {
          break;
        }

        numberOfCharactersBeforeLine += currentLine.Offset.CharOffset;
      }

      return (currentLine, numberOfCharactersBeforeLine);
    }

    public MeasuredRectangle MeasureCharacter(TextCaret caret)
    {
      RecalculateIfDirty();

      return GetLineFor(caret)?.GetMeasurement(caret)
             ?? MeasuredRectangle.Invalid;
    }

    private double GetLinesToDraw(List<TextLineContainer> linesToDraw)
    {
      var textLengthInChars = GetTotalTextLength();
      Point currentLinePosition = new Point();

      double maxWidth = 0;
      bool isFirst = true;

      TextSpan currentSpan = _block.Content.FirstSpan;
      int currentFragmentOffset = 0;

      int textStorePositionInChars = 0;
      //int textStorePositionInGraphemes = 0;

      var caret = TextCaret.FromBeginning(_block.Content);

      // Format each line of text from the text store and draw it.
      while (textStorePositionInChars < textLengthInChars)
      {
        // OPTIMIZE we could just re-format the lines that changed, not everything
        // (if the change was text being added/removed)

        // TODO when we switch length to be graphemes, this will have to change
        while (textStorePositionInChars > currentFragmentOffset + currentSpan.NumberOfChars)
        {
          currentFragmentOffset += currentSpan.NumberOfChars;
          currentSpan = currentSpan.Next;
        }

        // Create a textline from the text store using the TextFormatter object.
        TextLine myTextLine = _textFormatter.FormatLine(
          _textSource,
          textStorePositionInChars,
          _restrictedWidth,
          new GenericTextParagraphProperties(isFirst),
          null);

        linesToDraw.Add(new TextLineContainer(currentLinePosition, myTextLine, caret.Offset, currentSpan));

        // Update the index position in the text store.
        textStorePositionInChars += myTextLine.Length;

        while (caret.IsValid && caret.Offset.CharOffset != textStorePositionInChars)
        {
          caret = caret.GetNextPosition();
        }

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

      foreach (var s in _block.Content.Spans)
      {
        textLength += s.NumberOfChars;
      }
      return textLength;
    }

    public void Invalidate()
    {
      _isDirty = true;
    }
  }
}