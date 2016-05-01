using System;
using System.Collections.Generic;
using System.Linq;
using TextRight.ContentEditor.Core.Editing.Actions;
using TextRight.ContentEditor.Core.ObjectModel.Blocks;

namespace TextRight.ContentEditor.Core.Editing
{
  /// <summary> Breaks a text-block into two. </summary>
  public class BreakTextBlockAction : IContextAction
  {
    /// <inheritdoc />
    string IContextAction.GetName(DocumentEditorContext context)
    {
      return "Split Paragraph";
    }

    /// <inheritdoc />
    string IContextAction.GetDescription(DocumentEditorContext context)
    {
      return "Split block into two";
    }

    /// <inheritdoc />
    bool IContextAction.CanActivate(DocumentEditorContext context)
    {
      var textBlock = context.Cursor.Block as TextBlock;
      // TODO check if the parent collection allows multiple children
      return textBlock != null;
    }

    /// <inheritdoc />
    void IContextAction.Activate(DocumentEditorContext context, ActionStack actionStack)
    {
      // TODO delete any text that is selected
      actionStack.Do(new BreakParagraphAction(context.Caret));
    }
  }
}