using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Documents;
using System.Windows.Input;
using TextRight.ContentEditor.Desktop.Blocks;
using TextRight.ContentEditor.Desktop.ObjectModel.Blocks;

namespace TextRight.ContentEditor.Desktop.View
{
  /// <summary>
  ///  Creates an editor from a FlowDocument and associated TextRight Document.
  /// </summary>
  public class DocumentEditorBridge
  {
    private readonly FlowDocument _flowDocument;
    private readonly DocumentOwner _document;
    private readonly DocumentCursor _cursor;

    public DocumentEditorBridge(FlowDocument flowDocument, DocumentOwner document)
    {
      _flowDocument = flowDocument;
      _document = document;

      var paragraph = ((Paragraph)flowDocument.Blocks.FirstBlock);
      var firstRun = paragraph.Inlines.FirstInline;
      paragraph.Inlines.Remove(firstRun);

      paragraph.Inlines.Add(new TextSpanViewRun(((TextBlock)_document.Root.FirstBlock).First()));

      var cursor = _document.Root.FirstBlock.GetCursor();
      cursor.MoveToBeginning();
      _cursor = new DocumentCursor(_document, cursor);
    }

    public void HandleKeyDown(Key key)
    {
      switch (key)
      {
        case Key.Left:
          _cursor.MoveBackwardInBlock();
          break;
        case Key.Right:
          _cursor.MoveForwardInBlock();
          break;
      }
    }

    public void InsertText(string text)
    {
      _cursor.InsertText(text);
    }
  }
}