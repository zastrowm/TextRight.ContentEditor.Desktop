using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using TextRight.ContentEditor.Core.ObjectModel;
using TextRight.ContentEditor.Core.ObjectModel.Blocks;

namespace TextRight.ContentEditor.Desktop.View
{
  /// <summary> View representation of a <see cref="TextBlock"/> </summary>
  public class TextBlockView : System.Windows.Controls.TextBlock,
                               ITextBlockView
  {
    private readonly TextBlock _block;

    /// <summary> Constructor. </summary>
    /// <param name="block"> The block that this view is for. </param>
    public TextBlockView(TextBlock block)
    {
      TextWrapping = TextWrapping.Wrap;
      Margin = new Thickness(10);

      _block = block;
      _block.Target = this;

      foreach (var span in block)
      {
        Inlines.Add(new StyledStyledTextSpanView(this, span));
      }
    }

    /// <inheritdoc />
    public IDocumentItem DocumentItem
      => _block;

    /// <inheritdoc />
    public void NotifyBlockInserted(StyledTextFragment previousSibling,
                                    StyledTextFragment newFragment,
                                    StyledTextFragment nextSibling)
    {
      var newView = new StyledStyledTextSpanView(this, newFragment);

      var previousSpanView = previousSibling?.Target as StyledStyledTextSpanView;
      var nextSpanView = nextSibling?.Target as StyledStyledTextSpanView;
      if (previousSpanView != null)

      {
        Inlines.InsertAfter(previousSpanView, newView);
      }
      else if (nextSpanView != null)
      {
        Inlines.InsertBefore(nextSpanView, newView);
      }
      else
      {
        Inlines.Add(newView);
      }
    }
  }
}