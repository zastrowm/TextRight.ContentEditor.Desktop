using System;
using System.Collections.Generic;
using System.Linq;
using TextRight.Core.ObjectModel.Blocks.Text;
using TextRight.Core.ObjectModel.Blocks.Text.View;
using TextRight.Core.ObjectModel.Cursors;
using TextRight.Editor.Text;

namespace TextRight.Core.Commands.Caret
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
      var contentView = textCaret.Block.GetViewOrNull<ITextBlockContentView>();
      if (contentView == null)
        return false;

      textCaret = contentView.GetLineFor(textCaret).FindClosestTo(0);
      cursor.MoveTo(textCaret, mode);
      return true;
    }
  }
}