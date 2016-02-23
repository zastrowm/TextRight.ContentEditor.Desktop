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

      CommandPipeline = new CommandPipeline(this);
    }

    /// <summary> The document that is being edited. </summary>
    public DocumentOwner Document { get; }

    /// <summary> The Caret's current position. </summary>
    public DocumentCursor Caret { get; }

    /// <summary> TODO </summary>
    public IBlockContentCursor Cursor
      => Caret.BlockCursor;

    public CommandPipeline CommandPipeline { get; }

    /// <summary> Movement information about the caret. </summary>
    public CaretMovementMode CaretMovementMode { get; }

    /// <summary> Break the current block at the current position. </summary>
    internal void BreakCurrentBlock()
    {
      var currentBlock = Cursor.Block;
      var parentBlock = currentBlock.Parent;

      if (parentBlock.CanBreak(Cursor))
      {
        var newBlock = parentBlock.Break(Cursor);
        if (newBlock != null)
        {
          var cursor = newBlock.GetCursor();
          cursor.MoveToBeginning();
          Caret.MoveTo(cursor);
        }
      }
    }
  }
}