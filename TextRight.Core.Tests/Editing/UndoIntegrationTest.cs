using System;
using System.Collections.Generic;
using System.Linq;
using TextRight.Core.Editing.Actions;
using TextRight.Core.Editing.Commands.Text;
using Xunit;

namespace TextRight.Core.Tests.Editing
{
  public class UndoIntegrationTest : UndoBasedTest
  {
    [Fact]
    public void Undo_RestoresInitalState_AnywhereInParagraphs()
    {
      for (int i = 0; i < 10; i++)
      {
        Console.WriteLine("Increment: {0}", i);
        DoAllAndThenUndo(new Func<UndoableAction>[]
                         {
                           // Block 1
                           FromCommand<InsertTextCommand, string>(() => BlockAt(0).EndCursor().ToHandle(), "012345679"),
                           FromCommand<BreakTextBlockCommand>(() => BlockAt(0).BeginCursor(i).ToHandle()),
                           // Block 2
                           FromCommand<InsertTextCommand, string>(
                             () => BlockAt(1).BeginCursor().ToHandle(),
                             "012345679"),
                           FromCommand<BreakTextBlockCommand>(() => BlockAt(0).BeginCursor(i).ToHandle()),
                           // Block 3
                           FromCommand<InsertTextCommand, string>(() => BlockAt(1).EndCursor().ToHandle(), "012345679"),
                           // Block 2 (even though there is a third block
                           FromCommand<InsertTextCommand, string>(
                             () => BlockAt(1).BeginCursor().ToHandle(),
                             "012345679"),
                           FromCommand<BreakTextBlockCommand>(() => BlockAt(0).BeginCursor(i).ToHandle()),
                         });
      }
    }
  }
}