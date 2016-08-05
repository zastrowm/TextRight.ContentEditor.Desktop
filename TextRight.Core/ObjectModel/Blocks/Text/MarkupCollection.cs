using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using JetBrains.Annotations;

namespace TextRight.ContentEditor.Core.ObjectModel.Blocks
{
  /// <summary> Contains a collection of SubFragmentMarkups for a specific <see cref="StyledTextFragment"/>. </summary>
  public class MarkupCollection : IEnumerable<Markup>
  {
    // at some point we may want to consider using some form of an Interval Tree but for now our
    // current architecture is acceptable. (http:// en.wikipedia.org/wiki/Interval_tree) 

    private readonly IMarkupCollectionOwner _owner;
    private readonly LinkedList<MarkupNodeReference> _subFragments;

    internal MarkupCollection(IMarkupCollectionOwner owner)
    {
      _owner = owner;
      _subFragments = new LinkedList<MarkupNodeReference>();
    }

    /// <summary> Gets the index that can be used to determine when the markups have changed. </summary>
    internal ChangeIndex ChangeIndex { get; private set; }

    /// <summary> Marks the given range to have the given markup and data. </summary>
    /// <exception cref="ArgumentOutOfRangeException"> Thrown when one or more arguments are outside
    ///  the required range. </exception>
    /// <param name="range"> The range of text to mark. </param>
    /// <param name="type"> The type of markup to apply to the range. </param>
    /// <param name="additionalData"> Any additional data to store with the markup. </param>
    public Markup MarkRange(TextRange range, MarkupType type, object additionalData)
    {
      if (range.StartIndex < 0)
        throw new ArgumentOutOfRangeException(nameof(range));
      if (range.EndIndex > _owner.Length)
        throw new ArgumentOutOfRangeException(nameof(range));

      var newFragment = new MarkupNodeReference(this, type, additionalData)
                        {
                          Length = range.Length
                        };

      int previousNodeAbsoluteIndex;
      var previousNode = GetClosestNodeBefore(range.StartIndex, out previousNodeAbsoluteIndex);
      LinkedListNode<MarkupNodeReference> node;

      if (previousNode == null)
      {
        var nextNode = _subFragments.First;

        if (nextNode != null)
        {
          // because we're at the beginning, the offset of the next fragment was an absolute index, thus
          // we now convert it to a relative index by offsetting our start index. 
          nextNode.Value.RelativeOffsetSinceLastNode -= range.StartIndex;
        }

        node = _subFragments.AddFirst(newFragment);
        newFragment.RelativeOffsetSinceLastNode = range.StartIndex;
      }
      else
      {
        var nextNode = previousNode.Next;

        // fix up the next node's relative offset if need be
        if (nextNode != null)
        {
          var nextFragment = nextNode.Value;
          int absoluteIndexOfNextNode = nextFragment.RelativeOffsetSinceLastNode + previousNodeAbsoluteIndex;

          nextFragment.RelativeOffsetSinceLastNode = absoluteIndexOfNextNode - range.StartIndex;
        }

        newFragment.RelativeOffsetSinceLastNode = range.StartIndex - previousNodeAbsoluteIndex;

        node = _subFragments.AddAfter(previousNode, newFragment);
      }

      newFragment.Node = node;
      ChangeIndex = ChangeIndex.Next();

      return newFragment.Markup;
    }

    /// <summary> Updates the collection based on the given text-changed event. </summary>
    /// <param name="changeEvent"> The event for which the collection of entries should be updated. </param>
    internal void UpdateFromEvent(TextModification changeEvent)
    {
      if (changeEvent.WasAdded)
      {
        UpdateFromInsert(changeEvent);
      }
      else
      {
        UpdateFromDelete(changeEvent);
      }

      // todo update dirty things

      ChangeIndex = ChangeIndex.Next();
    }

