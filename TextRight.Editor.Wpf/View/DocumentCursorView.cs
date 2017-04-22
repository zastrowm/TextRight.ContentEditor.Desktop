using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;
using TextRight.Core.ObjectModel.Cursors;
using TextRight.Core.Utilities;

namespace TextRight.Editor.Wpf.View
{
  /// <summary> The visual representation of a DocumentCursor. </summary>
  public class DocumentCursorView
  {
    private readonly DocumentCursor _cursor;
    private readonly PointCollection _pointCollection;
    private readonly Polygon _polygon;
    private readonly Rectangle _rectangle;
    private bool _isDirty;

    /// <summary> Default constructor. </summary>
    public DocumentCursorView(DocumentCursor cursor)
    {
      _cursor = cursor;
      _cursor.CursorMoved += InvalidateCursor;

      _rectangle = new Rectangle()
                   {
                     Height = 20,
                     Width = 1,
                     Fill = Brushes.Black,
                   };
      Panel.SetZIndex(_rectangle, 10);

      _polygon = new Polygon
                 {
                   Fill = new SolidColorBrush()
                          {
                            Color = Color.FromArgb(128, 128, 128, 128)
                          },
                   Points = _pointCollection
                 };
      Panel.SetZIndex(_rectangle, 10);

      _pointCollection = new PointCollection()
                         {
                           new Point(0, 0),
                           new Point(0, 50),
                           new Point(50, 50),
                           new Point(50, 0),
                           new Point(0, 0),
                           new Point(0, 50),
                           new Point(50, 50),
                           new Point(50, 0),
                         };

      _polygon.Points = _pointCollection;
    }

    private void InvalidateCursor(object sender, EventArgs e)
    {
      _isDirty = true;
    }

    /// <summary> Attaches the view to the given editor. </summary>
    /// <param name="editor"> The editor to which the view should be attached.. </param>
    public void Attach(DocumentEditorContextView editor)
    {
      editor.Children.Add(_polygon);
      editor.Children.Add(_rectangle);
    }

    /// <inheritdoc/>
    public void Refresh()
    {
      Refresh(false);
    }

    /// <inheritdoc />
    public void Refresh(bool shouldForce)
    {
      if (!shouldForce && !_isDirty)
        return;

      _isDirty = true;

      var start = _cursor.Cursor.MeasureCursorPosition();
      if (!start.IsValid)
      {
        // it's possible that we haven't had a new-layout yet, in which case we need to wait until the next tick

        // TODO OPTIMIZE by having some sort of queue of future actions
        Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.ContextIdle, new Action(Refresh));
        return;
      }

      if (_cursor.HasSelection)
      {
        _polygon.Visibility = Visibility.Visible;
        UpdateSelectionPolygon(start);
      }
      else
      {
        _polygon.Visibility = Visibility.Hidden;
      }

      UpdateCaretRectangle(start);
    }

    /// <summary> Updates the rectangle for displaying the caret. </summary>
    private void UpdateCaretRectangle(MeasuredRectangle caretPosition)
    {
      // always update the caret position
      Canvas.SetLeft(_rectangle, caretPosition.X);
      Canvas.SetTop(_rectangle, caretPosition.Y);
      _rectangle.Height = caretPosition.Height;
    }

    /// <summary> Updates the rectangle that shows the current selection. </summary>
    private void UpdateSelectionPolygon(MeasuredRectangle caretPosition)
    {
      MeasuredRectangle start = caretPosition;
      var end = _cursor.SelectionStart.MeasureCursorPosition();

      if (MeasuredRectangle.AreInline(start, end))
      {
        DrawInlineSelection(start, end);
      }
      else
      {
        DrawSpanningSelection(start, end);
      }

      _polygon.Points = _pointCollection;
    }

    /// <summary> Draw the selection as existing on a single line. </summary>
    /// <param name="start"> The position of the start of the selection. </param>
    /// <param name="end"> The position of the end of the selection. </param>
    private void DrawInlineSelection(MeasuredRectangle start, MeasuredRectangle end)
    {
      MeasuredRectangle leftRect;
      MeasuredRectangle rightRect;

      if (start.Left < end.Left)
      {
        leftRect = start;
        rightRect = end;
      }
      else
      {
        rightRect = start;
        leftRect = end;
      }

      /**
         *                  
         *      1---------2
         *      |         |
         *  ----0---------3---
         */

      double topMost = Math.Min(leftRect.Top, rightRect.Top);
      double botMost = Math.Max(leftRect.Bottom, rightRect.Bottom);

      _pointCollection[0] = new Point(leftRect.Left, botMost);
      _pointCollection[1] = new Point(leftRect.Left, topMost);

      _pointCollection[2] = new Point(rightRect.Right, topMost);
      _pointCollection[3] = new Point(rightRect.Right, botMost);

      _pointCollection[4] = _pointCollection[3];
      _pointCollection[5] = _pointCollection[3];
      _pointCollection[6] = _pointCollection[3];
      _pointCollection[7] = _pointCollection[3];
    }

    /// <summary> Draw the selection as spanning one or more lines. </summary>
    /// <param name="start"> The position of the start of the selection. </param>
    /// <param name="end"> The position of the end of the selection. </param>
    private void DrawSpanningSelection(MeasuredRectangle start, MeasuredRectangle end)
    {
      // TODO we really should go line-by-line or block-by-block as needed
      var startBlockRect = _cursor.Cursor.Block.GetBounds();
      var endBlockRect = _cursor.SelectionStart.Block.GetBounds();

      double maxRight = Math.Max(startBlockRect.Right, endBlockRect.Right);
      double maxLeft = Math.Max(startBlockRect.Left, endBlockRect.Left);

      // It may not be strictly needed, but it's easier for me to understand the code if I know
      // which rect is the "upper" rect and which one is the "lower" rect.
      MeasuredRectangle upperRect;
      MeasuredRectangle lowerRect;

      if (start.Top < end.Top)
      {
        upperRect = start;
        lowerRect = end;
      }
      else
      {
        lowerRect = start;
        upperRect = end;
      }

      /**
         *                  
         *      1------------2
         *      |            |
         *  7---0            |
         *  |                |
         *  |                |
         *  |                |
         *  |                |
         *  |                |
         *  |           4----3
         *  |           |
         *  6-----------5
         */

      _pointCollection[0] = new Point(upperRect.Left, upperRect.Bottom);
      _pointCollection[1] = new Point(upperRect.Left, upperRect.Top);

      _pointCollection[2] = new Point(maxRight, upperRect.Top);
      _pointCollection[3] = new Point(maxRight, lowerRect.Top);

      _pointCollection[4] = new Point(lowerRect.Right, lowerRect.Top);
      _pointCollection[5] = new Point(lowerRect.Right, lowerRect.Bottom);

      _pointCollection[6] = new Point(maxLeft, lowerRect.Bottom);
      _pointCollection[7] = new Point(maxLeft, upperRect.Bottom);
    }
  }
}