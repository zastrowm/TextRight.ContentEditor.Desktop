using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using TextRight.ContentEditor.Core.ObjectModel;
using TextRight.ContentEditor.Desktop.Blocks;
using TextRight.ContentEditor.Desktop.ObjectModel.Blocks;
using TextBlock = TextRight.ContentEditor.Desktop.ObjectModel.Blocks.TextBlock;

namespace TextRight.ContentEditor.Desktop.View
{
  /// <summary>
  ///  Creates an editor from a FlowDocument and associated TextRight Document.
  /// </summary>
  public class DocumentEditorContextView
  {
    private readonly Canvas _canvas;
    private readonly FlowDocument _flowDocument;
    private readonly DocumentEditorContext _editor;
    private readonly CaretView _caretView;

    public DocumentCursor Cursor => _editor.Caret;

    public DocumentEditorContextView(Canvas canvas, FlowDocument flowDocument, DocumentEditorContext editor)
    {
      _canvas = canvas;
      _flowDocument = flowDocument;
      _editor = editor;

      _caretView = new CaretView(_editor.Caret);
      _canvas.Children.Add(_caretView.Element);

      // clear out the existing content
      flowDocument.Blocks.Clear();

      flowDocument.Blocks.Add(new TextBlockView((TextBlock)_editor.Document.Root.FirstBlock));
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
      var textCursor = Cursor.BlockCursor as ITextContentCursor;
      if (textCursor?.CanInsertText() != true)
        return;

      textCursor.InsertText(text);
    }

    public void UpdateCaretPosition()
    {
      _caretView.SyncPosition();
    }
  }
}