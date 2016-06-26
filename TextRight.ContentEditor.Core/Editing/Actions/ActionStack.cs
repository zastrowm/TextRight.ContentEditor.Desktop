using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TextRight.ContentEditor.Core.Editing.Actions
{
  /// <summary> Holds a set of actions that can be undone/redone. </summary>
  public sealed class ActionStack
  {
    private readonly DocumentEditorContext _context;
    private readonly Stack<IUndoableAction> _toUndoStack;
    private readonly Stack<IUndoableAction> _toRedoStack;

    /// <summary> Constructor. </summary>
    /// <param name="context"> The editor context for which this stack was created. </param>
    public ActionStack(DocumentEditorContext context)
    {
      _context = context;

      _toRedoStack = new Stack<IUndoableAction>();
      _toUndoStack = new Stack<IUndoableAction>();
    }

    /// <summary> Performs the given action and adds it to the undoable stack. </summary>
    /// <param name="undableAction"> The undoable action. </param>
    public void Do(IUndoableAction undableAction)
    {
      undableAction.Do(_context);
      _toUndoStack.Push(undableAction);
      _toRedoStack.Clear();
    }

    /// <summary> Undoes the last undoable action </summary>
    public void Undo()
    {
      if (_toUndoStack.Count == 0)
        return;

      var action = _toUndoStack.Pop();
      action.Undo(_context);
      _toRedoStack.Push(action);
    }

    /// <summary> Re-executes the last undoable action that was undone. </summary>
    public void Redo()
    {
      if (_toRedoStack.Count == 0)
        return;

      var action = _toRedoStack.Pop();
      action.Do(_context);
      _toUndoStack.Push(action);
    }
  }
}