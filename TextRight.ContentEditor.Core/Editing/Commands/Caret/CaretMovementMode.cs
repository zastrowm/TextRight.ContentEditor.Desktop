using System;
using System.Collections.Generic;
using System.Linq;

namespace TextRight.ContentEditor.Core.Editing.Commands
{
  /// <summary>
  ///  Holds information about how a caret should move when navigating up or
  ///  down.
  /// </summary>
  public class CaretMovementMode
  {
    /// <summary>
    ///  The position of where the cursor should move towards when navigating up
    ///  or down.
    /// </summary>
    public double Position { get; private set; }

    /// <summary> The current mode of the cursor. </summary>
    public Mode CurrentMode { get; private set; }

    /// <summary> Sets the mode to Mode.Home </summary>
    public void SetModeToHome()
      => CurrentMode = Mode.Home;

    /// <summary> Sets the mode to Mode.End </summary>
    public void SetModeToEnd()
      => CurrentMode = Mode.End;

    /// <summary> Sets mode to Mode.None </summary>
    public void SetModeToNone()
      => CurrentMode = Mode.None;

    /// <summary>
    ///  Sets the mode to Mode.Position and sets the <see cref="Position"/> to the
    ///  given value.
    /// </summary>
    /// <param name="position"> The position of where the cursor should move
    ///  towards when navigating up or down. </param>
    public void SetModeToPosition(double position)
    {
      CurrentMode = Mode.Position;
      Position = position;
    }

    public enum Mode
    {
      None,
      Position,
      Home,
      End,
    }
  }
}