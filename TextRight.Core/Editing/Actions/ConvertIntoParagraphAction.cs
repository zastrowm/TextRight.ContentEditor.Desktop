using System;
using System.Collections.Generic;
using System.Linq;
using TextRight.Core.ObjectModel.Blocks;
using TextRight.Core.ObjectModel.Blocks.Text;
using TextRight.Core.ObjectModel.Serialization;

namespace TextRight.Core.Editing.Actions
{
  /// <summary> Converts a TextBlock into a normal paragraph. </summary>
  public class ConvertIntoParagraphAction : UndoableAction
  {
    private readonly DocumentCursorHandle _handle;
    private readonly BlobSerializedData _blobSerializedData;
    private readonly BlockDescriptor _originalDescriptor;

    public ConvertIntoParagraphAction(ReadonlyCursor cursor)
    {
      _handle = cursor;

      var block = (TextBlock)cursor.Block;
      _originalDescriptor = block.DescriptorHandle.Descriptor;
      _blobSerializedData = block.SerializeProperties();
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
      using (var copy = _handle.Get(context))
      {
        Replace(context,
                (TextBlock)copy.Block,
                new ParagraphBlock());
      }
    }

    /// <inheritdoc />
    public override void Undo(DocumentEditorContext context)
    {
      using (var copy = _handle.Get(context))
      {
        var original = (TextBlock)_originalDescriptor.CreateInstance();
        original.DeserializeProperties(_blobSerializedData);
        Replace(context, (TextBlock)copy.Block, original);
      }
    }

    /// <summary>
    ///  Replaces the given block in the document with the new block, transferring all contents of the
    ///  block and adjusting the document cursor to point to the location where it was pointing
    ///  originally.
    /// </summary>
    private void Replace(DocumentEditorContext context, TextBlock blockToReplace, TextBlock newBlock)
    {
      blockToReplace.MoveTextInto(newBlock);
      blockToReplace.Parent.Replace(blockToReplace, newBlock);
      context.Caret.MoveTo(_handle, context);
    }
  }
}