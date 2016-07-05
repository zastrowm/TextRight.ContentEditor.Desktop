using System;
using System.Collections.Generic;
using System.Linq;

namespace TextRight.ContentEditor.Core.ObjectModel.Blocks
{
  /// <summary> Contains a single range of markup for a fragment. </summary>
  public class FragmentMarkup
  {
    private int _absoluteIndex;
    private readonly SubFragmentMarkupCollection.SubFragmentNode _node;
    private ChangeIndex _changeIndex;

    /// <summary> Constructor. </summary>
    /// <param name="node"> The node for which this instance wraps. </param>
    internal FragmentMarkup(SubFragmentMarkupCollection.SubFragmentNode node)
    {
      _node = node;
      // force the absolute index to be recalculated on demand
      _changeIndex = node.Owner.ChangeIndex.Previous();
      _absoluteIndex = 0;
    }

    /// <summary> Sets the absolute index of the given instance. </summary>
    /// <param name="absoluteIndex"> The current absolute index of the node. </param>
    internal void SetAbsoluteIndex(int absoluteIndex)
    {
      _absoluteIndex = absoluteIndex;
      _changeIndex = _node.Owner.ChangeIndex;
    }

    /// <summary> Gets the range for which this markup is active.  </summary>
    /// <returns> The calculated range. </returns>
    /// <remarks> This is potentially an O(n) operation. </remarks>
    public TextRange GetRange()
    {
      if (_node.Owner.ChangeIndex.HasChanged(ref _changeIndex))
      {
        _absoluteIndex = _node.CalculateAbsoluteIndex();
      }

      return new TextRange(_absoluteIndex, _absoluteIndex + _node.Length);
    }

    /// <summary> The type of the markup. </summary>
    public SubFragmentMarkupType Type
      => _node.Type;

    /// <summary> Any additional data that was added when the markup was created. </summary>
    public object AdditionalData
      => _node.AssociatedData;
  }
}