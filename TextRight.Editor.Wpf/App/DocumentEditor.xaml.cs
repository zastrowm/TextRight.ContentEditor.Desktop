using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Controls;
using TextRight.ContentEditor.Core.Editing;
using TextRight.ContentEditor.Desktop.View;

namespace TextRight.ContentEditor.Desktop.App
{
  /// <summary>
  /// Interaction logic for DocumentEditor.xaml
  /// </summary>
  public partial class DocumentEditor : UserControl
  {
    public DocumentEditor()
    {
      Loaded += delegate
                {
                  var view = ((DocumentEditorContextView)Content);
                  view.Initialize();
                  view.Focus();
                };

      Content = new DocumentEditorContextView(new DocumentEditorContext());
    }
  }
}