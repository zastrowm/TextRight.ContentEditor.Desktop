using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using TextRight.Core.Editing.Actions;
using TextRight.Core.Editing.Actions.Text;
using TextRight.Core.ObjectModel.Blocks;
using TextRight.Core.ObjectModel.Blocks.Text;

namespace TextRight.Core.Editing.Commands.Text
{
  /// <summary> Merges two text blocks together so that they form a single text block. </summary>
  public class MergeTextBlocksCommand : IContextualCommand
  {
    /// <inheritdoc />
    public string Id
      => "block.merge";

    /// <inheritdoc />
    public string GetName(DocumentEditorContext context)
    {
      if (context.Cursor.IsAtBlockStart)
        return "Merge paragraph backwards";
      else
        return "Merge paragraph forwards";
    }

    /// <inheritdoc />
    public string GetDescription(DocumentEditorContext context)
    {
      return "Merge block with previous or next block";
    }

    /// <inheritdoc />
    public bool CanActivate(DocumentEditorContext context)
    {
      TextBlock unused1;
      TextBlock unused2;

      return TryGetTextBlocks(context, out unused1, out unused2);
    }

    /// <inheritdoc />
    public void Activate(DocumentEditorContext context, IActionStack actionStack)
    {
      TextBlock previous;
      TextBlock next;

      if (!TryGetTextBlocks(context, out previous, out next))
      {
        return;
      }

      actionStack.Do(new MergeTextBlockAction(previous, next, context.Caret));
    }

    /// <summary>
    ///  Attempts to retrieve the previous and next text block for the caret at the current position.
    /// </summary>
    private bool TryGetTextBlocks(DocumentEditorContext context, out TextBlock previous, out TextBlock next)
    {
      var cursor = context.Cursor;

      if (cursor.IsAtBlockStart && cursor.Block.PreviousBlock != null)
      {
        previous = cursor.Block.PreviousBlock as TextBlock;
        next = cursor.Block as TextBlock;
      }
      else if (cursor.IsAtBlockEnd && cursor.Block.NextBlock != null)
      {
        previous = cursor.Block as TextBlock;
        next = cursor.Block.NextBlock as TextBlock;
      }
      else
      {
        previous = null;
        next = null;
      }

      return previous != null && next != null;
    }

    /// <summary> Deletes text from the document. </summary>
    private class MergeTextBlockAction : UndoableAction
    {
      private readonly DocumentCursorHandle _originalCaretPosition;
      private readonly BlockPath _previousPath;
      private readonly BlockPath _nextPath;
      private readonly DocumentCursorHandle _endOfPreviousBlockHandle;

      /// <summary> Constructor. </summary>
      /// <param name="previousBlock"> The previous block that will have <paramref name="nextBlock"/>
      ///  merged into. </param>
      /// <param name="nextBlock"> The next block that will be merged into
      ///  <paramref name="previousBlock"/>. </param>
      /// <param name="originalCaretPosition"> The position at which the caret is at. </param>
      public MergeTextBlockAction(TextBlock previousBlock, TextBlock nextBlock, DocumentCursorHandle originalCaretPosition)
      {
        Debug.Assert(previousBlock.NextBlock == nextBlock);

        _originalCaretPosition = originalCaretPosition;
        _previousPath = previousBlock.GetBlockPath();
        _nextPath = nextBlock.GetBlockPath();

        _endOfPreviousBlockHandle = new DocumentCursorHandle(previousBlock.Content.GetCaretAtEnd());
      }

      /// <inheritdoc />
      public override string Name
        => "Merge blocks";

      /// <inheritdoc />
      public override string Description
        => "Merges paragraphs together";

      /// <inheritdoc />
      public override void Do(DocumentEditorContext context)
      {
        var next = _nextPath.Get(context.Document);

        TextBlockHelperMethods.MergeWithPrevious((TextBlock)next);
        context.Caret.MoveTo(_endOfPreviousBlockHandle.Get(context));
      }

      /// <inheritdoc />
      public override void Undo(DocumentEditorContext context)
      {
        var caret = (TextCaret)_endOfPreviousBlockHandle.GetCaret(context);

        TextBlockHelperMethods.TryBreakBlock(caret);

        context.Caret.MoveTo(_originalCaretPosition.Get(context));
    }
    }
  }
}