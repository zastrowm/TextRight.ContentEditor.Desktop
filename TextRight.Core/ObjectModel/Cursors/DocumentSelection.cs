using System;
using System.Collections.Generic;
using System.Linq;
using TextRight.Core.Editing;
using TextRight.Core.ObjectModel.Blocks;

namespace TextRight.Core.ObjectModel.Cursors
{
  /// <summary>
  ///  Represents a selection within the document, holding the start position of the selection and
  ///  the end position of the selection.
  /// </summary>
  public class DocumentSelection
  {
    private readonly DocumentEditorContext _context;
    private readonly DocumentCursor _end;

    public DocumentSelection(DocumentEditorContext context)
    {
      _context = context;

      var firstBlock = (ContentBlock)context.Document.Root.FirstBlock;

      // TODO get cursors in a way that doesn't require assumptions
      Start = new DocumentCursor(context.Document, firstBlock.CreateCursor().ToBeginning());
      _end = new DocumentCursor(context.Document, firstBlock.CreateCursor().ToBeginning());

      IsActive = true;
    }

    /// <summary> True if there is an active selection in the document, false otherwise. </summary>
    public bool IsActive { get; set; }

    /// <summary> The start of the selection. </summary>
    public DocumentCursor Start { get; }

    /// <summary> The end of the selection </summary>
    public DocumentCursor End
      => IsActive ? _end : null;
  }
}