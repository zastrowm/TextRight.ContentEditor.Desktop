using System;
using System.Collections.Generic;
using System.Linq;
using TextRight.Core.Cursors;
using TextRight.Core.ObjectModel;
using TextRight.Core.ObjectModel.Blocks;
using TextRight.Core.Utilities;

namespace TextRight.Editor
{
  /// <summary>
  ///   The editor view for a <see cref="ContentBlock"/>
  /// </summary>
  public interface IContentBlockView : IBlockView, IEditorData
  {
    // <summary> Measures the bounds of a specific caret position. </summary>
    MeasuredRectangle Measure(BlockCaret caret);
  }
}