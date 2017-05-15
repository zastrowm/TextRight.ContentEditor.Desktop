using System;
using System.Collections.Generic;
using System.Linq;
using TextRight.Core.ObjectModel.Blocks.Text;

namespace TextRight.Core.Cursors
{
  /// <summary> Helper methods for interacting with carets.D </summary>
  public static class CaretHelpers
  {
    public static bool TryMoveForward(ref TextCaret caret)
    {
      TextCaret maybe = caret.GetNextPosition();
      if (maybe.IsValid)
      {
        caret = maybe;
        return true;
      }

      return false;
    }

    public static bool TryMoveBackward(ref TextCaret caret)
    {
      TextCaret maybe = caret.GetPreviousPosition();
      if (maybe.IsValid)
      {
        caret = maybe;
        return true;
      }

      return false;
    }
  }
}