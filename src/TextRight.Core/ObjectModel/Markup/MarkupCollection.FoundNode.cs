using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace TextRight.Core.ObjectModel
{
  public partial class MarkupCollection
  {
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
  }
}