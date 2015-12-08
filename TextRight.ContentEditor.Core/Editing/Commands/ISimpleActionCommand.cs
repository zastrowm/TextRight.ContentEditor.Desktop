using System;
using System.Collections.Generic;
using System.Linq;

namespace TextRight.ContentEditor.Desktop.Commands
{
  /// <summary> Interface for action command. </summary>
  public interface ISimpleActionCommand : IActionCommand
  {
    /// <summary> The unique id of the command. </summary>
    string Id { get; }
  }
}