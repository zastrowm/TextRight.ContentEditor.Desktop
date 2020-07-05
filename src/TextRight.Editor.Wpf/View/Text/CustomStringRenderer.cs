using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.TextFormatting;
using TextRight.Core;
using TextRight.Core.ObjectModel.Blocks.Text;
using TextRight.Core.Utilities;
using TextRight.Editor.Text;

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
    private readonly BlockBasedTextSource _textSource;
    private readonly TextFormatter _textFormatter;
    private Size _size;

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
    
    public void Invalidate()
    {
      _isDirty = true;
    }

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
    public IVisualLine<TextCaret> GetLineFor(TextCaret caret)
    {
      var line = FirstTextLine;

      // TODO use binary search

      // NOTE - we need to pass it index into the larger text string.  Not sure if that's the underlying 
      // string or a some other buffer (The TextRun?, the Paragraph?)
      while (line.Next != null && line.Next.GetContainer().Offset.GraphemeOffset <= caret.Offset.GraphemeOffset)
        line = line.Next;

      return line;
    }

    /// <summary> Renders the lines in this string at the given offset. </summary>
    internal void Render(DrawingContext drawingContext, Point offset)
    {
      RecalculateIfDirty();

      // TODO refactor
      var textLine = FirstTextLine;

      foreach (var cachedLine in CachedLines)
      {
        // Draw the formatted text into the drawing context.
        var drawPoint = new Point(cachedLine.Point.X + offset.X, cachedLine.Point.Y + offset.Y);
        cachedLine.Line.Draw(drawingContext, drawPoint, InvertAxes.None);

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
        _size = CalculateLinesToDraw(CachedLines);
        _isDirty = false;
      }
    }

    /// <summary>
    ///   Gets the size of the text to be rendered.
    /// </summary>
    public Size GetSize()
    {
      RecalculateIfDirty();
      return _size;
    }
 
    /// <summary> Gets the caret at the designated point. </summary>
    /// <param name="point"> The point at which the cursor should be pointing. </param>
    /// <returns> The caret at the designated point. </returns>
    public TextCaret GetCaret(DocumentPoint point)
    {
      var caret = GetCaretAtPointClicked(point);

      // We clicked on a character, but the caret position actually represents the left side of the character.
      // For example, given "|a|", when we click on 'a', the | represents the possible places for the caret
      // to be placed.  If we're closer to the left, choose the current caret position.  If the right is closer
      // to where we clicked, choose the next caret position.
      var nextPosition = caret.GetNextPosition();
      if (nextPosition.IsValid)
      {
        var positionPrevious = MeasureCharacter(caret).FlattenLeft();
        var positionNext = MeasureCharacter(nextPosition).FlattenLeft();

        var distancePrevious = DocumentPoint.MeasureDistanceSquared(positionPrevious.Center, point);
        var distanceNext = DocumentPoint.MeasureDistanceSquared(positionNext.Center, point);

        // prefer left over right, thus the <
        if (distanceNext < distancePrevious)
        {
          caret = nextPosition;
        }
      }

      return caret;
    }

    /// <summary> Gets a caret representing the next character closest to where the point is. </summary>
    private TextCaret GetCaretAtPointClicked(DocumentPoint point)
    {
      var (currentLine, numberOfCharactersBeforeLine) = GetLineForYPosition(point.Y);

      // note - this returns the character index (e.g. 'char'), not the grapheme index, so we'll have
      // to convert it further down. 
      var characterHit = currentLine.Line.GetCharacterHitFromDistance(point.X);
      int absoluteIndexOfCharacter = characterHit.FirstCharacterIndex;

      return TextCaret.FromCharacterIndex(_block.Content, absoluteIndexOfCharacter);
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

    private Size CalculateLinesToDraw(List<TextLineContainer> linesToDraw)
    {
      var textLengthInChars = _block.Content.TextLength;
      var currentLinePosition = new Point();

      double maxWidth = 0;
      bool isFirst = true;

      int textStorePositionInChars = 0;
      //int textStorePositionInGraphemes = 0;

      var caret = TextCaret.FromBeginning(_block.Content);

      // Format each line of text from the text store and draw it.
      while (textStorePositionInChars < textLengthInChars)
      {
        // OPTIMIZE we could just re-format the lines that changed, not everything
        // (if the change was text being added/removed)

        // Create a textline from the text store using the TextFormatter object.
        TextLine myTextLine = _textFormatter.FormatLine(
          _textSource,
          textStorePositionInChars,
          _restrictedWidth,
          new GenericTextParagraphProperties(isFirst),
          null);

        linesToDraw.Add(new TextLineContainer(currentLinePosition, myTextLine, caret.Offset, _block.Content));

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

      // we always want at least one line to be rendered so that the block renders the correct height
      if (linesToDraw.Count == 0)
      {
        var myTextLine = _textFormatter.FormatLine(new BlankLineSource(_textSource),
                                                   textStorePositionInChars,
                                                   _restrictedWidth,
                                                   new GenericTextParagraphProperties(isFirst: true),
                                                   null);
        currentLinePosition.Y += myTextLine.Height;

        linesToDraw.Add(new TextLineContainer(currentLinePosition, myTextLine, caret.Offset, _block.Content));
      }

      return new Size(maxWidth, currentLinePosition.Y);
    }

    private class BlankLineSource : TextSource
    {
      private BlockBasedTextSource _properties;

      public BlankLineSource(BlockBasedTextSource properties)
      {
        _properties = properties;
      }

      public override TextRun GetTextRun(int textSourceCharacterIndex)
      {
        // render a single character with the correct font characteristics
        if (textSourceCharacterIndex == 0)
        {
          return new TextCharacters(" ",
                                    0,
                                    1,
                                    _properties.CreateTextSpanRunProperties());
        }

        return new TextEndOfParagraph(1, _properties.CreateTextSpanRunProperties());
      }

      public override TextSpan<CultureSpecificCharacterBufferRange> GetPrecedingText(int textSourceCharacterIndexLimit)
        => throw new NotImplementedException();

      public override int GetTextEffectCharacterIndexFromTextSourceCharacterIndex(int textSourceCharacterIndex)
        => throw new NotImplementedException();
    }
  }
}