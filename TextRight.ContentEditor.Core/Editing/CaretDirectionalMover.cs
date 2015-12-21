using System;
using System.Collections.Generic;
using System.Linq;

namespace TextRight.ContentEditor.Core.Editing
{
  /// <summary> Moves the caret up or down. </summary>
  internal static class CaretDirectionalMover
  {
    /// <summary> Moves the caret to the beginning of the line. </summary>
    /// <param name="context"> The context. </param>
    /// <returns> True if the caret moved, false otherwise. </returns>
    public static bool MoveCaretToBeginningOfLine(DocumentEditorContext context)
    {
      return MoveLeftMover.MoveCaretTowardsLineEdge(context);
    }

    /// <summary> Move the caret to the end of the current line . </summary>
    /// <exception cref="ArgumentOutOfRangeException"> Thrown when one or more
    ///  arguments are outside the required range. </exception>
    /// <param name="context"> The context. </param>
    /// <returns> True if the cursor moved, false otherwise. </returns>
    public static bool MoveCaretToEndOfLine(DocumentEditorContext context)
    {
      return MoveRightMover.MoveCaretTowardsLineEdge(context);
    }

    public static void MoveCaretUpInDocument(DocumentEditorContext context)
    {
      MoveLeftMover.MoveCaretTowardsPositionInNextLine(context);
    }

    public static void MoveCaretDownInDocument(DocumentEditorContext context)
    {
      MoveRightMover.MoveCaretTowardsPositionInNextLine(context);
    }

    private static readonly TextBlockCursorMover MoveRightMover = TextBlockCursorMover.ForwardMover;
    private static readonly TextBlockCursorMover MoveLeftMover = TextBlockCursorMover.BackwardMover;
  }
}