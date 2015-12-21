using System;
using System.Collections.Generic;
using System.Linq;

namespace TextRight.ContentEditor.Core.Editing.Commands
{
  /// <summary> Commands that operate on the current caret. </summary>
  public static class CaretCommands
  {
    public static ISimpleActionCommand MoveForward { get; }
      = new SimpleCaretActionCommand("Caret.MoveForward", e => e.Caret.MoveForward(),
                                     shouldPreserveCaretState: false);

    public static ISimpleActionCommand MoveBackward { get; }
      = new SimpleCaretActionCommand("Caret.MoveBackward", e => e.Caret.MoveBackward(),
                                     shouldPreserveCaretState: false);

    public static ISimpleActionCommand MoveToNextWord { get; }
      = new SimpleCaretActionCommand("Caret.MoveForwardWord",
                                     CaretWordMover.MoveCaretToBeginningOfNextWord,
                                     shouldPreserveCaretState: false);

    public static ISimpleActionCommand MoveToPreviousWord { get; }
      = new SimpleCaretActionCommand("Caret.MoveBackwardWord", CaretWordMover.MoveCaretToEndOfPreviousWord,
                                     shouldPreserveCaretState: false);

    public static ISimpleActionCommand MoveToBeginningOfLine { get; }
      = new SimpleCaretActionCommand("Caret.MoveToLineBeginning",
                                     c => MoveCaretToBeginningOfLine(c),
                                     shouldPreserveCaretState: false);

    public static ISimpleActionCommand MoveToEndOfLine { get; }
      = new SimpleCaretActionCommand("Caret.MoveToLineEnd",
                                     c => MoveCaretToEndOfLine(c),
                                     shouldPreserveCaretState: true);

    public static ISimpleActionCommand MoveUp { get; }
      = new SimpleCaretActionCommand("Caret.MoveUp",
                                     c => MoveCaretUpInDocument(c),
                                     shouldPreserveCaretState: true);

    public static ISimpleActionCommand MoveDown { get; }
      = new SimpleCaretActionCommand("Caret.MoveUp",
                                     c => MoveCaretDownInDocument(c),
                                     shouldPreserveCaretState: true);

    private static void MoveCaretToBeginningOfLine(DocumentEditorContext context)
    {
      TextBlockCursorMover.BackwardMover.MoveCaretTowardsLineEdge(context);
      context.CaretMovementMode.SetModeToHome();
    }

    private static void MoveCaretToEndOfLine(DocumentEditorContext context)
    {
      TextBlockCursorMover.ForwardMover.MoveCaretTowardsLineEdge(context);
      context.CaretMovementMode.SetModeToEnd();
    }

    private static void MoveCaretUpInDocument(DocumentEditorContext context)
    {
      TextBlockCursorMover.BackwardMover.MoveCaretTowardsPositionInNextLine(context);
    }

    private static void MoveCaretDownInDocument(DocumentEditorContext context)
    {
      TextBlockCursorMover.ForwardMover.MoveCaretTowardsPositionInNextLine(context);
    }
  }
}