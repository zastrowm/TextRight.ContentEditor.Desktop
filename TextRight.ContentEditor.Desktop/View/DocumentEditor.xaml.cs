using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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

    protected override void OnTextInput(TextCompositionEventArgs e)
    {
      base.OnTextInput(e);

      _bridge.InsertText(e.Text);
    }

    protected override void OnPreviewKeyDown(KeyEventArgs e)
    {
      base.OnPreviewKeyDown(e);

      _bridge.HandleKeyDown(e.Key);
    }
  }
}