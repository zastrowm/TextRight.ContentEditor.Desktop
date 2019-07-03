using System;
using System.Collections.Generic;
using System.Linq;
using TextRight.Core.ObjectModel.Blocks.Text;

namespace TextRight.Core.ObjectModel
{
  /// <summary>
  ///  Contains data that is tied to a specific range of data within a buffer, usually a
  ///  <see cref="TextBlock"/> or <see cref="TextSpan"/>.
  /// </summary>
  public class Markup
  {
    private int _absoluteIndex;
    private readonly MarkupNodeReference _node;
    private ChangeIndex _changeIndex;

    /// <summary> Constructor. </summary>
    /// <param name="node"> The node for which this instance wraps. </param>
    internal Markup(MarkupNodeReference node)
    {
      _node = node;
      // force the absolute index to be recalculated on demand
      _changeIndex = node.Owner.ChangeIndex.Previous();
      _absoluteIndex = 0;
    }

    /// <summary> The object that describes how the markup is used of the markup. </summary>
    public IMarkupDescriptor Descriptor
      => _node.Descriptor;

    /// <summary> Any additional data that was added when the markup was created. </summary>
    public IMarkupData AdditionalData
      => _node.AssociatedData;

    /// <summary> Sets the absolute index of the given instance. </summary>
    /// <param name="absoluteIndex"> The current absolute index of the node. </param>
    internal void SetAbsoluteIndex(int absoluteIndex)
    {
      _absoluteIndex = absoluteIndex;
      _changeIndex = _node.Owner.ChangeIndex;
    }

    /// <summary>
    ///   Gets the range for which this markup is active. The returned value is only
    ///   valid until the document is modified again.
    /// </summary>
    /// <returns> The calculated range. </returns>
    /// <remarks> This is potentially an O(n) operation. </remarks>
    public Range GetRange()
    {
      if (_node.Owner.ChangeIndex.HasChanged(ref _changeIndex))
      {
        _absoluteIndex = _node.CalculateAbsoluteIndex();
      }

      return new Range(_absoluteIndex, _absoluteIndex + _node.Length);
    }
  }
}