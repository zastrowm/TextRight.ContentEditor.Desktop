﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using TextRight.Core.Actions;
using TextRight.Core.ObjectModel.Blocks.Text;
using TextRight.Core.ObjectModel.Cursors;

namespace TextRight.Core.Commands.Text
{
  /// <summary> Deletes the previous character in the document. </summary>
  public class DeletePreviousCharacterCommand : IContextualCommand
  {
    /// <inheritdoc />
    public string Id
      => "text.deletePreviousChar";

    /// <inheritdoc />
    public string GetName(DocumentEditorContext context)
      => "Delete previous character";

    /// <inheritdoc />
    public string GetDescription(DocumentEditorContext context)
      => "Deletes the previous character in the document";

    /// <inheritdoc />
    public bool CanActivate(DocumentEditorContext context)
    {
      var cursor = context.Selection.Start;
      return cursor.Is<TextCaret>() && !cursor.IsAtBlockStart;
    }

    /// <inheritdoc />
    public void Activate(DocumentEditorContext context, IActionStack actionStack)
    {
      var caret = (TextCaret)context.Selection.Start;
      actionStack.Do(new DeletePreviousCharacterAction(caret));
    }

    /// <summary> Deletes text from the document. </summary>
    internal class DeletePreviousCharacterAction : UndoableAction
    {
      public DeletePreviousCharacterAction(TextCaret caret)
      {
        Debug.Assert(caret.GetPreviousPosition().IsValid, "Caret previous position not valid");

        caret = caret.GetPreviousPosition();
        CursorHandle = new DocumentCursorHandle(caret);
        OriginalText = caret.CharacterAfter.Text;
      }

      /// <summary> The location at which the character was deleted. </summary>
      public DocumentCursorHandle CursorHandle { get; }

      /// <summary> The text that was removed. </summary>
      public string OriginalText { get; }

      /// <inheritdoc />
      public override string Name
        => "Delete Text";

      /// <inheritdoc />
      public override string Description
        => "Delete text from the document";

      /// <inheritdoc />
      public override void Do(DocumentEditorContext context)
      {
        var caret = (TextCaret)CursorHandle.GetCaret(context);
        caret = caret.DeleteText(1);
        context.Selection.Replace(caret);
      }

      /// <inheritdoc />
      public override void Undo(DocumentEditorContext context)
      {
        var caret = (TextCaret)CursorHandle.GetCaret(context);
        caret = caret.InsertText(OriginalText);
        context.Selection.Replace(caret);
      }
    }
  }
}