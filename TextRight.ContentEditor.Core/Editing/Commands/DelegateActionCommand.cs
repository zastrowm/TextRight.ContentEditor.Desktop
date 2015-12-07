using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TextRight.ContentEditor.Core.ObjectModel;

namespace TextRight.ContentEditor.Desktop.Commands
{
  /// <summary> An action command that takes in a delegate to perform the command. </summary>
  internal class DelegateActionCommand : IActionCommand
  {
    private readonly Action<DocumentEditorContext> _callback;

    public DelegateActionCommand(string id, Action<DocumentEditorContext> callback)
    {
      Id = id;
      _callback = callback;
    }

    public string Id { get; }

    public void Execute(DocumentEditorContext context)
    {
      _callback.Invoke(context);
    }
  }
}