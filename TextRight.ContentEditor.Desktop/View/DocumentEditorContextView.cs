using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
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
  public class DocumentEditorContextView : Canvas
  {
    private readonly FlowDocument _flowDocument;
    private readonly DocumentEditorContext _editor;
    private readonly CaretView _caretView;
    private readonly FlowDocumentScrollViewer _documentViewer;

    public DocumentCursor Cursor => _editor.Caret;

    public DocumentEditorContextView(DocumentEditorContext editor)
    {
      _editor = editor;

      _caretView = new CaretView(_editor.Caret);
      Children.Add(_caretView.Element);

      // clear out the existing content
      _flowDocument = new FlowDocument();
      _flowDocument.Blocks.Add(new TextBlockView((TextBlock)_editor.Document.Root.FirstBlock));

      _documentViewer = new FlowDocumentScrollViewer()
                        {
                          Document = _flowDocument,
                        };

      Children.Add(_documentViewer);

      SetTop(_documentViewer, 0);
      SetLeft(_documentViewer, 0);
      SetZIndex(_documentViewer, 0);
    }

    protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
    {
      base.OnRenderSizeChanged(sizeInfo);

      _documentViewer.Width = sizeInfo.NewSize.Width;
      _documentViewer.Height = sizeInfo.NewSize.Height;
    }

    protected override void OnTextInput(TextCompositionEventArgs e)
    {
      base.OnTextInput(e);

      InsertText(e.Text);
      UpdateCaretPosition();
    }

    protected override void OnPreviewKeyDown(KeyEventArgs e)
    {
      base.OnPreviewKeyDown(e);

      HandleKeyDown(e.Key);
      UpdateCaretPosition();
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