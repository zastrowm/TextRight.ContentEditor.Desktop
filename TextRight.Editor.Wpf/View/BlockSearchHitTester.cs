using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using TextRight.Core;
using TextRight.Core.ObjectModel;
using TextRight.Core.ObjectModel.Blocks;
using TextRight.Core.ObjectModel.Blocks.Collections;

namespace TextRight.Editor.Wpf.View
{
  /// <summary> Performs a HitTest to determine which block a point belongs to. </summary>
  internal class BlockSearchHitTester
  {
    private static readonly object TagThatIndicatesHitTestingShouldStop 
      = new object();

    private readonly DocumentEditorContextView _ownerView;
    private readonly HitTestFilterCallback _hitTestFilterCallback;
    private readonly HitTestResultCallback _hitTestResultCallback;
    private Block _block;

    /// <summary> Constructor. </summary>
    /// <param name="ownerView"> The visual that is performing the hit test. </param>
    public BlockSearchHitTester(DocumentEditorContextView ownerView)
    {
      _ownerView = ownerView;
      _hitTestFilterCallback = FilterCallback;
      _hitTestResultCallback = ResultCallback;
    }

    /// <summary>
    ///  Marks the element as indicating that future hit testing should be stopped when this element
    ///  is reached.
    /// </summary>
    public static void SetShouldStopHitTesting(FrameworkElement element, bool enable)
    {
      if (enable)
      {
        if (element.Tag == null || element.Tag == TagThatIndicatesHitTestingShouldStop)
        {
          element.Tag = TagThatIndicatesHitTestingShouldStop;
        }
        else
        {
          throw new ArgumentException("Element already has «Tag» property set, which is the way that hit testing is indicated to be stopped.");
        }
      }
      else
      {
        if (element.Tag == TagThatIndicatesHitTestingShouldStop)
        {
          element.Tag = null;
        }
      }
    }

    /// <summary> True if hit testing should stop after reaching this element. </summary>
    public static bool GetShouldStopHitTesting(FrameworkElement element) 
      => element.Tag == TagThatIndicatesHitTestingShouldStop;

    /// <summary> Uses HitTesting to determine the block at the specified point. </summary>
    /// <param name="point"> The point at which the hit test should be performed. </param>
    /// <returns> The block at a the given point. </returns>
    public Block GetBlockAt(DocumentPoint point)
    {
      _block = null;

      if (point.Y < 0)
      {
        return _ownerView.GetBlockFor(new DocumentPoint(0, 0));
      }
      else if (point.Y > _ownerView.RootVisual.ActualHeight)
      {
        var lastBlock = _ownerView.Context.Document.Root.LastBlock;
        return BlockTreeWalker.GetNextNonContainerBlock(lastBlock);
      }

      var localPoint = _ownerView.ToPoint(point);
      var instance = _ownerView.InputHitTest(localPoint);

      if (instance is IDocumentItemView view 
          && GetAssociatedBlock(view) is Block block)
      {
        _block = block;
      }
      else
      {
        VisualTreeHelper.HitTest(_ownerView,
                                 _hitTestFilterCallback,
                                 _hitTestResultCallback,
                                 new PointHitTestParameters(localPoint));
      }

      return _block;
    }

    /// <summary>
    ///  Tries to get the block associated with the given <see cref="IDocumentItemView"/>, or null if
    ///  no such block is associated with it.
    /// </summary>
    private static Block GetAssociatedBlock(IDocumentItemView documentItemView)
    {
      if (documentItemView.DocumentItem is Block block 
          && !(block is BlockCollection))
      {
        return block;
      }

      return null;
    }

    /// <summary>
    ///  Callback for use with
    ///  <see cref="VisualTreeHelper.HitTest(Visual, HitTestFilterCallback, HitTestResultCallback, HitTestParameters)"/>
    /// </summary>
    private HitTestFilterBehavior FilterCallback(DependencyObject potentialHitTestTarget)
    {
      var documentItemView = potentialHitTestTarget as IDocumentItemView;
      if (documentItemView == null)
      {
        bool shouldStop = potentialHitTestTarget is FrameworkElement element && GetShouldStopHitTesting(element);

        return shouldStop ? HitTestFilterBehavior.ContinueSkipSelfAndChildren : HitTestFilterBehavior.Continue;
      }

      if (GetAssociatedBlock(documentItemView) is Block block)
      {
        _block = block;
        return HitTestFilterBehavior.Stop;
      }

      return HitTestFilterBehavior.Continue;
    }

    /// <summary>
    ///  Callback for use with
    ///  <see cref="VisualTreeHelper.HitTest(Visual, HitTestFilterCallback, HitTestResultCallback, HitTestParameters)"/>
    /// </summary>
    private HitTestResultBehavior ResultCallback(HitTestResult result)
    {
      return HitTestResultBehavior.Continue;
    }
  }
}