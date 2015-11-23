using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using TextRight.ContentEditor.Desktop.Blocks;
using TextRight.ContentEditor.Desktop.ObjectModel;
using TextRight.ContentEditor.Desktop.ObjectModel.Blocks;
using TextBlock = TextRight.ContentEditor.Desktop.ObjectModel.Blocks.TextBlock;

namespace TextRight.ContentEditor.Desktop.View
{
  /// <summary>
  /// Interaction logic for DocumentEditor.xaml
  /// </summary>
  public partial class DocumentEditor : UserControl
  {
    private readonly DocumentOwner _document = new DocumentOwner();

    public DocumentEditor()
    {
      InitializeComponent();

      var paragraph = ((Paragraph)PrimaryDocument.Blocks.FirstBlock);
      var firstRun = paragraph.Inlines.FirstInline;
      paragraph.Inlines.Remove(firstRun);

      paragraph.Inlines.Add(new TextSpanViewRun(((TextBlock)_document.Root.FirstBlock).First()));
    }

    protected override void OnTextInput(TextCompositionEventArgs e)
    {
      base.OnTextInput(e);

      ((TextBlock)_document.Root.FirstBlock).First().Text += e.Text;
    }

    /// <summary>
    ///  Associates a WPF Run with a TextSpan and keeps them in sync.
    /// </summary>
    public class TextSpanViewRun : Run, INotifee<TextSpan.ChangeType>
    {
      private readonly TextSpan _span;

      /// <summary> Constructor. </summary>
      /// <param name="span"> The span to keep synchronized. </param>
      public TextSpanViewRun(TextSpan span)
      {
        _span = span;
        _span.AssociatedNotifiee = this;
      }

      public void MarkChange(DocumentElement<TextSpan.ChangeType> element, TextSpan.ChangeType changeType)
      {
        switch (changeType)
        {
          case TextSpan.ChangeType.TextChanged:
            Text = _span.Text;
            break;
          case TextSpan.ChangeType.StyleChanged:
            break;
          default:
            throw new ArgumentOutOfRangeException(nameof(changeType), changeType, null);
        }
      }
    }
  }
}