using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using TextRight.Core.Cursors;
using TextRight.Core.ObjectModel.Cursors;
using TextRight.Core.Utilities;

namespace TextRight.Editor.Wpf.View
{
  /// <summary>
  ///   Helper methods for implementations of IBlockCollectionView
  /// </summary>
  public static class CollectionViewHelper
  {
    public static MeasuredRectangle MeasureSelectionBounds(DocumentEditorContextView root, FrameworkElement self)
    {
      var offset = self.TransformToAncestor(root).Transform(new Point(0, 0));

      return new MeasuredRectangle()
             {
               X = offset.X,
               Y = offset.Y,
               Width = self.ActualWidth,
               Height = self.ActualHeight
             };
    }
    
    public static BlockCaret GetCaretFromBottom(UIElementCollection collection, CaretMovementMode movementMode)
      => ((IBlockView)collection.First()).GetCaretFromBottom(movementMode);

    public static BlockCaret GetCaretFromTop(UIElementCollection collection, CaretMovementMode movementMode)
      => ((IBlockView)collection.First()).GetCaretFromTop(movementMode);
  }
}