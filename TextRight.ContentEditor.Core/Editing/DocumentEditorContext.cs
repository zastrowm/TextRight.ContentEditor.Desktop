using System;
using System.Collections.Generic;
using System.Linq;
using TextRight.ContentEditor.Desktop.Blocks;
using TextRight.ContentEditor.Desktop.Commands;

namespace TextRight.ContentEditor.Core.ObjectModel
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
    }

    /// <summary> The document that is being edited. </summary>
    public DocumentOwner Document { get; }

    /// <summary> The Caret's current position. </summary>
    public DocumentCursor Caret { get; }

    /// <summary> Commands available for operating on the DocumentEditorContext. </summary>
    public static class Commands
    {
      /// <summary> A command which moves the cursor forward in the document. </summary>
      public static IActionCommand MoveCursorForward { get; }
        = new DelegateActionCommand("Caret.MoveForward", e => e.Caret.MoveForward());

      /// <summary> A command which moves the cursor backward in the document. </summary>
      public static IActionCommand MoveCursorBackward { get; }
        = new DelegateActionCommand("Caret.MoveBackward", e => e.Caret.MoveBackward());
    }
  }
}