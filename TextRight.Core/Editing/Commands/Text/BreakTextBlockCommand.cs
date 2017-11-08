using System;
using System.Collections.Generic;
using System.Linq;
using TextRight.Core.Actions;
using TextRight.Core.ObjectModel.Blocks.Text;

namespace TextRight.Core.Commands.Text
{
  /// <summary> Breaks a text-block into two. </summary>
  public class BreakTextBlockCommand : IContextualCommand
  {
    /// <inheritdoc />
    public string Id
      => "block.split";

    /// <inheritdoc />
    string IContextualCommand.GetName(DocumentEditorContext context)
    {
      return "Split Paragraph";
    }

    /// <inheritdoc />
    string IContextualCommand.GetDescription(DocumentEditorContext context)
    {
      return "Split block into two";
    }

    /// <inheritdoc />
    bool IContextualCommand.CanActivate(DocumentEditorContext context)
    {
      var textBlock = context.Caret.Block as TextBlock;
      // TODO check if the parent collection allows multiple children
      return textBlock != null;
    }

    /// <inheritdoc />
    void IContextualCommand.Activate(DocumentEditorContext context, IActionStack actionStack)
    {
      // TODO delete any text that is selected
      actionStack.Do(new BreakTextBlockAction(context.Selection));
    }

    /// <summary> Breaks a paragraph at the given caret location. </summary>
    internal class BreakTextBlockAction : UndoableAction
    {
      private readonly DocumentCursorHandle _handle;

      /// <summary> Constructor. </summary>
      /// <param name="handle"> The location at which the paragraph should be broken. </param>
      public BreakTextBlockAction(DocumentCursorHandle handle)
      {
        _handle = handle;
      }

      /// <inheritdoc />
      public override string Name { get; }
        = "Break Paragraph";

      /// <inheritdoc />
      public override string Description { get; }
        = "Break paragraph into two";

      /// <inheritdoc />
      public override void Do(DocumentEditorContext context)
      {
        var caret = (TextCaret)_handle.GetCaret(context);
        var caretPosition = TextBlockHelperMethods.TryBreakBlock(caret);
        if (!caretPosition.IsValid)
          return;
        context.Selection.MoveTo(caretPosition);
      }

      /// <inheritdoc />
      public override void Undo(DocumentEditorContext context)
      {
        var caret = (TextCaret)_handle.GetCaret(context);
        var previousBlock = caret.Block;
        var nextBlock = previousBlock.NextBlock;

        TextBlockHelperMethods.MergeWithPrevious((TextBlock)nextBlock);

        // move it to where it was when we wanted to break the paragraph.  It's safer to deserialize
        // again, as the cursor above is not guaranteed to be valid. 
        context.Selection.MoveTo(_handle, context);
      }
    }
  }
}