using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using TextRight.ContentEditor.Core.ObjectModel;
using TextBlock = TextRight.ContentEditor.Core.ObjectModel.Blocks.TextBlock;

namespace TextRight.ContentEditor.Desktop.View
{
  /// <summary> A view to represent a cursor. </summary>
  public interface ICaretView
  {
    void UpdatePosition(double x, double y, double height);
  }

  /// <summary> Responsible to presenting a caret in the editor to the user. </summary>
  internal class CaretView : ICaretView
  {
    private readonly DocumentCursor _cursor;
    private readonly Rectangle _rectangle;

    /// <summary> Constructor. </summary>
    /// <param name="cursor"> The cursor that should be presented to the user. </param>
    public CaretView(DocumentCursor cursor)
    {
      _cursor = cursor;
      _rectangle = new Rectangle()
                   {
                     Height = 20,
                     Width = 1,
                     Fill = Brushes.Black,
                   };

      Panel.SetZIndex(_rectangle, 10);
    }

    /// <summary> The element that should be added to a Canvas. </summary>
    public FrameworkElement Element
      => _rectangle;

    /// <summary> Synchronized the view's representation with the underlying cursor. </summary>
    public void SyncPosition()
    {
      // TODO do we need to cast it as a text cursor?  Should the block cursor
      // know how to measure itself? 
      var textCursor = (TextBlock.TextBlockCursor)_cursor.BlockCursor;

      var block = (TextBlock)_cursor.BlockCursor.Block;
      var measure = textCursor.Fragment.Target.Measure(textCursor.OffsetIntoSpan);

      // TODO should core being doing this?
      UpdatePosition(measure.X, measure.Y, measure.Height);
    }

    public void UpdatePosition(double x, double y, double height)
    {
      Canvas.SetLeft(_rectangle, x);
      Canvas.SetTop(_rectangle, y);
      _rectangle.Height = height;
    }
  }
}