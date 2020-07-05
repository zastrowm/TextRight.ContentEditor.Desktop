using System;
using System.Collections.Generic;
using System.Linq;
using TextRight.Core.ObjectModel;

namespace TextRight.Editor.Wpf.View
{
  /// <summary> An view which performs layout. </summary>
  public interface ILayoutable : IDocumentItemView
  {
    /// <summary> The current change index for the current layout. </summary>
    ChangeIndex LayoutIndex { get; }
  }
}