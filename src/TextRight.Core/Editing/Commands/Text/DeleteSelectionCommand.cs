using System;
using System.Collections.Generic;
using System.Linq;
using TextRight.Core.Actions;

namespace TextRight.Core.Commands.Text
{
  public class DeleteSelectionCommand : IContextualCommand
  {
    /// <inheritdoc />
    public string Id
      => "text.deleteSection";

    /// <inheritdoc />
    public string GetName(DocumentEditorContext context)
      => "Delete Selection";

    /// <inheritdoc />
    public string GetDescription(DocumentEditorContext context)
      => "Deletes the current selected text";

    public bool CanActivate(DocumentEditorContext context)
      => context.Selection.HasSelection;

    public void Activate(DocumentEditorContext context, IActionStack actionStack)
    {
      throw new NotImplementedException();
    }
  }
}