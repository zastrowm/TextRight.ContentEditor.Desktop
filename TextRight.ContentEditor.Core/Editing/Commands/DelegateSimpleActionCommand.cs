using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TextRight.ContentEditor.Core.Editing.Commands
{
  /// <summary> An action command that takes in a delegate to perform the command. </summary>
  internal class DelegateSimpleActionCommand : ISimpleActionCommand
  {
    private readonly Action<DocumentEditorContext> _callback;

    /// <summary> Constructor. </summary>
    /// <param name="id"> The unique id of the command. </param>
    /// <param name="callback"> The callback. </param>
    public DelegateSimpleActionCommand(string id, Action<DocumentEditorContext> callback)
    {
      Id = id;
      _callback = callback;
    }

    /// <inheritdoc />
    public string Id { get; }

    /// <inheritdoc />
    public void Execute(DocumentEditorContext context)
    {
      _callback.Invoke(context);
    }
  }
}