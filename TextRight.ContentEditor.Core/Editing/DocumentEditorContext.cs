using System;
using System.Collections.Generic;
using System.Linq;
using TextRight.ContentEditor.Core.Editing.Commands;
using TextRight.ContentEditor.Core.ObjectModel;
using TextRight.ContentEditor.Core.ObjectModel.Blocks;

namespace TextRight.ContentEditor.Core.Editing
{
  /// <summary> Represents a TextRight document that is being edited. </summary>
  public class DocumentEditorContext
  {
    /// <summary> Default constructor. </summary>
    public DocumentEditorContext()
    {
      Document = new DocumentOwner();

      var cursor = Document.Root.FirstBlock.GetCursor();
      cursor.MoveToBeginning();

      Caret = new DocumentCursor(Document, cursor);
      CaretMovementMode = new CaretMovementMode();
    }

    /// <summary> The document that is being edited. </summary>
    public DocumentOwner Document { get; }

    /// <summary> The Caret's current position. </summary>
    public DocumentCursor Caret { get; }

    /// <summary> Movement information about the caret. </summary>
    public CaretMovementMode CaretMovementMode { get; }

    /// <summary> Removes the character after the cursor. </summary>
    private void DeleteNextCharacter()
    {
      var textCursor = Caret.BlockCursor as ITextContentCursor;
      if (textCursor == null)
        return;

      textCursor.DeleteText(1);
    }

    /// <summary> Removes the character before the cursor. </summary>
    private void DeletePreviousCharacter()
    {
      Caret.MoveBackward();
      DeleteNextCharacter();
    }

    /// <summary> Executes the given command. </summary>
    /// <typeparam name="T"> Generic type parameter. </typeparam>
    /// <param name="command"> The command to execute. </param>
    public void Execute<T>(T command)
      where T : ISimpleActionCommand
    {
      command.Execute(this);

      var caretCommand = command as ICaretCommand;
      if (caretCommand != null && !caretCommand.ShouldPreserveCaretState)
      {
        CaretMovementMode.SetModeToNone();
      }
    }

    /// <summary> Break the current block at the current position. </summary>
    private void BreakCurrentBlock()
    {
      var currentBlock = Caret.BlockCursor.Block;
      var parentBlock = currentBlock.Parent;

      if (parentBlock.CanBreak(Caret))
      {
        var newBlock = parentBlock.Break(Caret);
        if (newBlock != null)
        {
          var cursor = newBlock.GetCursor();
          cursor.MoveToBeginning();
          Caret.MoveTo(cursor);
        }
      }
    }

    /// <summary> Commands available for operating on the DocumentEditorContext. </summary>
    public static class Commands
    {
      public static ISimpleActionCommand DeleteNextCharacter { get; }
        = new DelegateSimpleActionCommand("Caret.DeleteNext", e => e.DeleteNextCharacter());

      public static ISimpleActionCommand DeletePreviousCharacter { get; }
        = new DelegateSimpleActionCommand("Caret.DeleteNext", e => e.DeletePreviousCharacter());

      public static ISimpleActionCommand BreakBlock { get; }
        = new DelegateSimpleActionCommand("Block.Break", e => e.BreakCurrentBlock());

      public static ISimpleActionCommand MoveCursorForward { get; }
        = new SimpleCaretActionCommand("Caret.MoveForward", e => e.Caret.MoveForward(),
                                       shouldPreserveCaretState: false);

      public static ISimpleActionCommand MoveCursorBackward { get; }
        = new SimpleCaretActionCommand("Caret.MoveBackward", e => e.Caret.MoveBackward(),
                                       shouldPreserveCaretState: false);

      public static ISimpleActionCommand MoveToNextWord { get; }
        = new SimpleCaretActionCommand("Caret.MoveForwardWord",
                                       CaretWordMover.MoveCaretToBeginningOfNextWord,
                                       shouldPreserveCaretState: false);

      public static ISimpleActionCommand MoveToPreviousWord { get; }
        = new SimpleCaretActionCommand("Caret.MoveBackwardWord", CaretWordMover.MoveCaretToEndOfPreviousWord,
                                       shouldPreserveCaretState: false);

      public static ISimpleActionCommand MoveToBeginningOfLine { get; }
        = new SimpleCaretActionCommand("Caret.MoveToLineBeginning",
                                       c => CaretDirectionalMover.MoveCaretToBeginningOfLine(c),
                                       shouldPreserveCaretState: false);

      public static ISimpleActionCommand MoveToEndOfLine { get; }
        = new SimpleCaretActionCommand("Caret.MoveToLineEnd",
                                       c => CaretDirectionalMover.MoveCaretToEndOfLine(c),
                                       shouldPreserveCaretState: true);

      public static ISimpleActionCommand MoveUp { get; }
        = new SimpleCaretActionCommand("Caret.MoveUp",
                                       c => CaretDirectionalMover.MoveCaretUpInDocument(c),
                                       shouldPreserveCaretState: true);

      public static ISimpleActionCommand MoveDown { get; }
        = new SimpleCaretActionCommand("Caret.MoveUp",
                                       c => CaretDirectionalMover.MoveCaretDownInDocument(c),
                                       shouldPreserveCaretState: true);
    }
  }
}