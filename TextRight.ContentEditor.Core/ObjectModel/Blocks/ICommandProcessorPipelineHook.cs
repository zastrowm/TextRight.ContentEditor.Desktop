using System;
using System.Collections.Generic;
using System.Linq;
using TextRight.ContentEditor.Core.Editing;
using TextRight.ContentEditor.Core.Editing.Commands;

namespace TextRight.ContentEditor.Core.ObjectModel.Blocks
{
  /// <summary>
  ///  Hooks into the execution of commands, allowing an action to be performed
  ///  before the command is executed and afterwards.
  /// </summary>
  public interface ICommandProcessorPipelineHook
  {
    /// <summary> Invoked prior to the command being executed. </summary>
    void PreProcess(EditorCommand command, DocumentEditorContext context);

    /// <summary> Invoked after the command is executed. </summary>
    void PostProcess(EditorCommand command, DocumentEditorContext context);
  }
}