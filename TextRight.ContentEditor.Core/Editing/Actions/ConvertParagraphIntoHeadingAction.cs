using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TextRight.ContentEditor.Core.ObjectModel.Blocks;

namespace TextRight.ContentEditor.Core.Editing.Actions
{
  /// <summary> Converts a normal paragraph into a heading. </summary>
  public class ConvertParagraphIntoHeadingAction : UndoableAction
  {
    private readonly DocumentCursorHandle _handle;

    /// <summary> Constructor. </summary>
    public ConvertParagraphIntoHeadingAction(DocumentCursorHandle handle)
    {
      _handle = handle;
    }

    /// <inheritdoc />
    public override string Name
      => "Convert to Heading";

    /// <inheritdoc />
    public override string Description
      => "Convert paragraph into a heading";

    /// <inheritdoc />
    public override void Do(DocumentEditorContext context)
    {
      ReplaceCurrentTextBlockWith(context, new HeadingBlock());
    }

    /// <inheritdoc />
    public override void Undo(DocumentEditorContext context)
    {
      ReplaceCurrentTextBlockWith(context, new ParagraphBlock());
    }

    private void ReplaceCurrentTextBlockWith(DocumentEditorContext context, TextBlock newBlock)
    {
      using (var copy = _handle.Get(context))
      {
        // we really only need the block to extract the contents
        var cursor = (TextBlockCursor)copy.Cursor;
        var targetBlock = cursor.Block;

        targetBlock.MoveTextInto(newBlock);

        targetBlock.Parent.Replace(targetBlock, newBlock);

        context.Caret.MoveTo(_handle.Get(context));
      }
      
    }
  }
}