    /// <summary> Gets the node closest to the given index. </summary>
    /// <param name="index"> The index to which we're trying to get the closest previous node. </param>
    /// <param name="absoluteIndexOfPreviousNode"> [out] The absolute index of the node that was found
    ///  (if one was found). </param>
    /// <returns> The closest node that does not surpass the given index. </returns>
    private LinkedListNode<MarkupNodeReference> GetClosestNodeBefore(int index, out int absoluteIndexOfPreviousNode)
    {
      absoluteIndexOfPreviousNode = 0;
      LinkedListNode<MarkupNodeReference> currentNode = _subFragments.First;
      LinkedListNode<MarkupNodeReference> lastNode = null;

      while (currentNode != null)
      {
        int currentIndex = absoluteIndexOfPreviousNode + currentNode.Value.RelativeOffsetSinceLastNode;

        if (currentIndex > index)
        {
          return lastNode;
        }

        lastNode = currentNode;
        absoluteIndexOfPreviousNode = currentIndex;
        currentNode = currentNode.Next;
      }

      return lastNode;
    }

    private void Invalidate(MarkupNodeReference value)
    {
      // TODO notify callers that this fragment needs to be updated
      value.IsValid = false;
    }

    /// <summary> Finds a series of nodes that are affected by a change at the given index. </summary>
    /// <param name="indexOfChange"> The index at which a change is being applied </param>
    /// <returns> The nodes that are affected by the change. </returns>
    private FoundNode GetAffectedNodes(TextRange range)
    {
      LinkedListNode<MarkupNodeReference> listNode = _subFragments.First;
      int absoluteIndexOfLastSubFragment = 0;

      FoundNode foundNode = new FoundNode();

      while (listNode != null)
      {
        MarkupNodeReference subFragment = listNode.Value;

        var subFragmentAbsoluteStart = absoluteIndexOfLastSubFragment + subFragment.RelativeOffsetSinceLastNode;
        var nodeRange = new TextRange(subFragmentAbsoluteStart, subFragmentAbsoluteStart + subFragment.Length);

        if (range.EndIndex < nodeRange.StartIndex)
        {
          // the change is before every remaining sub-fragment, so we can bail out now
          foundNode.MarkRemaining(listNode);
          break;
        }

        if (nodeRange.OverlapsInclusive(range))
        {
          foundNode.RememberFirstAndLast(nodeRange.StartIndex, listNode);
        }

        absoluteIndexOfLastSubFragment = nodeRange.StartIndex;
        listNode = listNode.Next;
      }

      return foundNode;
    }

    /// <summary> Updates the ranges as a result of a insert text modification. </summary>
    private void UpdateFromInsert(TextModification changeEvent)
    {
      var findings = GetAffectedNodes(new TextRange(changeEvent.Index, changeEvent.Index));

      bool didAdjustedOffset = false;

      foreach (var nodeAndRange in findings.GetAffectedFragments())
      {
        var currentFragmentNode = nodeAndRange.Node;
        var currentRange = nodeAndRange.Range;

        if (currentRange.ContainsExclusive(changeEvent.Index))
        {
          // if the change is in the middle, extend the range to include the newly inserted text. 
          currentFragmentNode.Length += changeEvent.NumberOfCharacters;
        }
        else if (!didAdjustedOffset && currentRange.StartIndex == changeEvent.Index)
        {
          didAdjustedOffset = true;
          currentFragmentNode.RelativeOffsetSinceLastNode += changeEvent.NumberOfCharacters;
        }

        Invalidate(currentFragmentNode);
      }

      if (!didAdjustedOffset && findings.RemainingNode != null)
      {
        // we need to offset the remaining nodes, so make sure this offset is adjusted
        findings.RemainingNode.Value.RelativeOffsetSinceLastNode += changeEvent.NumberOfCharacters;
      }
    }

