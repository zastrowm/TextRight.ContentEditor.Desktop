using System;
using System.Collections.Generic;
using System.Linq;

namespace TextRight.ContentEditor.Core.ObjectModel.Blocks
{
  /// <summary> Extension methods to <see cref="IBlockContentCursor"/> </summary>
  public static class BlockContentCursorExtensions
  {
    /// <summary>
    ///  Fluent Api: Move the cursor to the end of the block.
    /// </summary>
    public static IBlockContentCursor ToEnd(this IBlockContentCursor cursor)
    {
      cursor.MoveToEnd();
      return cursor;
    }

    /// <summary>
    ///  Fluent Api: Move the cursor to the beginning of the block.
    /// </summary>
    public static IBlockContentCursor ToBeginning(this IBlockContentCursor cursor)
    {
      cursor.MoveToBeginning();
      return cursor;
    }
  }
}