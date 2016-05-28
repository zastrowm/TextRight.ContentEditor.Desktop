using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TextRight.ContentEditor.Core.Utilities
{
  /// <summary> Contains a measurement of a location within the block. </summary>
  public struct MeasuredRectangle
  {
    /// <summary> An invalid rectangle. </summary>
    public static readonly MeasuredRectangle Invalid
      = new MeasuredRectangle();

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
      => Width > 0 && Height > 0;

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
  }
}