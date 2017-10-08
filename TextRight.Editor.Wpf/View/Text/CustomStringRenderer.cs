using System;
using System.Collections.Generic;
using System.Diagnostics;
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
    private readonly IOffsetBasedItem _parent;
    private readonly TextBlock _block;
    private readonly List<StyledStyledTextSpanView> _spans;
    private readonly LineBasedRenderer _cachedLinesBased;
    private bool _isDirty = true;
    private double _restrictedWidth;
    private double _height;
    private BlockBasedTextSource _textSource;
    private TextFormatter _textFormatter;

    public CustomStringRenderer(IOffsetBasedItem parent, TextBlock block, List<StyledStyledTextSpanView> spans)
    {
      _parent = parent;
      _block = block;
      _spans = spans;
      _restrictedWidth = 100;
      _cachedLinesBased = new LineBasedRenderer(parent);

      _textSource = new BlockBasedTextSource(_block);
      _textFormatter = TextFormatter.Create(TextFormattingMode.Display);
    }

    /// <summary> The maximum width of the lines in this renderer </summary>
    public double MaxWidth { get; private set; }

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

    internal void Render(DrawingContext drawingContext)
    {
      RecalculateIfDirty();

      foreach (var line in _cachedLinesBased)
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
        foreach (var it in _cachedLinesBased)
        {
          it.Line.Dispose();
        }

        _cachedLinesBased.Clear();
        _height = GetLinesToDraw(_cachedLinesBased);
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
      TextLineContainer currentLine = _cachedLinesBased[0];

      int numberOfCharactersBeforeLine = 0;

      foreach (var lineInfo in _cachedLinesBased)
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

      var line = _cachedLinesBased.FirstTextLine;

      int totalLengthThusFar = 0;

      // NOTE - we need to pass it index into the larger text string.  Not sure if that's the underlying 
      // string or a some other buffer (The TextRun?, the Paragraph?)
      while (line != null)
      {
        totalLengthThusFar += line.NumberOfCaretPositions;

        if (characterIndex < totalLengthThusFar)
        {
          return line.GetMeasurement(cursor);
        }

        line = line.Next;
      }

      return MeasuredRectangle.Invalid;
    }

    private double GetLinesToDraw(List<TextLineContainer> linesToDraw)
    {
      var textLengthInChars = GetTotalTextLength();
      Point currentLinePosition = new Point();

      double maxWidth = 0;
      bool isFirst = true;

      StyledTextFragment currentFragment = _block.Content.FirstFragment;
      int currentFragmentOffset = 0;

      int textStorePositionInChars = 0;

      // Format each line of text from the text store and draw it.
      while (textStorePositionInChars < textLengthInChars)
      {
        // OPTIMIZE we could just re-format the lines that changed, not everything
        // (if the change was text being added/removed)

        // TODO when we switch length to be graphemes, this will have to change
        while (textStorePositionInChars > currentFragmentOffset + currentFragment.NumberOfChars)
        {
          currentFragmentOffset += currentFragment.NumberOfChars;
          currentFragment = currentFragment.Next;
        }

        // Create a textline from the text store using the TextFormatter object.
        TextLine myTextLine = _textFormatter.FormatLine(
          _textSource,
          textStorePositionInChars,
          _restrictedWidth,
          new GenericTextParagraphProperties(isFirst),
          null);

        linesToDraw.Add(new TextLineContainer(currentLinePosition, myTextLine, textStorePositionInChars, currentFragment, currentFragmentOffset));

        // Update the index position in the text store.
        textStorePositionInChars += myTextLine.Length;

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
        textLength += ((StyledTextFragment)s.DocumentItem).NumberOfChars;
      }
      return textLength;
    }

    public void Invalidate()
    {
      _isDirty = true;
    }
  }
}