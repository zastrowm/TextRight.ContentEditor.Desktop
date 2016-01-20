using System;
using System.Collections.Generic;
using System.Linq;

namespace TextRight.ContentEditor.Core.Editing.Commands
{
  /// <summary> Commands related to text processing. </summary>
  public static class TextCommands
  {
    public static EditorCommand DeleteNextCharacter { get; }
      = new EditorCommand("Caret.DeleteNextChar");

    public static EditorCommand DeletePreviousCharacter { get; }
      = new EditorCommand("Caret.DeletePreviousChar");

    public static EditorCommand BreakBlock { get; }
      = new EditorCommand("Block.Break");
  }
}