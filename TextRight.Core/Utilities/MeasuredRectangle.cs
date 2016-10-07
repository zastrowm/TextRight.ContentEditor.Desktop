using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace TextRight.Core.Utilities
{
  /// <summary> Contains a measurement of a location within the block. </summary>
  [DebuggerDisplay("Left: {Left}, Top: {Top}")]
  public struct MeasuredRectangle
  {
    /// <summary> An invalid rectangle. </summary>
    public static readonly MeasuredRectangle Invalid
      = new MeasuredRectangle(double.MinValue, double.MinValue, 0, 0);

    /// <summary> Constructor. </summary>
    /// <param name="x"> The X coordinate of the rectangle. </param>
    /// <param name="y"> The Y coordinate of the rectangle. </param>
    /// <param name="width"> The width of the rectangle. </param>
    /// <param name="height"> The height of the rectangle. </param>
    public MeasuredRectangle(double x, double y, double width, double height)
    {
      X = x;
      Y = y;
      Width = width;
      Height = height;
    }

    /// <summary> The X coordinate of the rectangle. </summary>
    public double X { get; set; }

    /// <summary> The Y coordinate of the rectangle. </summary>
    public double Y { get; set; }

    /// <summary> The width of the rectangle. </summary>
    public double Width { get; set; }

    /// <summary> The height of the rectangle. </summary>
    public double Height { get; set; }

    public double Top
      => Y;

    public double Bottom
      => Y + Height;

    public double Left
      => X;

    public double Right
      => X + Width;

    /// <summary> True if the rectangle has a valid width and height. </summary>
    public bool IsValid
    {
      get
      {
        // ReSharper disable CompareOfFloatsByEqualityOperator
        return X != double.MinValue && Y != double.MinValue;
        // ReSharper restore CompareOfFloatsByEqualityOperator
      }
    }

    /// <summary> Gets a point that represents the center of the given point. </summary>
    public DocumentPoint Center
      => new DocumentPoint(X + Width / 2, Y + Height / 2);

    /// <summary>
    ///  Flattens the rectangle into a line representing the right side of the
    ///  rectangle.
    /// </summary>
    /// <returns> The original MeasuredRectangle. </returns>
    public MeasuredRectangle FlattenRight()
    {
      X = Right;
      Width = 0;

      return this;
    }

    /// <summary>
    ///  Flattens the rectangle into a line representing the left side of the
    ///  rectangle.
    /// </summary>
    /// <returns> The original MeasuredRectangle. </returns>
    public MeasuredRectangle FlattenLeft()
    {
      Width = 0;

      return this;
    }

    /// <summary>
    ///  Check if one rectangle is considered on the same "line" as another.
    /// </summary>
    public static bool AreInline(MeasuredRectangle original, MeasuredRectangle comparedTo)
    {
      MeasuredRectangle first,
                        second;

      if (original.Top <= comparedTo.Top)
      {
        first = original;
        second = comparedTo;
      }
      else
      {
        first = comparedTo;
        second = original;
      }

      // if the second point has its top between the top of the first
      // point and the first points bottom, the second point is considered
      // inline with the other
      // TODO do we need some sort of buffer (perhaps subtracting a small number from top?
      return second.Top < first.Bottom;
    }

    /// <summary> Add the given x and y values to create a new point. </summary>
    public MeasuredRectangle OffsetBy(double x, double y)
    {
      return new MeasuredRectangle()
             {
               X = x + X,
               Y = y + Y,
               Height = Height,
               Width = Width,
             };
    }

    /// <summary> Trims the point to be within the current rectangle. </summary>
    /// <param name="point"> The point to trim. </param>
    /// <returns>
    ///  A DocumentPoint with the X/Y values that now exist within the current rectangle.
    /// </returns>
    public DocumentPoint Trim(DocumentPoint point)
    {
      point.X = Math.Max(Left, Math.Min(Right, point.X));
      point.Y = Math.Max(Top, Math.Min(Bottom, point.Y));
      return point;
    }

    /// <inheritdoc/>
    public override string ToString()
    {
      return $"X: {X}, Y: {Y}, Width: {Width}, Height: {Height}";
    }
  }
}