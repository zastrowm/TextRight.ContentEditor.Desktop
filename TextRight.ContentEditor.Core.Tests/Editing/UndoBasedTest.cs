using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using NUnit.Framework;
using TextRight.ContentEditor.Core.Editing;
using TextRight.ContentEditor.Core.Editing.Actions;
using TextRight.ContentEditor.Core.ObjectModel;
using TextRight.ContentEditor.Core.ObjectModel.Blocks;
using TextRight.ContentEditor.Core.ObjectModel.Serialization;

namespace TextRight.ContentEditor.Core.Tests.Editing
{
  /// <summary>
  ///  Sets up a test framework for verifying that an undo action leaves the document in the same
  ///  state as before the undo being performed.
  /// </summary>
  public class UndoBasedTest
  {
    protected DocumentEditorContext Context;

    protected DocumentOwner Document
      => Context.Document;

    [SetUp]
    public void BaseSetup()
    {
      Reinitialize();
    }

    /// <summary> Prepares the document for performing actions. </summary>
    private void Reinitialize()
    {
      Context = new DocumentEditorContext();

      var toExecute = InitializeDocument();
      if (toExecute != null)
      {
        foreach (var item in toExecute)
        {
          item.Invoke().Do(Context);
        }
      }
    }

    /// <summary>
    ///  Takes a serious of actions that should be performed before any of the tests.
    /// </summary>
    public virtual IReadOnlyList<Func<UndoableAction>> InitializeDocument()
    {
      return null;
    }

    /// <summary> Gets the block at the specified index. </summary>
    public Block BlockAt(int index)
      => Context.Document.Root.NthBlock(index);

    /// <summary>
    ///  Performs all of the actions given, returning an <see cref="UndoTester"/> that can be used to
    ///  test the undo of the actions performed.
    /// </summary>
    public UndoTester DoAll(IReadOnlyList<Func<UndoableAction>> actions)
    {
      foreach (var action in actions)
      {
        action.Invoke().Do(Context);
      }

      return new UndoTester(this, actions);
    }

    /// <summary> Executes all and then verifies that undoing them is successful. </summary>
    public void DoAllAndThenUndo(IReadOnlyList<Func<UndoableAction>> actions)
    {
      DoAll(actions).VerifyUndo();
    }

    /// <summary>
    ///  Run through all of the actions in a loop from 1 to N, verifying that when N actions are
    ///  performed, and then undone, that the state of the document becomes what it was originally.
    /// </summary>
    private void PerformStepByStep(IReadOnlyList<Func<UndoableAction>> actions)
    {
      Reinitialize();

      for (int i = 1; i < actions.Count + 1; i++)
      {
        PerformSingleIteration(actions, i);
      }
    }

    /// <summary> Helper method for <see cref="PerformStepByStep"/>. </summary>
    private void PerformSingleIteration(IReadOnlyList<Func<UndoableAction>> actions, int count)
    {
      Console.WriteLine("Performing {0} actions", count);

      var undoStack = new ActionStack(Context);
      var documentStates = new Stack<DocumentOwner>();

      for (int i = 0; i < count; i++)
      {
        documentStates.Push(Document.Clone());
        undoStack.Do(actions[i].Invoke());
      }

      while (documentStates.Count > 0)
      {
        // after each undo, we should be back to where we originally were
        var originalState = documentStates.Pop();
        undoStack.Undo();
        CheckEqual(originalState, Document);
      }
    }

    /// <summary> Check that the two documents are equal. </summary>
    private static void CheckEqual(DocumentOwner originalState, DocumentOwner currentState)
    {
      var originalJson = JsonConvert.SerializeObject(originalState.SerializeAsNode(), Formatting.Indented);
      var newJson = JsonConvert.SerializeObject(currentState.SerializeAsNode(), Formatting.Indented);

      if (originalState.Equals(currentState))
      {
        Assert.That(newJson, Is.EqualTo(originalJson));
        return;
      }

      Console.WriteLine("===== Original State: =====");
      Console.WriteLine(originalJson);
      Console.WriteLine("===== Current State: =====");
      Console.WriteLine(newJson);

      FindDifference(currentState.SerializeAsNode(), originalState.SerializeAsNode(), "");
    }

    /// <summary> Finds the first difference in the two documents. </summary>
    private static void FindDifference(SerializeNode currentState, SerializeNode originalState, string path)
    {
      Assert.That(currentState.Type, Is.EqualTo(originalState.Type), path + ".Type");

      var type = currentState.Type.Name;
      path += "<" + type + ">";
      Assert.That(currentState.Data, Is.EqualTo(originalState.Data), path + ".Data");

      int max = Math.Min(currentState.Children.Count, originalState.Children.Count);

      for (int i = 0; i < max; i++)
      {
        FindDifference(currentState.Children[i], originalState.Children[i], path + ".[" + i + "]");
      }

      Assert.That(currentState.Children.Count, Is.EqualTo(originalState.Children.Count), path + ".Count");
    }

    /// <summary>
    ///  Holds a set of actions that should be executed to verify that undo is working correctly.
    /// </summary>
    public class UndoTester
    {
      private readonly UndoBasedTest _undoBasedTest;
      private readonly IReadOnlyList<Func<UndoableAction>> _actions;

      public UndoTester(UndoBasedTest undoBasedTest, IReadOnlyList<Func<UndoableAction>> actions)
      {
        _undoBasedTest = undoBasedTest;
        _actions = actions;
      }

      public void VerifyUndo()
      {
        _undoBasedTest.PerformStepByStep(_actions);
      }
    }
  }
}