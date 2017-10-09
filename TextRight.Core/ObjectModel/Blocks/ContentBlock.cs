using System;
using System.Collections.Generic;
using System.Linq;
using TextRight.Core.Cursors;
using TextRight.Core.ObjectModel.Cursors;

namespace TextRight.Core.ObjectModel.Blocks
{
  /// <summary>
  ///  A block that contains content instead of blocks, and thus supports cursors through the
  ///  content.
  /// </summary>
  public abstract class ContentBlock : Block
  {
    public abstract BlockCaret GetCaretAtStart();
    public abstract BlockCaret GetCaretAtEnd();
  }
}