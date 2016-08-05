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
    private readonly IActionStackMergePolicy _mergePolicy;
    private readonly Stack<UndoStackEntry> _toUndoStack;
    private readonly Stack<UndoStackEntry> _toRedoStack;

    /// <summary> Constructor. </summary>
    /// <param name="context"> The editor context for which this stack was created. </param>
    /// <param name="mergePolicy"> Strategy class for determining when actions should be merged </param>
    public ActionStack(DocumentEditorContext context, IActionStackMergePolicy mergePolicy)
    {
      _context = context;
      _mergePolicy = mergePolicy;

      _toRedoStack = new Stack<UndoStackEntry>();
      _toUndoStack = new Stack<UndoStackEntry>();
    }

    /// <summary> The number of items that can be undone. </summary>
    public int UndoStackSize
      => _toUndoStack.Count;

    /// <summary> Performs the given action and adds it to the undoable stack. </summary>
    /// <param name="undoableAction"> The undoable action. </param>
    public void Do(UndoableAction undoableAction)
    {
      bool wasMerged = false;

      var newestEntry = new UndoStackEntry(undoableAction, DateTimeOffset.UtcNow);

      if (_toUndoStack.Count > 0)
      {
        var lastEntry = _toUndoStack.Peek();

        if (_mergePolicy.ShouldTryMerge(lastEntry, newestEntry))
        {
          wasMerged = lastEntry.Action.TryMerge(_context, undoableAction);
        }
      }

      undoableAction.Do(_context);

      if (!wasMerged)
      {
        // it was merged into the previous command, so no need to add it to the stack
        _toUndoStack.Push(newestEntry);
      }

      _toRedoStack.Clear();
    }

    /// <summary> Undoes the last undoable action </summary>
    public void Undo()
    {
      if (_toUndoStack.Count == 0)
        return;

      var lastEntry = _toUndoStack.Pop();
      lastEntry.Action.Undo(_context);
      _toRedoStack.Push(lastEntry);
    }

    /// <summary> Re-executes the last undoable action that was undone. </summary>
    public void Redo()
    {
      if (_toRedoStack.Count == 0)
        return;

      var lastUndoneEntry = _toRedoStack.Pop();
      lastUndoneEntry.Action.Do(_context);
      _toUndoStack.Push(lastUndoneEntry);
    }

    /// <summary> Clears the undo stack, disallowing undo/redo of past actions. </summary>
    public void Clear()
    {
      _toUndoStack.Clear();
      _toRedoStack.Clear();
    }

    /// <summary>  </summary>
    public struct UndoStackEntry
    {
      public UndoStackEntry(UndoableAction action, DateTimeOffset insertTime)
      {
        Action = action;
        InsertTime = insertTime;
      }

      public UndoableAction Action { get; }
      public DateTimeOffset InsertTime { get; }
    }

  }
}