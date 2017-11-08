using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using TextRight.Core.Actions;
using TextRight.Core.Cursors;
using TextRight.Core.ObjectModel;
using TextRight.Core.ObjectModel.Blocks;
using TextRight.Core.ObjectModel.Cursors;

namespace TextRight.Core
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

      var cursor = ((ContentBlock)Document.Root.FirstBlock).GetCaretAtStart();

      Selection = new DocumentSelection(Document, cursor);
      CaretMovementMode = new CaretMovementMode();

      UndoStack = new ActionStack(this, new StandardMergePolicy());
    }

    /// <summary> The document that is being edited. </summary>
    public DocumentOwner Document { get; }

    /// <summary> The Caret's current position. </summary>
    public DocumentSelection Selection { get; }

    /// <summary> The position of the current caret. </summary>
    public BlockCaret Caret
    {
      get => Selection.Start;
      set => Selection.MoveTo(value);
    }

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
      get => Selection.ShouldExtendSelection;
      set => Selection.ShouldExtendSelection = value;
    }

    /// <summary> The stack of actions that can be undone. </summary>
    public ActionStack UndoStack { get; }
  }
}