using System;
using System.Collections.Generic;
using System.Linq;

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
      = new BuiltInCaretNavigationCommand("Caret.Up", NavigationType.Up);

    public static BuiltInCaretNavigationCommand Down { get; }
      = new BuiltInCaretNavigationCommand("Caret.Down", NavigationType.Down);

    public static BuiltInCaretNavigationCommand NextWord { get; }
      = new BuiltInCaretNavigationCommand("Caret.NextWord", NavigationType.NextWord);

    public static BuiltInCaretNavigationCommand PreviousWord { get; }
      = new BuiltInCaretNavigationCommand("Caret.PreviousWord", NavigationType.PreviousWord);

    public static BuiltInCaretNavigationCommand Home { get; }
      = new BuiltInCaretNavigationCommand("Caret.Home", NavigationType.Home);

    public static BuiltInCaretNavigationCommand End { get; }
      = new BuiltInCaretNavigationCommand("Caret.End", NavigationType.End);

    public static BuiltInCaretNavigationCommand BeginningOfBlock { get; }
      = new BuiltInCaretNavigationCommand("Caret.BeginningOfBlock", NavigationType.BeginningOfBlock);

    public static BuiltInCaretNavigationCommand EndOfBlock { get; }
      = new BuiltInCaretNavigationCommand("Caret.EndOfBlock", NavigationType.EndOfBlock);

    /// <summary> Constructor. </summary>
    /// <param name="id"> The identifier. </param>
    /// <param name="mode"> The mode of the command. </param>
    private BuiltInCaretNavigationCommand(string id, NavigationType mode)
      : base(id)
    {
      Mode = mode;
    }

    /// <summary> The mode of the command. </summary>
    public NavigationType Mode { get; }

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
  }
}