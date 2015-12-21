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
    public static void MoveCaretToBeginningOfLine(DocumentEditorContext context)
    {
      TextBlockCursorMover.BackwardMover.MoveCaretTowardsLineEdge(context);
      context.CaretMovementMode.SetModeToHome();
    }

    /// <summary> Move the caret to the end of the current line . </summary>
    /// <exception cref="ArgumentOutOfRangeException"> Thrown when one or more
    ///  arguments are outside the required range. </exception>
    /// <param name="context"> The context. </param>
    public static void MoveCaretToEndOfLine(DocumentEditorContext context)
    {
      TextBlockCursorMover.ForwardMover.MoveCaretTowardsLineEdge(context);
      context.CaretMovementMode.SetModeToEnd();
    }

    public static void MoveCaretUpInDocument(DocumentEditorContext context)
    {
      TextBlockCursorMover.BackwardMover.MoveCaretTowardsPositionInNextLine(context);
    }

    public static void MoveCaretDownInDocument(DocumentEditorContext context)
    {
      TextBlockCursorMover.ForwardMover.MoveCaretTowardsPositionInNextLine(context);
    }
  }
}