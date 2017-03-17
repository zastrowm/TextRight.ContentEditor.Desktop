using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TextRight.Core.Editing.Actions;

namespace TextRight.Core.Editing.Commands
{
  /// <summary>
  ///  An command that can be executed when certain conditions are present.  The basis for all
  ///  commands that can be committed by the user.   For example, a TextBlock can be broken/split
  ///  into two different paragraphs; this would be an command that would only be enabled when the
  ///  caret is in the middle of a text-block.
  /// </summary>
  /// <remarks>
  ///  Most <see cref="IContextualCommand"/>s result in one or more <see cref="UndoableAction"/>
  ///  being executed on a document.
  /// </remarks>
  public interface IContextualCommand
  {
    /// <summary> The unique, human-readable id of the command. </summary>
    string Id { get; }

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
    void Activate(DocumentEditorContext context, IActionStack actionStack);
  }
}