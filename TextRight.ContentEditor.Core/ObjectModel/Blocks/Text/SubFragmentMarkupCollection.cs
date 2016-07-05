using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace TextRight.ContentEditor.Core.ObjectModel.Blocks
{
  public struct SubFragmentMarkupObjectSnapshot
  {
    private readonly int _absoluteIndex;
    private readonly SubFragmentMarkupCollection.SubFragmentNode _node;

    internal SubFragmentMarkupObjectSnapshot(int absoluteIndex, SubFragmentMarkupCollection.SubFragmentNode node)
    {
      _absoluteIndex = absoluteIndex;
      _node = node;
    }

    public TextRange Range
      => new TextRange(_absoluteIndex, _absoluteIndex + _node.Length);
  }

  /// <summary> Contains a collection of SubFragmentMarkups for a specific <see cref="StyledTextFragment"/>. </summary>
  public class SubFragmentMarkupCollection : IEnumerable<SubFragmentMarkupObjectSnapshot>
  {
    // at some point we may want to consider using some form of an Interval Tree but for now our
    // current architecture is acceptable. (http:// en.wikipedia.org/wiki/Interval_tree) 

    private readonly StyledTextFragment _owner;
    private readonly LinkedList<SubFragmentNode> _subFragments;

    public SubFragmentMarkupCollection(StyledTextFragment owner)
    {
      _owner = owner;
      _subFragments = new LinkedList<SubFragmentNode>();
    }

    /// <summary> Marks the given range to have the given markup and data. </summary>
    /// <exception cref="ArgumentOutOfRangeException"> Thrown when one or more arguments are outside
    ///  the required range. </exception>
    /// <param name="range"> The range of text to mark. </param>
    /// <param name="type"> The type of markup to apply to the range. </param>
    /// <param name="additionalData"> Any additional data to store with the markup. </param>
    public void MarkRange(TextRange range, SubFragmentMarkupType type, object additionalData)
    {
      if (range.StartIndex < 0)
        throw new ArgumentOutOfRangeException(nameof(range));
      if (range.EndIndex > _owner.Length)
        throw new ArgumentOutOfRangeException(nameof(range));

      var newFragment = new SubFragmentNode(type, additionalData)
                        {
                          Length = range.Length
                        };

      int previousNodeAbsoluteIndex;
      var previousNode = GetClosestNodeBefore(range.StartIndex, out previousNodeAbsoluteIndex);

      if (previousNode == null)
      {
        var nextNode = _subFragments.First;

        if (nextNode != null)
        {
          // because we're at the beginning, the offset of the next fragment was an absolute index, thus
          // we now convert it to a relative index by offsetting our start index. 
          nextNode.Value.RelativeOffsetSinceLastNode -= range.StartIndex;
        }

        _subFragments.AddFirst(newFragment);
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

        _subFragments.AddAfter(previousNode, newFragment);
      }
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
    }

    /// <summary> Gets the node closest to the given index. </summary>
    /// <param name="index"> The index to which we're trying to get the closest previous node. </param>
    /// <param name="absoluteIndexOfPreviousNode"> [out] The absolute index of the node that was found
    ///  (if one was found). </param>
    /// <returns> The closest node that does not surpass the given index. </returns>
    private LinkedListNode<SubFragmentNode> GetClosestNodeBefore(int index, out int absoluteIndexOfPreviousNode)
    {
      absoluteIndexOfPreviousNode = 0;
      LinkedListNode<SubFragmentNode> currentNode = _subFragments.First;
      LinkedListNode<SubFragmentNode> lastNode = null;

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

    private void Invalidate(SubFragmentNode value)
    {
      // TODO notify callers that this fragment needs to be updated
      value.IsValid = false;
    }

    /// <summary> Finds a series of nodes that are affected by a change at the given index. </summary>
    /// <param name="indexOfChange"> The index at which a change is being applied </param>
    /// <returns> The nodes that are affected by the change. </returns>
    private FoundNode FindAffectedNodes(TextRange range)
    {
      LinkedListNode<SubFragmentNode> listNode = _subFragments.First;
      int absoluteIndexOfLastSubFragment = 0;

      FoundNode foundNode = new FoundNode();

      while (listNode != null)
      {
        SubFragmentNode subFragment = listNode.Value;

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
      var findings = FindAffectedNodes(new TextRange(changeEvent.Index, changeEvent.Index));

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
      var findings = FindAffectedNodes(deletionRange);

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

    public IEnumerator<SubFragmentMarkupObjectSnapshot> GetEnumerator()
    {
      int absoluteIndex = 0;

      foreach (var item in _subFragments)
      {
        absoluteIndex += item.RelativeOffsetSinceLastNode;
        yield return new SubFragmentMarkupObjectSnapshot(absoluteIndex, item);
      }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
      return GetEnumerator();
    }

    internal class SubFragmentNode
    {
      public SubFragmentNode(SubFragmentMarkupType type, object associatedData)
      {
        Type = type;
        AssociatedData = associatedData;
      }

      public int RelativeOffsetSinceLastNode { get; set; }

      public int Length { get; set; }

      public bool IsValid { get; set; }

      /// <summary> The type of markup that this span represents. </summary>
      public SubFragmentMarkupType Type { get; private set; }

      /// <summary> Any additional data associated with the markup. </summary>
      public object AssociatedData { get; private set; }
    }

    /// <summary>
    ///  Return value of <see cref="FindAffectedNodes"/> containing the first node that changed and
    ///  the last node that changed.
    /// </summary>
    private struct FoundNode
    {
      private int _absoluteIndexOfFirstFoundNode;

      private LinkedListNode<SubFragmentNode> _firstFoundNode;

      private LinkedListNode<SubFragmentNode> _lastFoundNode;

      public LinkedListNode<SubFragmentNode> RemainingNode { get; private set; }

      /// <summary> Remembers the given node as the first changed node (of no-others have been found) and the last changed node. </summary>
      /// <param name="absoluteIndex"> The absolute offset of the node. </param>
      /// <param name="node"> The node to remember. </param>
      public void RememberFirstAndLast(int absoluteIndex, LinkedListNode<SubFragmentNode> node)
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
      public void MarkRemaining(LinkedListNode<SubFragmentNode> theNode)
      {
        RemainingNode = theNode;
      }

      /// <summary> Gets an iterator that goes through all of the affected fragments. </summary>
      public IEnumerable<FragmentAndOffset> GetAffectedFragments()
      {
        var current = _firstFoundNode;

        if (current != null)
        {
          int currentOffset = _absoluteIndexOfFirstFoundNode - current.Value.RelativeOffsetSinceLastNode;

          while (current != _lastFoundNode)
          {
            int offset = current.Value.RelativeOffsetSinceLastNode;
            currentOffset += offset;
            yield return new FragmentAndOffset(current.Value, currentOffset);

            if (offset != current.Value.RelativeOffsetSinceLastNode)
            {
              currentOffset -= (offset - current.Value.RelativeOffsetSinceLastNode);
            }

            current = current.Next;
          }

          currentOffset += current.Value.RelativeOffsetSinceLastNode;
          yield return new FragmentAndOffset(current.Value, currentOffset);
        }
      }
    }

    /// <summary> Contains a single SubFragmentNode and its absolute offset. </summary>
    private struct FragmentAndOffset
    {
      public FragmentAndOffset(SubFragmentNode node, int absoluteOffset)
      {
        Node = node;
        Range = new TextRange(absoluteOffset, absoluteOffset + Node.Length);
      }

      public readonly SubFragmentNode Node;
      public readonly TextRange Range;
    }
  }
}