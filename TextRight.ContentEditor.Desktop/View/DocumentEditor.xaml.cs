using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using TextRight.ContentEditor.Core.ObjectModel;

namespace TextRight.ContentEditor.Desktop.View
{
  /// <summary>
  /// Interaction logic for DocumentEditor.xaml
  /// </summary>
  public partial class DocumentEditor : UserControl
  {
    private readonly DocumentEditorContextView _contextView;
    private readonly DocumentEditorContext _editor = new DocumentEditorContext();

    public DocumentEditor()
    {
      InitializeComponent();

      _contextView = new DocumentEditorContextView(Positioning, PrimaryDocument, _editor);
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

      _contextView.InsertText(e.Text);
      _contextView.UpdateCaretPosition();
    }

    protected override void OnPreviewKeyDown(KeyEventArgs e)
    {
      base.OnPreviewKeyDown(e);

      _contextView.HandleKeyDown(e.Key);
      _contextView.UpdateCaretPosition();
    }
  }
}