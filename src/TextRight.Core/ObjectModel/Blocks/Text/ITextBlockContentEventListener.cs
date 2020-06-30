using System;
using System.Collections.Generic;
using System.Linq;

namespace TextRight.Core.ObjectModel.Blocks.Text
{
  /// <summary> Handles events from a <see cref="TextBlockContent"/> instance. </summary>
  public interface ITextBlockContentEventListener : IEventListener
  {
    /// <summary>
    ///  Notifies the receiver that the text in the given content has been changed.
    /// </summary>
    /// <param name="changedContent"> The changed content. </param>
    void NotifyTextChanged(TextBlockContent changedContent);
  }
}