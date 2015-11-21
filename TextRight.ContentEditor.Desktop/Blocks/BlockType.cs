using System;
using System.Collections.Generic;
using System.Linq;

namespace TextRight.ContentEditor.Desktop.Blocks
{
  /// <summary> The various types of block. </summary>
  public enum BlockType
  {
    /// <summary> A block that contains one or more other blocks. </summary>
    ContainerBlock,

    /// <summary> A block that contains only text. </summary>
    TextBlock,
  }
}