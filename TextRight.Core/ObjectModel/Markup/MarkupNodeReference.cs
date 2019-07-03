using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using TextRight.Core.ObjectModel.Blocks.Text;

namespace TextRight.Core.ObjectModel
{
  /// <summary>
  ///  Internal data-structure for holding the relevent data for a <see cref="Markup"/>.
  /// </summary>
  internal class MarkupNodeReference
  {
    // ReSharper disable once NotNullMemberIsNotInitialized
    public MarkupNodeReference(MarkupCollection owner, IMarkupDescriptor descriptor, IMarkupData associatedData)
    {
      Owner = owner;
      Descriptor = descriptor;
      AssociatedData = associatedData;

      Markup = new Markup(this);
    }

    /// <summary> The collection that owns this node. </summary>
    public MarkupCollection Owner { get; }

    /// <summary> The actual Markup instance associated with this node. </summary>
    public Markup Markup { get; }

    /// <summary> The type of markup that this span represents. </summary>
    public IMarkupDescriptor Descriptor { get; }

    /// <summary> Any additional data associated with the markup. </summary>
    public IMarkupData AssociatedData { get; }

    /// <summary>
    ///  Calculates the AbsoluteIndex for this node, by iterating through all nodes until we get to
    ///  ourselves.
    /// </summary>
    /// <returns> The calculated absolute index. </returns>
    public int CalculateAbsoluteIndex()
    {
      return Owner.CalculateAbsoluteIndex(this);
    }

    #region Externally modified

    /// <summary> The linked list node associated with the SubFragmentNode. </summary>
    [NotNull]
    public LinkedListNode<MarkupNodeReference> Node { get; set; }

    /// <summary> The gap between this node and the previous node. </summary>
    public int RelativeOffsetSinceLastNode { get; set; }

    /// <summary> The number of elements that this markup node covers. </summary>
    public int Length { get; set; }

    /// <summary>
    ///   Whether or not the node is marked for removal or needs to be revalidated.
    /// </summary>
    public NodeStatus Status { get; set; }

    #endregion

    public enum NodeStatus
    {
      Normal,
      NeedsRevalidation,
      MarkedForRemoval,
    }
  }
}