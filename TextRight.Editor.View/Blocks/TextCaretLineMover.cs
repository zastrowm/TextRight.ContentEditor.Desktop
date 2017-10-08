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


  /// <summary> Utility methods for moving a cursor among lines. </summary>
  public static class TextCaretLineMover
  {

  }
}