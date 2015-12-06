using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Controls;
using TextRight.ContentEditor.Core.ObjectModel;

namespace TextRight.ContentEditor.Desktop.View
{
  /// <summary>
  /// Interaction logic for DocumentEditor.xaml
  /// </summary>
  public partial class DocumentEditor : UserControl
  {
    public DocumentEditor()
    {
      InitializeComponent();

      Content = new DocumentEditorContextView(new DocumentEditorContext());
    }
  }
}