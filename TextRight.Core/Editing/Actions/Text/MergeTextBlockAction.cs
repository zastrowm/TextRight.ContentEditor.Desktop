using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using TextRight.Core.ObjectModel.Blocks;
using TextRight.Core.ObjectModel.Blocks.Text;

namespace TextRight.Core.Editing.Actions.Text
{
  /// <summary> Deletes text from the document. </summary>
  public class MergeTextBlockAction : UndoableAction
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

      _endOfPreviousBlockHandle = new DocumentCursorHandle(previousBlock.GetCursor().ToEnd());
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
      using (var breakSpotCopy = _endOfPreviousBlockHandle.Get(context))
      {
        var breakSpot = breakSpotCopy.Cursor;

        var previousBlock = _previousPath.Get(context.Document);
        TextBlockHelperMethods.TryBreakBlock((TextBlockCursor)breakSpot);

        context.Caret.MoveTo(_originalCaretPosition.Get(context));
      }
    }
  }
}