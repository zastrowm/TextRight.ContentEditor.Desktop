using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using NFluent;
using TextRight.Core.Actions;
using TextRight.Core.Blocks;
using TextRight.Core.Editing.Actions;
using TextRight.Core.Editing.Commands.Text;
using Xunit;

namespace TextRight.Core.Tests.Editing
{
  public class ConvertParagraphIntoHeadingActionTests : UndoBasedTest
  {
    public override IReadOnlyList<Func<UndoableAction>> InitializeDocument()
    {
      return new Func<UndoableAction>[]
             {
               FromCommand<InsertTextCommand, string>(() => Context.Caret, "This is the text"),
             };
    }

    [Fact]
    public void Action_Works()
    {
      var it = DoAll(new Func<UndoableAction>[]
                     {
                       () => new ConvertTextBlockIntoHeadingAction(Context.Caret.Cursor, 0),
                     });

      var heading = Document.Root.FirstBlock.As<HeadingBlock>();
      Check.That(heading).IsNotNull();
      Check.That(heading.HeadingLevel).IsEqualTo(0);
      Check.That(heading.AsText()).IsEqualTo("This is the text");

      it.VerifyUndo();
    }

    [Fact]
    public void ExecutingOnHeading_SimplyChangesLevel()
    {
      DoAll(new Func<UndoableAction>[]
            {
              () => new ConvertTextBlockIntoHeadingAction(Context.Caret.Cursor, 0),
            });

      var heading = Document.Root.FirstBlock.As<HeadingBlock>();

      DoAll(new Func<UndoableAction>[]
            {
              () => new ConvertTextBlockIntoHeadingAction(Context.Caret.Cursor, 3),
            });

      Check.That(heading).IsSameReferenceThan(Document.Root.FirstBlock);
      Check.That(heading.HeadingLevel).IsEqualTo(3);
    }

    [Fact]
    public void ExecutingOnHeading_IsUndoable()
    {
      var it = DoAll(new Func<UndoableAction>[]
                     {
                       () => new ConvertTextBlockIntoHeadingAction(Context.Caret.Cursor, 0),
                       () => new ConvertTextBlockIntoHeadingAction(Context.Caret.Cursor, 3),
                     });

      it.VerifyUndo();
    }
  }
}