using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TextRight.ContentEditor.Desktop.Utilities
{
  /// <summary> Contains a measurement of a location within the block. </summary>
  public struct MeasuredRectangle
  {
    /// <summary> The X coordinate of the rectangle. </summary>
    public double X;

    /// <summary> The Y coordinate of the rectangle. </summary>
    public double Y;

    /// <summary> The width of the rectangle. </summary>
    public double Width;

    /// <summary> The height of the rectangle. </summary>
    public double Height;
  }
}