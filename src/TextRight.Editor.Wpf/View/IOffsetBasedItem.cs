using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace TextRight.Editor.Wpf.View
{
  /// <summary> An View-Object that has an offset that may change as time goes on. </summary>
  public interface IOffsetBasedItem
  {
    Point Offset { get; }
  }
}