using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TextRight.ContentEditor.Core.Editing.Actions
{
  public class ActionStack
  {
    private readonly DocumentEditorContext _context;
    private readonly Stack<IAction> _actions;

    public ActionStack(DocumentEditorContext context)
    {
      _context = context;
      _actions = new Stack<IAction>();
    }

    public void Do(IAction action)
    {
      action.Do(_context);
      _actions.Push(action);
    }

    public void Undo()
    {
      if (_actions.Count == 0)
        return;

      var action = _actions.Pop();
      action.Undo(_context);
    }
  }
}