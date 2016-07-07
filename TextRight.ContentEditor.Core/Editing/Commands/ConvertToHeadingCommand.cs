using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TextRight.ContentEditor.Core.Editing.Actions;
using TextRight.ContentEditor.Core.ObjectModel.Blocks;

namespace TextRight.ContentEditor.Core.Editing.Commands
{
  public class ConvertToHeadingCommand : IContextualCommand
  {
    public string GetName(DocumentEditorContext context)
    {
      throw new NotImplementedException();
    }

    public string GetDescription(DocumentEditorContext context)
    {
      throw new NotImplementedException();
    }

    public bool CanActivate(DocumentEditorContext context)
    {
      return context.Cursor.Block is ParagraphBlock;
    }

    public void Activate(DocumentEditorContext context, ActionStack actionStack)
    {
      actionStack.Do(new ConvertParagraphIntoHeadingAction(context.Cursor));
    }
  }
}
