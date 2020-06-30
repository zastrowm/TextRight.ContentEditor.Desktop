using System;
using System.Collections.Generic;
using System.Linq;
using TextRight.Core.ObjectModel.Blocks.Text;
using TextRight.Core.ObjectModel.Blocks.Text.View;
using TextRight.Core.ObjectModel.Cursors;

namespace TextRight.Core.Commands.Caret
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
      => context.Selection.Is<TextCaret>();

    /// <inheritdoc/>
    public override bool Activate(DocumentSelection cursor, CaretMovementMode movementMode, SelectionMode mode)
    {
      var textCaret = cursor.Start.As<TextCaret>();

      movementMode.SetModeToEnd();
      if (!(textCaret.Content.Target is ITextBlockContentView contentView))
        return false;

      textCaret = contentView.GetLineFor(textCaret).FindClosestTo(double.MaxValue);
      cursor.MoveTo(textCaret, mode);
      return true;
    }
  }
}