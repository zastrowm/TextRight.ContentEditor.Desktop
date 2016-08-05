using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using TextRight.ContentEditor.Core.Editing;
using TextRight.ContentEditor.Core.Editing.Actions;

namespace TextRight.ContentEditor.Core.Tests.Editing
{
  public class UndoIntegrationTest : UndoBasedTest
  {
    [Test]
    public void Undo_RestoresInitalState_AnywhereInParagraphs()
    {
      for (int i = 0; i < 10; i++)
      {
        Console.WriteLine("Increment: {0}", i);
        DoAllAndThenUndo(new Func<UndoableAction>[]
                         {
                           // Block 1
                           () => new InsertTextUndoableAction(BlockAt(0).EndCursor().ToHandle(), "012345679"),
                           () => new BreakTextBlockAction(BlockAt(0).BeginCursor(i).ToHandle()),
                           // Block 2
                           () => new InsertTextUndoableAction(BlockAt(1).BeginCursor().ToHandle(), "012345679"),
                           () => new BreakTextBlockAction(BlockAt(0).BeginCursor(i).ToHandle()),
                           // Block 3
                           () => new InsertTextUndoableAction(BlockAt(1).EndCursor().ToHandle(), "012345679"),
                           // Block 2 (even though there is a third block
                           () => new InsertTextUndoableAction(BlockAt(1).BeginCursor().ToHandle(), "012345679"),
                           () => new BreakTextBlockAction(BlockAt(0).BeginCursor(i).ToHandle()),
                         });
      }
    }
  }
}