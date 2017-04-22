using System;
using System.Collections.Generic;
using System.Linq;
using TextRight.Core.Editing.Actions;
using TextRight.Core.ObjectModel.Blocks;
using TextRight.Core.ObjectModel.Blocks.Text;
using TextRight.Core.ObjectModel.Serialization;

namespace TextRight.Core.Editing.Actions
{
  /// <summary>
  ///  Base class for an action that converts a TextBlock to the given type of TextBlock.
  /// </summary>
  /// <typeparam name="TBlock"> Type of the block that the action is converting to. </typeparam>
  public abstract class ConvertTextBlockToTextBlockAction<TBlock> : UndoableAction
    where TBlock : TextBlock
  {
    private readonly DocumentCursorHandle _handle;

    private readonly BlobSerializedData _originalProperties;
    private readonly RegisteredDescriptor _originalDescriptor;

    /// <summary> Constructor. </summary>
    protected ConvertTextBlockToTextBlockAction(ReadonlyCursor cursor)
    {
      _handle = cursor;

      var block = (TextBlock)cursor.Block;

      _originalDescriptor = block.DescriptorHandle;
      _originalProperties = block.SerializeProperties();
    }

    public abstract RegisteredDescriptor GetDestinationDescriptor();

    /// <inheritdoc />
    public override void Do(DocumentEditorContext context)
    {
      using (var copy = _handle.Get(context))
      {
        var block = copy.Block;

        var destinationDescriptor = GetDestinationDescriptor();

        // optimization; if it's already the correct block type, we just have to change it in-place
        if (block.DescriptorHandle == destinationDescriptor)
        {
          MakeChangesTo((TBlock)block);
        }
        else
        {
          var newBlock = (TBlock)destinationDescriptor.Descriptor.CreateInstance();
          MakeChangesTo(newBlock);

          Replace(context,(TextBlock)block,newBlock);
        }
      }
    }

    /// <summary> Make the changes necessary to the given block to  </summary>
    /// <param name="block"> The block. </param>
    public abstract void MakeChangesTo(TBlock block);

    /// <inheritdoc />
    public override void Undo(DocumentEditorContext context)
    {
      using (var copy = _handle.Get(context))
      {
        var block = copy.Block;

        // optimization; if it was originally a block of the same type, we can just deserialize
        // but leave it in-place
        if (block.DescriptorHandle == _originalDescriptor)
        {
          var originalBlock = (TextBlock)block;
          originalBlock.DeserializeProperties(_originalProperties);
        }
        else
        {
          // otherwise we have to re-create it
          TextBlock original = (TextBlock)_originalDescriptor.CreateInstance();
          original.DeserializeProperties(_originalProperties);
          Replace(context, (TextBlock)copy.Block, original);
        }
      }
    }

    /// <summary>
    ///  Replaces the given block in the document with the new block, transferring all contents of the
    ///  block and adjusting the document cursor to point to the location where it was pointing
    ///  originally.
    /// </summary>
    private void Replace(DocumentEditorContext context, TextBlock blockToReplace, TextBlock newBlock)
    {
      newBlock.Content = blockToReplace.Content;
      blockToReplace.Parent.Replace(blockToReplace, newBlock);
      context.Caret.MoveTo(_handle, context);
    }
  }
}