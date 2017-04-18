using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace TextRight.Core.Utilities
{
  /// <summary> Contains a measurement of a point. </summary>
  [DebuggerDisplay("X: {X}, Y: {Y}")]
  public struct MeasuredPoint
  {
    /// <summary> The X coordinate of the point. </summary>
    public double X { get; set; }

    /// <summary> The Y coordinate of the point. </summary>
    public double Y { get; set; }
  }
}