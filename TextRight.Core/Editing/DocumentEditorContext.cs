using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using TextRight.ContentEditor.Core.Editing.Actions;
using TextRight.ContentEditor.Core.ObjectModel;
using TextRight.ContentEditor.Core.ObjectModel.Blocks;
using TextRight.ContentEditor.Core.ObjectModel.Cursors;

namespace TextRight.ContentEditor.Core.Editing
{
  public interface IDocumentEditorView : IDocumentItemView
  {
    /// <summary> Gets the block closest to the given point. </summary>
    /// <param name="point"> The point at which the block should be queried. </param>
    /// <returns> The closes block to the given point. </returns>
    [NotNull]
    Block GetBlockFor(DocumentPoint point);
  }

  /// <summary> Represents a TextRight document that is being edited. </summary>
  public class DocumentEditorContext : IDocumentItem<IDocumentEditorView>
  {
    public DocumentEditorContext()
      : this(new DocumentOwner())
    {
    }

    /// <summary> Default constructor. </summary>
    public DocumentEditorContext(DocumentOwner owner)
    {
      Document = owner;

      var cursor = ((ContentBlock)Document.Root.FirstBlock).GetCursor();
      cursor.MoveToBeginning();

      Caret = new DocumentCursor(Document, cursor);
      CaretMovementMode = new CaretMovementMode();

      UndoStack = new ActionStack(this, new StandardMergePolicy());
    }

    /// <summary> The document that is being edited. </summary>
    public DocumentOwner Document { get; }

    /// <summary> The Caret's current position. </summary>
    public DocumentCursor Caret { get; }

    /// <summary> A readonly representation of a block cursor. </summary>
    public ReadonlyCursor Cursor
      => Caret.Cursor;

    /// <summary> Movement information about the caret. </summary>
    public CaretMovementMode CaretMovementMode { get; }

    /// <summary> The View that is currently attached to the item. </summary>
    public IDocumentEditorView Target { get; set; }

    /// <inheritdoc />
    IDocumentItemView IDocumentItem.DocumentItemView
      => Target;

    /// <summary> True if the current selection should be extended. </summary>
    public bool IsSelectionExtendActive
    {
      get { return Caret.ShouldExtendSelection; }
      set { Caret.ShouldExtendSelection = value; }
    }

    /// <summary> The stack of actions that can be undone. </summary>
    public ActionStack UndoStack { get; }

    public void HandleMouseDown(DocumentPoint point)
    {
      if (Target == null)
        return;

      var block = Target.GetBlockFor(point) as ContentBlock;
      if (block != null)
      {
        var newCursor = block.GetCursorFor(point);
        Caret.MoveTo(newCursor);
      }
    }
  }
}