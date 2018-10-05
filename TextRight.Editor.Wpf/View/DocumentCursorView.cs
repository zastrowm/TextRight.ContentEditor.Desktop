using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using TextRight.Core.ObjectModel;
using TextRight.Core.ObjectModel.Cursors;
using TextRight.Core.Utilities;

namespace TextRight.Editor.Wpf.View
{
  /// <summary> The visual representation of a DocumentCursor and the selection. </summary>
  public class DocumentCursorView : Grid
  {
    private readonly DocumentSelection _cursor;
    private readonly PointCollection _selectionPointCollection;
    private readonly PointCollection _caretPointsCollection;
    private readonly Polygon _selectionPolygon;
    private readonly Polygon _caretRect;

    /// <summary> Indicates if the caret's block changed and we need to re-render. </summary>
    private IndexLayout _layoutIndex;

    private const int CaretWidth = 2;

    private bool _isDirty = true;

    /// <summary> Default constructor. </summary>
    public DocumentCursorView(DocumentSelection cursor)
    {
      _cursor = cursor;
      _cursor.CursorMoved += HandleCaretChanged;

      // selection
      _selectionPointCollection = new PointCollection(8)
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

      _selectionPolygon = new Polygon
                          {
                            Fill = new SolidColorBrush()
                                   {
                                     Color = Color.FromArgb(128, 128, 128, 128)
                                   },
                            Points = _selectionPointCollection
                          };


      // caret
      _caretPointsCollection = new PointCollection(4)
                               {
                                 new Point(0, 0),
                                 new Point(1, 0),
                                 new Point(1, 30),
                                 new Point(0, 30),
                               };

      _caretRect = new Polygon()
                   {
                     Fill = new SolidColorBrush()
                            {
                              Color = Colors.Black,
                            },
                     Points = _caretPointsCollection,
                   };

      Children.Add(_selectionPolygon);
      Children.Add(_caretRect);
    }

    /// <summary> Verify that the cursor is not rendering according to the last layout. </summary>
    public void VerifyNotStale()
    {
      var newIndex = GetCurrentLayoutIndex();

      if (newIndex.HasChanged(ref _layoutIndex))
      {
        MarkDirty();
      }
    }

    /// <summary> Force the view to re-render. </summary>
    public void MarkDirty()
    {
      _isDirty = true;
      InvalidateMeasure();
    }

    private void HandleCaretChanged(object sender, EventArgs e)
    {
      MarkDirty();
    }

    private bool _isQueued = false;

    private async void RemeasureUntilNotInvalid()
    {
      if (_isQueued)
        return;

      _isQueued = true;

      await Task.Yield();

      _isQueued = false;

      if (MeasureCursor())
      {
        InvalidateMeasure();
      }
    }

    protected override Size MeasureOverride(Size constraint)
    {
      MeasureCursor();

      return base.MeasureOverride(constraint);
    }

    protected override Size ArrangeOverride(Size arrangeSize)
    {
      MeasureCursor();

      return base.ArrangeOverride(arrangeSize);
    }

    /// <summary> The visual that represents the caret. </summary>
    public FrameworkElement CaretElement
      => _caretRect;

    /// <summary> The visual that represents the selection. </summary>
    public FrameworkElement SelectionElement
      => _selectionPolygon;

    private bool MeasureCursor()
    {
      if (!_isDirty)
        return true;

      var start = _cursor.Start.Measure();
      if (!start.IsValid)
      {
        RemeasureUntilNotInvalid();
        // it's possible that we haven't had a new-layout yet, in which case we need to wait until the next tick
        //Debug.Fail("How");
        return false;
      }

      if (_cursor.HasSelection)
      {
        _selectionPolygon.Visibility = Visibility.Visible;
        UpdateSelectionPolygon(start);
      }
      else
      {
        _selectionPolygon.Visibility = Visibility.Hidden;
      }

      UpdateCaretRectangle(start);

      _layoutIndex = GetCurrentLayoutIndex();

      _isDirty = false;
      return true;
    }

    private IndexLayout GetCurrentLayoutIndex()
    {
      // TODO how can we do this not having so many 'as' casts?
      var associatedView = (_cursor.Start.Block as IDocumentItem)?.DocumentItemView as ILayoutable;
      var layoutIndex = new IndexLayout(associatedView);
      return layoutIndex;
    }

    /// <summary> Updates the rectangle for displaying the caret. </summary>
    private void UpdateCaretRectangle(MeasuredRectangle caretPosition)
    {

      /**
         *                  
         *      0---------1
         *      |         |
         *  ----3---------2---
         */
      _caretPointsCollection[0] = new Point(caretPosition.X, caretPosition.Y);
      _caretPointsCollection[1] = new Point(caretPosition.X, caretPosition.Y + caretPosition.Height);
      _caretPointsCollection[2] = new Point(caretPosition.X + CaretWidth, caretPosition.Y + caretPosition.Height);
      _caretPointsCollection[3] = new Point(caretPosition.X + CaretWidth, caretPosition.Y);

      _caretRect.Points = _caretPointsCollection;
    }

    /// <summary> Updates the rectangle that shows the current selection. </summary>
    private void UpdateSelectionPolygon(MeasuredRectangle caretPosition)
    {
      var start = caretPosition;
      var end = _cursor.End.Measure();

      if (MeasuredRectangle.AreInline(start, end))
        DrawInlineSelection(start, end);
      else
        DrawSpanningSelection(start, end);

      _selectionPolygon.Points = _selectionPointCollection;
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

      var topMost = Math.Min(leftRect.Top, rightRect.Top);
      var botMost = Math.Max(leftRect.Bottom, rightRect.Bottom);

      _selectionPointCollection[0] = new Point(leftRect.Left, botMost);
      _selectionPointCollection[1] = new Point(leftRect.Left, topMost);

      _selectionPointCollection[2] = new Point(rightRect.Right, topMost);
      _selectionPointCollection[3] = new Point(rightRect.Right, botMost);

      _selectionPointCollection[4] = _selectionPointCollection[3];
      _selectionPointCollection[5] = _selectionPointCollection[3];
      _selectionPointCollection[6] = _selectionPointCollection[3];
      _selectionPointCollection[7] = _selectionPointCollection[3];
    }

    /// <summary> Draw the selection as spanning one or more lines. </summary>
    /// <param name="start"> The position of the start of the selection. </param>
    /// <param name="end"> The position of the end of the selection. </param>
    private void DrawSpanningSelection(MeasuredRectangle start, MeasuredRectangle end)
    {
      // TODO we really should go line-by-line or block-by-block as needed
      var startBlockRect = _cursor.Start.Block.GetBounds();
      var endBlockRect = _cursor.End.Block.GetBounds();

      var maxRight = Math.Max(startBlockRect.Right, endBlockRect.Right);
      var maxLeft = Math.Max(startBlockRect.Left, endBlockRect.Left);

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

      _selectionPointCollection[0] = new Point(upperRect.Left, upperRect.Bottom);
      _selectionPointCollection[1] = new Point(upperRect.Left, upperRect.Top);

      _selectionPointCollection[2] = new Point(maxRight, upperRect.Top);
      _selectionPointCollection[3] = new Point(maxRight, lowerRect.Top);

      _selectionPointCollection[4] = new Point(lowerRect.Right, lowerRect.Top);
      _selectionPointCollection[5] = new Point(lowerRect.Right, lowerRect.Bottom);

      _selectionPointCollection[6] = new Point(maxLeft, lowerRect.Bottom);
      _selectionPointCollection[7] = new Point(maxLeft, upperRect.Bottom);
    }
  }
}