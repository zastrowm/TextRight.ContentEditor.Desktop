using System;
using System.Collections.Generic;
using System.Linq;
using TextRight.ContentEditor.Core.Editing.Commands;

namespace TextRight.ContentEditor.Core.ObjectModel.Blocks
{
  /// <summary> Interface for an object that hooks into command processing for an object.. </summary>
  public interface ICommandProcessorHook
  {
    /// <summary> Attempts to process incoming commands. </summary>
    ICommandProcessor CommandProcessor { get; }
  }
}