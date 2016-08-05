using System;
using System.Collections.Generic;
using System.Linq;
using TextRight.ContentEditor.Core.ObjectModel.Cursors;

namespace TextRight.ContentEditor.Core.ObjectModel.Blocks
{
  /// <summary> Extension methods to <see cref="IBlockContentCursor"/> </summary>
  public static class BlockContentCursorExtensions
  {
    /// <summary>
    ///  Fluent Api: Move the cursor to the end of the block.
    /// </summary>
    public static TCursor ToEnd<TCursor>(this TCursor cursor)
      where TCursor : IBlockContentCursor
    {
      cursor.MoveToEnd();
      return cursor;
    }

    /// <summary>
    ///  Fluent Api: Move the cursor to the beginning of the block.
    /// </summary>
    public static TCursor ToBeginning<TCursor>(this TCursor cursor)
      where TCursor : IBlockContentCursor
    {
      cursor.MoveToBeginning();
      return cursor;
    }

    /// <summary>
    ///  Creates a copy of the cursor that can be modified independently from the original.
    /// </summary>
    /// <param name="cursor"> The cursor to act on. </param>
    /// <returns> A copy of the given cursor. </returns>
    public static CursorCopy Copy(this IBlockContentCursor cursor)
    {
      var copied = cursor.CursorPool.GetCursorCopy(cursor.Block);
      copied.Cursor.MoveTo(cursor);
      return copied;
    }

  }
}