using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Documents;
using TextRight.ContentEditor.Desktop.ObjectModel;
using TextRight.ContentEditor.Desktop.ObjectModel.Blocks;

namespace TextRight.ContentEditor.Desktop.View
{
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