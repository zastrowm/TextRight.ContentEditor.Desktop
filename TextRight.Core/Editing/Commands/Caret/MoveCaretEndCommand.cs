using System;
using System.Collections.Generic;
using System.Linq;
using TextRight.Core.ObjectModel.Blocks.Text;
using TextRight.Core.ObjectModel.Blocks.Text.View;
using TextRight.Core.ObjectModel.Cursors;

namespace TextRight.Core.Editing.Commands.Caret
{
  /// <summary> Moves the caret to the end of the current line. </summary>
  public class MoveCaretEndCommand : CaretCommand
  {
    /// <inheritdoc />
    public override string Id
      => "caret.moveEnd";

    /// <inheritdoc />
    protected override bool ShouldPreserveCaretMovementMode
      => true;

    public override bool CanActivate(DocumentEditorContext context)
      => context.Caret.Is<TextCaret>();

    /// <inheritdoc/>
    public override bool Activate(DocumentCursor cursor, CaretMovementMode movementMode)
    {
      var textCaret = cursor.Caret.As<TextCaret>();

      movementMode.SetModeToEnd();
      if (!(textCaret.Fragment.Owner.Target is ILineBasedRenderer lineBasedRenderer))
        return false;

      textCaret = lineBasedRenderer.GetLineFor(textCaret).FindClosestTo(double.MaxValue);
      cursor.MoveTo(textCaret);
      return true;
    }
  }
}