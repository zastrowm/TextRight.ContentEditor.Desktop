﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace TextRight.Core
{
  /// <summary>
  ///  A simple X,Y value that represents a point relative to the top of the document.
  /// </summary>
  [DebuggerDisplay("X: {X}, Y: {Y}")]
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

    /// <summary>
    ///  Measures the distance between two points, squared, which is useful when determining which
    ///  points are closer to each other.
    /// </summary>
    public static double MeasureDistanceSquared(DocumentPoint left, DocumentPoint right)
    {
      var dX = (left.X - right.X);
      var dY = (left.Y - right.Y);
      return dX * dX + dY * dY;
    }

    /// <inheritdoc />
    public override string ToString() 
      => $"{X}, {Y}";
  }
}