using System;
using System.Collections.Generic;
using System.Linq;
using TextRight.ContentEditor.Core.Editing;
using TextRight.ContentEditor.Core.ObjectModel.Blocks;

namespace TextRight.ContentEditor.Core.ObjectModel.Cursors
{
  /// <summary> View for the DocumentSelection. </summary>
  public interface IDocumentSelectionView
  {
    /// <summary> Notifies the view that the selection has been changed. </summary>
    void NotifyChanged();
  }

  /// <summary>
  ///  Represents a selection within the document, holding the start position of the selection and
  ///  the end position of the selection.
  /// </summary>
  public class DocumentSelection : IViewableObject<IDocumentSelectionView>
  {
    private readonly DocumentEditorContext _context;
    private readonly DocumentCursor _end;

    public DocumentSelection(DocumentEditorContext context)
    {
      _context = context;

      Start = new DocumentCursor(context.Document, context.Document.Root.FirstBlock.CreateCursor().ToBeginning());
      _end = new DocumentCursor(context.Document, context.Document.Root.FirstBlock.CreateCursor().ToBeginning());

      IsActive = true;
    }

    /// <summary> True if there is an active selection in the document, false otherwise. </summary>
    public bool IsActive { get; set; }

    /// <summary> The start of the selection. </summary>
    public DocumentCursor Start { get; }

    /// <summary> The end of the selection </summary>
    public DocumentCursor End
      => IsActive ? _end : null;

    /// <inheritdoc />
    public IDocumentSelectionView Target { get; set; }
  }
}