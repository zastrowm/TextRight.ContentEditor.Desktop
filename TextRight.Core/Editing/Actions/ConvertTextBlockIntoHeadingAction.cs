﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TextRight.Core.ObjectModel.Blocks;
using TextRight.Core.ObjectModel.Blocks.Text;
using TextRight.Core.ObjectModel.Serialization;

namespace TextRight.Core.Editing.Actions
{
  /// <summary> Converts a TextBlock into a heading. </summary>
  public class ConvertTextBlockIntoHeadingAction : UndoableAction
  {
    private readonly DocumentCursorHandle _handle;
    private readonly int _level;
    private int? _originalHeadingLevel;

    private readonly BlobSerializedData _blobSerializedData;
    private readonly BlockDescriptor _originalDescriptor;

    /// <summary> Constructor. </summary>
    public ConvertTextBlockIntoHeadingAction(ReadonlyCursor cursor, int level)
    {
      _handle = cursor;
      _level = level;

      var block = (TextBlock)cursor.Block;

      var headingBlock = block as HeadingBlock;
      if (headingBlock != null)
      {
        _originalHeadingLevel = headingBlock.HeadingLevel;
      }
      else
      {
        _originalDescriptor = block.DescriptorHandle.Descriptor;
        _blobSerializedData = block.SerializeProperties();
      }
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
        var block = copy.Block;

        if (_originalHeadingLevel != null)
        {
          // optimization; if it's already a heading block, we just have to change the level
          ((HeadingBlock)block).HeadingLevel = _level;
        }
        else
        {
          Replace(context,
                  (TextBlock)block,
                  new HeadingBlock()
                  {
                    HeadingLevel = _level
                  });
        }
      }
    }

    /// <inheritdoc />
    public override void Undo(DocumentEditorContext context)
    {
      using (var copy = _handle.Get(context))
      {
        if (_originalHeadingLevel != null)
        {
          // optimization; if it's already a heading block, we just have to change the level
          ((HeadingBlock)copy.Block).HeadingLevel = _originalHeadingLevel.Value;
        }
        else
        {
          var original = (TextBlock)_originalDescriptor.CreateInstance();
          original.DeserializeProperties(_blobSerializedData);
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
      blockToReplace.MoveTextInto(newBlock);
      blockToReplace.Parent.Replace(blockToReplace, newBlock);
      context.Caret.MoveTo(_handle, context);
    }
  }
}