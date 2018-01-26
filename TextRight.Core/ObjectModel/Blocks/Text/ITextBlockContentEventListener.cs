using System;
using System.Collections.Generic;
using System.Linq;

namespace TextRight.Core.ObjectModel.Blocks.Text
{
  /// <summary> Handles events from a <see cref="TextBlockContent"/> instance. </summary>
  public interface ITextBlockContentEventListener : IEventListener
  {
    /// <summary> Notifies the receiver that a fragment has been inserted. </summary>
    /// <param name="previousSibling"> The fragment that precedes the new fragment. </param>
    /// <param name="newSpan"> The fragment that is inserted. </param>
    /// <param name="nextSibling"> The fragment that comes after the block that is being inserted. </param>
    void NotifyFragmentInserted(TextSpan previousSibling,
                                TextSpan newSpan,
                                TextSpan nextSibling);

    /// <summary> Notifies the receiver that a fragment has been inserted. </summary>
    /// <param name="previousSibling"> The fragment that precedes the new fragment. </param>
    /// <param name="removedSpan"> The fragment that was removed. </param>
    /// <param name="nextSibling"> The fragment that comes after the block that is being inserted. </param>
    void NotifyFragmentRemoved(TextSpan previousSibling,
                               TextSpan removedSpan,
                               TextSpan nextSibling);

    /// <summary>
    ///  Notifies the receiver that the text in the given fragment has been changed.
    /// </summary>
    /// <param name="changedSpan"> The changed fragment. </param>
    void NotifyTextChanged(TextSpan changedSpan);
  }
}