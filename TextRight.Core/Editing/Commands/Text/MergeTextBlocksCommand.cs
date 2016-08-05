using System;
using System.Collections.Generic;
using System.Linq;
using TextRight.ContentEditor.Core.Editing.Actions;
using TextRight.ContentEditor.Core.ObjectModel.Blocks;

namespace TextRight.ContentEditor.Core.Editing
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
      if (context.Cursor.IsAtBeginning)
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
    public void Activate(DocumentEditorContext context, ActionStack actionStack)
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

      if (cursor.IsAtBeginning && cursor.Block.PreviousBlock != null)
      {
        previous = cursor.Block.PreviousBlock as TextBlock;
        next = cursor.Block as TextBlock;
      }
      else if (cursor.IsAtEnd && cursor.Block.NextBlock != null)
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
  }
}