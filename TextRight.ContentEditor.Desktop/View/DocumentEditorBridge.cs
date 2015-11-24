﻿using System;
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
    public DocumentCursor Cursor { get; }

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
      Cursor = new DocumentCursor(_document, cursor);
    }

    public void HandleKeyDown(Key key)
    {
      switch (key)
      {
        case Key.Left:
          Cursor.MoveBackwardInBlock();
          break;
        case Key.Right:
          Cursor.MoveForwardInBlock();
          break;
      }
    }

    public void InsertText(string text)
    {
      Cursor.InsertText(text);
    }
  }
}