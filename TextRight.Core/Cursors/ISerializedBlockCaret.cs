using System;
using System.Collections.Generic;
using System.Linq;
using TextRight.Core.Editing;

namespace TextRight.Core.Cursors
{
  /// <summary> Interface for serialized block caret. </summary>
  public interface ISerializedBlockCaret
  {
    /// <summary> Deserializes the stored data back into a caret. </summary>
    BlockCaret Deserialize(DocumentEditorContext context);
  }
}