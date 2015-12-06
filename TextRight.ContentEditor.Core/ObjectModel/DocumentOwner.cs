using System;
using System.Collections.Generic;
using System.Linq;
using TextRight.ContentEditor.Desktop.ObjectModel.Blocks;

namespace TextRight.ContentEditor.Desktop.Blocks
{
  /// <summary> Represents a single TextRight document. </summary>
  public class DocumentOwner
  {
    /// <summary> Default constructor. </summary>
    public DocumentOwner()
    {
      Root = new BlockCollection();
    }

    /// <summary> The top level collection of elements.  </summary>
    public BlockCollection Root { get; }
  }
}