using System;
using System.Linq;
using System.Collections.Generic;
using TextRight.Core.ObjectModel.Blocks.Text;
using TextRight.Core.Utilities;

namespace TextRight.Editor.View.Blocks
{
  public struct TextCaretAndRenderer
  {
    public TextCaretAndRenderer(TextCaret caret, ITextBlockRenderer renderer)
    {
      Caret = caret;
      Renderer = renderer;
    }

    public TextCaret Caret { get; }

    public ITextBlockRenderer Renderer { get; }
  }

  /// <summary> A renderer that renders its contents via lines. </summary>
  public interface ILineBasedRenderer
  {
    /// <summary> The first line in the renderer. </summary>
    ILine FirstLine { get; }  
    /// <summary> The second line in the renderer. </summary>
    ILine LastLine { get; }

    /// <summary> Gets the line on which the caret appears. </summary>
    /// <param name="caret"> The caret for which the associated line should be retrieved. </param>
    /// <returns> The line for. </returns>
    ILine GetLineFor(TextCaret caret);
  }

  public interface ILine
  {
    int NumberOfCaretPositions { get; }
    double Height { get; }

    MeasuredRectangle GetBounds();

    ILine Next { get; }
    ILine Previous { get; }

    ILineIterator GetLineStart();
    ILineIterator GetLineEnd();
  }

  public interface ILineIterator
  {
    bool MoveNext();
    bool MovePrevious();

    TextCaret Caret { get; }
    MeasuredRectangle Measurement { get; }
  }

  /// <summary> Utility methods for moving a cursor among lines. </summary>
  public static class TextCaretLineMover
  {

  }
}