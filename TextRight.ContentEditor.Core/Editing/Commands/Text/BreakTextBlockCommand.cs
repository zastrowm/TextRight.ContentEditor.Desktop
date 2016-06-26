using System;
using System.Collections.Generic;
using System.Linq;
using TextRight.ContentEditor.Core.Editing.Actions;
using TextRight.ContentEditor.Core.ObjectModel.Blocks;

namespace TextRight.ContentEditor.Core.Editing
{
  /// <summary> Breaks a text-block into two. </summary>
  public class BreakTextBlockCommand : IContextualCommand
  {
    /// <inheritdoc />
    string IContextualCommand.GetName(DocumentEditorContext context)
    {
      return "Split Paragraph";
    }

    /// <inheritdoc />
    string IContextualCommand.GetDescription(DocumentEditorContext context)
    {
      return "Split block into two";
    }

    /// <inheritdoc />
    bool IContextualCommand.CanActivate(DocumentEditorContext context)
    {
      var textBlock = context.Cursor.Block as TextBlock;
      // TODO check if the parent collection allows multiple children
      return textBlock != null;
    }

    /// <inheritdoc />
    void IContextualCommand.Activate(DocumentEditorContext context, ActionStack actionStack)
    {
      // TODO delete any text that is selected
      actionStack.Do(new BreakTextBlockAction(context.Caret));
    }
  }
}