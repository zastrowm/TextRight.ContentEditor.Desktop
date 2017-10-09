using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TextRight.Core.Editing.Actions;
using TextRight.Core.ObjectModel.Blocks.Text;

namespace TextRight.Core.Editing.Commands
{
  /// <summary> Converts a given TextBlock into a heading. </summary>
  public class ConvertParagraphCommand : IContextualCommand
  {
    /// <inheritdoc />
    public string Id
      => "paragraph.convertToParagraph";

    /// <inheritdoc/>
    public string GetName(DocumentEditorContext context)
      => $"Convert to paragraph.";

    /// <inheritdoc/>
    public string GetDescription(DocumentEditorContext context)
      => $"Converts the given block into a paragraph";

    /// <inheritdoc/>
    public bool CanActivate(DocumentEditorContext context)
    {
      var block = context.Cursor.Block;
      return (block is TextBlock) && !(block is ParagraphBlock);
    }

    /// <inheritdoc/>
    public void Activate(DocumentEditorContext context, IActionStack actionStack)
    {
      actionStack.Do(new ConvertIntoParagraphAction((TextCaret)context.Caret.Caret));
    }
  }
}