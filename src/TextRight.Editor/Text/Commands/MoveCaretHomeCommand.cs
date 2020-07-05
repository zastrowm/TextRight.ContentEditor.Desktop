using System;
using System.Collections.Generic;
using System.Linq;
using TextRight.Core;
using TextRight.Core.Commands.Caret;
using TextRight.Core.ObjectModel.Blocks.Text;
using TextRight.Core.ObjectModel.Cursors;

namespace TextRight.Editor.Text.Commands
{
  /// <summary> Moves the caret to the beginning of the line. </summary>
  public class MoveCaretHomeCommand : CaretCommand
  {
    /// <inheritdoc />
    public override string Id
      => "caret.moveHome";

    /// <inheritdoc />
    protected override bool ShouldPreserveCaretMovementMode
      => true;

    public override bool CanActivate(DocumentEditorContext context)
      => context.Selection.Is<TextCaret>();

    /// <inheritdoc/>
    public override bool Activate(DocumentSelection cursor, CaretMovementMode movementMode, SelectionMode mode)
    {
      var textCaret = cursor.Start.As<TextCaret>();

      movementMode.SetModeToHome();
      var contentView = textCaret.Block.GetViewOrNull<ITextBlockView>();
      if (contentView == null)
        return false;

      textCaret = contentView.GetLineFor(textCaret).FindClosestTo(0);
      cursor.MoveTo(textCaret, mode);
      return true;
    }
  }
}