    /// <summary> Updates the ranges as a result of a delete text modification. </summary>
    private void UpdateFromDelete(TextModification changeEvent)
    {
      // $Simpler?$: NOTE deletion is so much more complicated that insert.  It's possible that this
      // implementation is just overly complicated, so we may want to revisit this at some point. 
      var deletionRange = new TextRange(changeEvent.Index, changeEvent.Index + changeEvent.NumberOfCharacters);
      var findings = GetAffectedNodes(deletionRange);

      bool didAdjustedOffset = false;
      int adjustmentsThusFar = 0;

      foreach (var nodeAndRange in findings.GetAffectedFragments())
      {
        var currentFragmentNode = nodeAndRange.Node;
        var currentRange = nodeAndRange.Range;
        // originalRange: the range before all previous edits to preceding ranges
        // 
        // the algorithm didn't originally use the originalRange, but after finding the edge cases that
        // the UTs exposed, it was determined that it was easier to reason about.  However, it's
        // possible that a simpler algorithm exists (see $Simpler?$ above)
        var originalRange = new TextRange(currentRange.StartIndex + adjustmentsThusFar,
                                          currentRange.EndIndex + adjustmentsThusFar);

        // if the deletion range is before the markup range, we need to both reduce the length and shift it over
        if (deletionRange.StartIndex <= originalRange.StartIndex)
        {
          // reduce the length by the number of characters after our start index
          int reduceLengthBy = Math.Min(deletionRange.EndIndex - originalRange.StartIndex,
                                        originalRange.Length);

          // $SafeReduction$: because we're using the original range without taking into account the modifications
          // of ranges before this one, it's possible that we get a negative # to delete, which is not
          // what we want, so normalize it. 
          reduceLengthBy = Math.Max(reduceLengthBy, 0);

          currentFragmentNode.Length -= reduceLengthBy;

          int startIndexOffset = originalRange.StartIndex - deletionRange.StartIndex;

          // again, we're using the original range, so we need to adjust our value by the offset that everyone 
          // has already offset by
          startIndexOffset = startIndexOffset - adjustmentsThusFar;

          currentFragmentNode.RelativeOffsetSinceLastNode -= startIndexOffset;
          adjustmentsThusFar += startIndexOffset;

          didAdjustedOffset = true;
        }
        else /* deletion range starts inside our range */
        {
          int numberOfCharactersToRemove = Math.Min(originalRange.EndIndex - deletionRange.StartIndex,
                                                    deletionRange.Length);
          // see comment ~20 lines up ($SafeReduction$)
          numberOfCharactersToRemove = Math.Max(numberOfCharactersToRemove, 0);
          currentFragmentNode.Length -= numberOfCharactersToRemove;
        }

        Invalidate(currentFragmentNode);
      }

      // this would only occur if nothing above adjusted the offset, in which case all of the nodes
      // are after the deletion. 
      if (!didAdjustedOffset && findings.RemainingNode != null)
      {
        // we need to offset the remaining nodes, so make sure this offset is adjusted
        findings.RemainingNode.Value.RelativeOffsetSinceLastNode -= changeEvent.NumberOfCharacters;
      }
    }

    public IEnumerator<Markup> GetEnumerator()
    {
      int absoluteIndex = 0;

      foreach (var item in _subFragments)
      {
        absoluteIndex += item.RelativeOffsetSinceLastNode;
        var instance = item.Markup;
        instance.SetAbsoluteIndex(absoluteIndex);
        yield return instance;
      }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
      return GetEnumerator();
    }

    /// <summary>
    ///  Internal data-structure for holding the relevent data for a <see cref="Markup"/>.
    /// </summary>
    internal class MarkupNodeReference
    {
      // ReSharper disable once NotNullMemberIsNotInitialized
      public MarkupNodeReference(MarkupCollection owner, MarkupType type, object associatedData)
      {
        Owner = owner;
        Type = type;
        AssociatedData = associatedData;

        Markup = new Markup(this);
      }

      /// <summary> The collection that owns this node. </summary>
      public MarkupCollection Owner { get; }

      /// <summary> The actual Markup instance associated with this node. </summary>
      public Markup Markup { get; }

      /// <summary> The type of markup that this span represents. </summary>
      public MarkupType Type { get; }

      /// <summary> Any additional data associated with the markup. </summary>
      public object AssociatedData { get; }

      #region Externally modified

      /// <summary> The linked list node associated with the SubFragmentNode. </summary>
      [NotNull]
      public LinkedListNode<MarkupNodeReference> Node { get; set; }

      /// <summary> The gap between this node and the previous node. </summary>
      public int RelativeOffsetSinceLastNode { get; set; }

      /// <summary> The number of elements that this markup node covers. </summary>
      public int Length { get; set; }

      /// <summary>
      ///  True if this node has been processed by the associated <see cref="MarkupType"/>
      ///  since the last update to its range.
      /// </summary>
      public bool IsValid { get; set; }

      #endregion

