using System;
using System.Collections.Generic;
using System.Linq;
using TextRight.ContentEditor.Core.ObjectModel.Blocks;

namespace TextRight.ContentEditor.Core.Editing.Commands
{
  /// <summary>
  ///  Commands that are built-in to the core system that have to do with caret
  ///  navigation.
  /// </summary>
  public sealed class BuiltInCaretNavigationCommand : CaretNavigationCommand
  {
    public static BuiltInCaretNavigationCommand Forward { get; }
      = new BuiltInCaretNavigationCommand("Caret.Forward", NavigationType.Forward);

    public static BuiltInCaretNavigationCommand Backward { get; }
      = new BuiltInCaretNavigationCommand("Caret.Backward", NavigationType.Backward);

    public static BuiltInCaretNavigationCommand Up { get; }
      = new BuiltInCaretNavigationCommand("Caret.Up", NavigationType.Up, isCursorStatePreserved: true);

    public static BuiltInCaretNavigationCommand Down { get; }
      = new BuiltInCaretNavigationCommand("Caret.Down", NavigationType.Down, isCursorStatePreserved: true);

    public static BuiltInCaretNavigationCommand NextWord { get; }
      = new BuiltInCaretNavigationCommand("Caret.NextWord", NavigationType.NextWord);

    public static BuiltInCaretNavigationCommand PreviousWord { get; }
      = new BuiltInCaretNavigationCommand("Caret.PreviousWord", NavigationType.PreviousWord);

    public static BuiltInCaretNavigationCommand Home { get; }
      = new BuiltInCaretNavigationCommand("Caret.Home", NavigationType.Home, isCursorStatePreserved: true);

    public static BuiltInCaretNavigationCommand End { get; }
      = new BuiltInCaretNavigationCommand("Caret.End", NavigationType.End, isCursorStatePreserved: true);

    public static BuiltInCaretNavigationCommand BeginningOfBlock { get; }
      = new BuiltInCaretNavigationCommand("Caret.BeginningOfBlock", NavigationType.BeginningOfBlock);

    public static BuiltInCaretNavigationCommand EndOfBlock { get; }
      = new BuiltInCaretNavigationCommand("Caret.EndOfBlock", NavigationType.EndOfBlock);

    /// <summary>
    ///  A pipeline hook that correctly sets up the CaretMovementMode for the
    ///  DocumentContext.
    /// </summary>
    public static ICommandProcessorPipelineHook PipelineHook { get; }
      = new ProcessorHook();

    /// <summary> Constructor. </summary>
    /// <param name="id"> The identifier. </param>
    /// <param name="mode"> The mode of the command. </param>
    /// <param name="isCursorStatePreserved"> True if the command keeps the cursor state. </param>
    private BuiltInCaretNavigationCommand(string id, NavigationType mode, bool isCursorStatePreserved = false)
      : base(id)
    {
      Mode = mode;
      IsResetCursorStateRequired = !isCursorStatePreserved;
    }

    /// <summary> The mode of the command. </summary>
    public NavigationType Mode { get; }

    /// <summary>
    ///  True if the document context's CaretMovementMode should be reset.
    /// </summary>
    public bool IsResetCursorStateRequired { get; set; }

    /// <summary> The type of built-in navigation command to handle. </summary>
    public enum NavigationType
    {
      Forward,
      Backward,
      Up,
      Down,
      NextWord,
      PreviousWord,
      Home,
      End,
      BeginningOfBlock,
      EndOfBlock,
    }

    /// <summary> Sets up the caret state for various commands.  </summary>
    private class ProcessorHook : ICommandProcessorPipelineHook
    {
      /// <inheritdoc />
      void ICommandProcessorPipelineHook.PreProcess(EditorCommand command, DocumentEditorContext context)
      {
        var builtIn = command as BuiltInCaretNavigationCommand;
        if (builtIn == null)
          return;

        switch (builtIn.Mode)
        {
          case NavigationType.Up:
          case NavigationType.Down:
            if (context.CaretMovementMode.CurrentMode == CaretMovementMode.Mode.None)
            {
              // make sure that when we move up/down, we have a non-None mode
              TextBlockCursor textBlockCursor = (TextBlockCursor)context.BlockCursor;
              context.CaretMovementMode.SetModeToPosition(textBlockCursor.MeasureCursorPosition().Left);
            }
            break;
          case NavigationType.Home:
            context.CaretMovementMode.SetModeToHome();
            break;
          case NavigationType.End:
            context.CaretMovementMode.SetModeToEnd();
            break;
          default:
            // if we move the caret and the command doesn't actually affect the cursor
            // state, then the state should be reset.  This is what preserves up/down
            // positioning. 
            if (builtIn.IsResetCursorStateRequired)
            {
              context.CaretMovementMode.SetModeToNone();
            }
            break;
        }
      }

      /// <inheritdoc />
      void ICommandProcessorPipelineHook.PostProcess(EditorCommand command, DocumentEditorContext context)
      {
        // no-op
      }
    }
  }
}