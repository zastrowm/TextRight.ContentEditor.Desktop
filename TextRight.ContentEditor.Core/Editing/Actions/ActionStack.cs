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
    private readonly Stack<UndoableAction> _toUndoStack;
    private readonly Stack<UndoableAction> _toRedoStack;

    /// <summary> Constructor. </summary>
    /// <param name="context"> The editor context for which this stack was created. </param>
    public ActionStack(DocumentEditorContext context)
    {
      _context = context;

      _toRedoStack = new Stack<UndoableAction>();
      _toUndoStack = new Stack<UndoableAction>();
    }

    /// <summary> The number of items that can be undone. </summary>
    public int UndoStackSize
      => _toUndoStack.Count;

    /// <summary> Performs the given action and adds it to the undoable stack. </summary>
    /// <param name="undoableAction"> The undoable action. </param>
    /// <param name="allowMerging"> (Optional) True if the action is allowed to be merged into the
    ///  previous action, false otherwise. </param>
    public void Do(UndoableAction undoableAction, bool allowMerging = false)
    {
      bool wasMerged = false;

      if (allowMerging && _toUndoStack.Count > 0)
      {
        var last = _toUndoStack.Peek();
        wasMerged = last.TryMerge(_context, undoableAction);
      }

      undoableAction.Do(_context);

      if (!wasMerged)
      {
        // it was merged into the previous command, so no need to add it to the stack
        _toUndoStack.Push(undoableAction);
      }

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