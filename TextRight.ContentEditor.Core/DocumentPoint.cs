using System;
using System.Collections.Generic;
using System.Linq;

namespace TextRight.ContentEditor.Core
{
  /// <summary> A simple X,Y value.  Not named Point, because everyone uses that. </summary>
  public struct DocumentPoint
  {
    /// <summary> Constructor. </summary>
    /// <param name="x"> The X coordinate of the point. </param>
    /// <param name="y"> The Y coordinate of the point. </param>
    public DocumentPoint(double x, double y)
    {
      X = x;
      Y = y;
    }

    /// <summary> The X coordinate of the point. </summary>
    public double X { get; set; }

    /// <summary> The Y coordinate of the point. </summary>
    public double Y { get; set; }
  }
}