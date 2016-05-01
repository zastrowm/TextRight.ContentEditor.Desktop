using System;
using System.Collections.Generic;
using System.Linq;
using TextRight.ContentEditor.Core.Editing.Actions;
using TextRight.ContentEditor.Core.ObjectModel.Blocks;

namespace TextRight.ContentEditor.Core.Editing
{
  /// <summary> Breaks a text-block into two. </summary>
  public class BreakTextBlockAction : IContextualCommand
  {
    /// <inheritdoc />
    string IContextualCommand.GetName(DocumentEditorContext context)
    {
      return "Split Paragraph";
    }

    /// <inheritdoc />
    string IContextualCommand.GetDescription(DocumentEditorContext context)
    {
      return "Split block into two";
    }

    /// <inheritdoc />
    bool IContextualCommand.CanActivate(DocumentEditorContext context)
    {
      var textBlock = context.Cursor.Block as TextBlock;
      // TODO check if the parent collection allows multiple children
      return textBlock != null;
    }

    /// <inheritdoc />
    void IContextualCommand.Activate(DocumentEditorContext context, ActionStack actionStack)
    {
      // TODO delete any text that is selected
      actionStack.Do(new UndoableAction(context.Caret));
    }

    /// <summary> Breaks a paragraph at the given caret location. </summary>
    private class UndoableAction : IUndoableAction
    {
      private readonly DocumentCursorHandle _handle;

      /// <summary> Constructor. </summary>
      /// <param name="handle"> The location at which the paragraph should be broken. </param>
      public UndoableAction(DocumentCursorHandle handle)
      {
        _handle = handle;
      }

      /// <inheritdoc />
      public string Name { get; }
        = "Break Paragraph";

      /// <inheritdoc />
      public string Description { get; }
        = "Break paragraph into two";

      /// <inheritdoc />
      public void Do(DocumentEditorContext context)
      {
        var blockCursor = _handle.Get(context);
        var blockCollection = blockCursor.Block.Parent;

        var newBlock = blockCollection.TryBreakBlock(context.Cursor);
        if (newBlock == null)
          return;

        context.Caret.MoveTo(newBlock.GetCursor().ToBeginning());
      }

      /// <inheritdoc />
      public void Undo(DocumentEditorContext context)
      {
        var blockCursor = _handle.Get(context);
        var previousBlock = blockCursor.Block;
        var nextBlock = previousBlock.NextBlock;

        nextBlock.Parent.MergeWithPrevious(nextBlock);

        // move it to where it was when we wanted to break the paragraph.  It's safer to deserialize
        // again, as the cursor above is not guaranteed to be valid. 
        context.Caret.MoveTo(_handle.Get(context));
      }
    }
  }
}