      /// <summary>
      ///  Calculates the AbsoluteIndex for this node, by iterating through all nodes until we get to
      ///  ourselves.
      /// </summary>
      /// <returns> The calculated absolute index. </returns>
      public int CalculateAbsoluteIndex()
      {
        int offset = 0;

        foreach (var node in Owner._subFragments)
        {
          offset += node.RelativeOffsetSinceLastNode;
          if (node == this)
          {
            break;
          }
        }

        return offset;
      }
    }

    /// <summary>
    ///  Return value of <see cref="MarkupCollection.GetAffectedNodes"/> containing the first node that changed and
    ///  the last node that changed.
    /// </summary>
    private struct FoundNode
    {
      private int _absoluteIndexOfFirstFoundNode;

      private LinkedListNode<MarkupNodeReference> _firstFoundNode;
      private LinkedListNode<MarkupNodeReference> _lastFoundNode;

      public LinkedListNode<MarkupNodeReference> RemainingNode { get; private set; }

      /// <summary> Remembers the given node as the first changed node (of no-others have been found) and the last changed node. </summary>
      /// <param name="absoluteIndex"> The absolute offset of the node. </param>
      /// <param name="node"> The node to remember. </param>
      public void RememberFirstAndLast(int absoluteIndex, LinkedListNode<MarkupNodeReference> node)
      {
        if (_firstFoundNode == null)
        {
          _firstFoundNode = node;
          _absoluteIndexOfFirstFoundNode = absoluteIndex;
        }
        _lastFoundNode = node;
      }

      /// <summary>
      ///  Marks the node as the first node that is not directly affected by the change, but rather is
      ///  after the change.
      /// </summary>
      public void MarkRemaining(LinkedListNode<MarkupNodeReference> theNode)
      {
        RemainingNode = theNode;
      }

      /// <summary> Gets an iterator that goes through all of the affected fragments. </summary>
      /// <returns>
      ///  An enumerator that allows foreach to be used to process the affected fragments in this
      ///  collection.
      /// </returns>
      public IEnumerable<FragmentAndOffset> GetAffectedFragments()
      {
        var current = _firstFoundNode;

        // NOTE: we bail out early if we have zero nodes
        if (current == null)
          yield break;

        int totalAbsoluteOffset = _absoluteIndexOfFirstFoundNode - current.Value.RelativeOffsetSinceLastNode;

        while (current != _lastFoundNode)
        {
          Debug.Assert(current != null,
                       "Between _firstFoundNode & _lastFoundNode was a null node somehow.  This is bad");

          int offsetSnapshot = current.Value.RelativeOffsetSinceLastNode;
          totalAbsoluteOffset += offsetSnapshot;
          yield return new FragmentAndOffset(current.Value, totalAbsoluteOffset);

          // it's possible that the caller changed the relative offset by the time we return.  We always
          // provide the "current" absolute index as it is when we yield it, therefore we need to include
          // any changes that the caller changed.
          // 
          // Note that if the caller changes the offset of an earlier node (e.g. not the node that was
          // just yielded) then we unfortunately do not track that (and we couldn't unless we wanted to
          // re-iterate the entire collection) 
          if (offsetSnapshot != current.Value.RelativeOffsetSinceLastNode)
          {
            var offsetDifference = offsetSnapshot - current.Value.RelativeOffsetSinceLastNode;
            totalAbsoluteOffset -= offsetDifference;
          }

          current = current.Next;
        }

        Debug.Assert(current != null,
                     "This doesn't even make sense");

        totalAbsoluteOffset += current.Value.RelativeOffsetSinceLastNode;
        yield return new FragmentAndOffset(current.Value, totalAbsoluteOffset);
      }
    }

    /// <summary>
    ///  Contains a single SubFragmentNode and its absolute offset. Used by
    ///  <see cref="GetAffectedNodes"/>
    /// </summary>
    private struct FragmentAndOffset
    {
      public FragmentAndOffset(MarkupNodeReference node, int absoluteOffset)
      {
        Node = node;
        Range = new TextRange(absoluteOffset, absoluteOffset + Node.Length);
      }

      public readonly MarkupNodeReference Node;
      public readonly TextRange Range;
    }
  }
}