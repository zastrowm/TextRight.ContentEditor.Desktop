using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using TextRight.ContentEditor.Desktop.Blocks;

namespace TextRight.ContentEditor.Desktop.View
{
  /// <summary>
  /// Interaction logic for DocumentEditor.xaml
  /// </summary>
  public partial class DocumentEditor : UserControl
  {
    private readonly DocumentOwner _document = new DocumentOwner();
    private readonly DocumentEditorBridge _bridge;

    public DocumentEditor()
    {
      InitializeComponent();

      _bridge = new DocumentEditorBridge(PrimaryDocument, _document);
    }

    protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
    {
      base.OnRenderSizeChanged(sizeInfo);

      Viewer.Width = sizeInfo.NewSize.Width;
      Viewer.Height = sizeInfo.NewSize.Height;
    }

    protected override void OnTextInput(TextCompositionEventArgs e)
    {
      base.OnTextInput(e);

      _bridge.InsertText(e.Text);

      UpdateCursorPosition();
    }

    protected override void OnPreviewKeyDown(KeyEventArgs e)
    {
      base.OnPreviewKeyDown(e);

      _bridge.HandleKeyDown(e.Key);

      UpdateCursorPosition();
    }

    /// <summary> Update the rectangle associated with the cursor. </summary>
    private void UpdateCursorPosition()
    {
      var cursor = _bridge.Cursor;

      // TODO do we need to cast it as a text cursor?  Should the block cursor
      // know how to measure itself? 
      var textCursor = (ObjectModel.Blocks.TextBlock.TextBlockCursor)cursor.BlockCursor;

      var block = (ObjectModel.Blocks.TextBlock)cursor.BlockCursor.Block;

      var measure = textCursor.Span.Measure(textCursor.OffsetIntoSpan);

      Canvas.SetLeft(CursorPosition, measure.X);
      Canvas.SetTop(CursorPosition, measure.Y);
    }
  }
}