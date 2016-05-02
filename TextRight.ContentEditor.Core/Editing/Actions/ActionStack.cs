using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TextRight.ContentEditor.Core.Editing.Actions
{
  public class ActionStack
  {
    private readonly DocumentEditorContext _context;
    private readonly Stack<IUndoableAction> _toUndoStack;
    private readonly Stack<IUndoableAction> _toRedoStack;

    public ActionStack(DocumentEditorContext context)
    {
      _context = context;

      _toRedoStack = new Stack<IUndoableAction>();
      _toUndoStack = new Stack<IUndoableAction>();
    }

    public void Do(IUndoableAction undableAction)
    {
      undableAction.Do(_context);
      _toUndoStack.Push(undableAction);
      _toRedoStack.Clear();
    }

    public void Undo()
    {
      if (_toUndoStack.Count == 0)
        return;

      var action = _toUndoStack.Pop();
      action.Undo(_context);
      _toRedoStack.Push(action);
    }

    public void Redo()
    {
      if (_toRedoStack.Count == 0)
        return;

      var action = _toRedoStack.Pop();
      action.Do(_context);
      _toUndoStack.Push(action);
    }

    private class Node
    {
      public IUndoableAction Action;
      public Node Next;
      public Node Previous;
    }
  }
}