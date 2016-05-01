﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TextRight.ContentEditor.Core.Editing.Actions;

namespace TextRight.ContentEditor.Core.Editing
{
  /// <summary>
  ///  An command that can be executed when certain conditions are present.  The basis for all
  ///  commands that can be committed by the user.   For example, a TextBlock can be broken/split
  ///  into two different paragraphs; this would be an command that would only be enabled when the
  ///  caret is in the middle of a text-block.
  /// </summary>
  public interface IContextualCommand
  {
    /// <summary> Gets the name of the command as it should be presented to the user. </summary>
    /// <param name="context"> The context in which it's presented to the user. </param>
    string GetName(DocumentEditorContext context);

    /// <summary> Gets the description of the command as it should be presented to the user. </summary>
    /// <param name="context"> The context in which it's presented to the user. </param>
    string GetDescription(DocumentEditorContext context);

    /// <summary> True if the command can be executed for the given context. </summary>
    /// <param name="context"> The context in which the action should be executed. </param>
    /// <returns> True if the command can be executed, command otherwise. </returns>
    bool CanActivate(DocumentEditorContext context);

    /// <summary>
    ///  Executes the command in the given context.  This call must be guarded by a call to
    ///  CanActivate.
    /// </summary>
    /// <param name="context"> The context in which the command should be executed. </param>
    /// <param name="actionStack"> Stack of undo-able actions. </param>
    void Activate(DocumentEditorContext context, ActionStack actionStack);
  }
}