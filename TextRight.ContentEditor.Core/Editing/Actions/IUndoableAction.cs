using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TextRight.ContentEditor.Core.Editing.Actions
{
  /// <summary> Interface for action that can be undone. </summary>
  public interface IUndoableAction
  {
    /// <summary> A human-readable name of the action. </summary>
    string Name { get; }

    /// <summary> A human-readable description of the action. </summary>
    string Description { get; }

    /// <summary> Performs the given operation. </summary>
    /// <param name="context"> The context in which the action should be performed. </param>
    void Do(DocumentEditorContext context);

    /// <summary> Undoes any effects that occurred from calling <see cref="Do"/>. </summary>
    /// <param name="context"> The context in which the action should be performed. </param>
    void Undo(DocumentEditorContext context);
  }
}