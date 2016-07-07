using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using TextRight.ContentEditor.Core.ObjectModel.Cursors;
using TextRight.ContentEditor.Core.ObjectModel.Serialization;
using TextRight.ContentEditor.Core.Utilities;

namespace TextRight.ContentEditor.Core.ObjectModel.Blocks
{
  /// <summary>
  ///  Represents the base class for both <see cref="BlockCollection"/> and
  ///  <see cref="ContentBlock"/>.
  /// </summary>
  public abstract class Block
  {
    /// <summary> Default constructor. </summary>
    internal Block()
    {
    }

    /// <summary>
    ///   The index of the block within the parent's collection.
    /// </summary>
    public int Index { get; set; }

    /// <summary>
    ///   The block that owns this block.
    /// </summary>
    public BlockCollection Parent { get; set; }

    /// <summary> True if the block is the first child of the parent collection. </summary>
    public bool IsFirst
      => Parent == null || Index == 0;

    /// <summary> True if the block is the last child of the parent collection. </summary>
    public bool IsLast
      => Parent == null || GetNextBlock() == null;

    /// <summary> Get the previous block in the block collection. </summary>
    public Block GetPreviousBlock()
      => PreviousBlock;

    /// <summary> Get the next block in the block collection. </summary>
    public Block GetNextBlock()
      => NextBlock;

    /// <summary> The type of the block. </summary>
    public abstract BlockType BlockType { get; }

    /// <summary> The mimetype of the content within the block.  Can be null. </summary>
    [CanBeNull]
    public abstract string MimeType { get; }

    /// <summary> Get the path down to this block in the document. </summary>
    /// <returns> The path to the block in the hierarchy. </returns>
    public BlockPath GetBlockPath()
    {
      return new BlockPath(this);
    }

    /// <summary> Makes a deep copy of this instance. </summary>
    /// <returns> A copy of this instance. </returns>
    public abstract Block Clone();

    /// <summary> Serializes this object into a node. </summary>
    /// <returns> A node that serializes the block.. </returns>
    public abstract SerializeNode SerializeAsNode();

    /// <summary>
    ///  Retrieves the block that comes after this block in the parent collection.
    /// </summary>
    internal Block NextBlock { get; set; }

    /// <summary>
    ///  Retrieves the block that comes before this block in the parent collection.
    /// </summary>
    internal Block PreviousBlock { get; set; }

    /// <summary> Gets the bounds of the block. </summary>
    /// <returns> The bounds that encompass the area consumed by the block. </returns>
    public abstract MeasuredRectangle GetBounds();

    /// <summary>
    ///  Retrieves a caret within the block that represents the given
    ///  CaretMovementMode as if a cursor with the given mode was arriving from
    ///  the top of the block.
    ///  
    ///  For example, for a CaretMovementMode with a Mode of
    ///  <see cref="CaretMovementMode.Mode.Position"/>
    ///  and a textblock, the caret should represent a caret that is
    ///  <see cref="CaretMovementMode.Position"/> units from the left-side of the
    ///  text on the first line in the text block.
    /// </summary>
    /// <seealso cref="GetCaretFromTop"/>
    /// <param name="caretMovementMode"> The caret movement mode. </param>
    /// <returns> The given caret. </returns>
    public abstract IBlockContentCursor GetCaretFromBottom(CaretMovementMode caretMovementMode);

    /// <summary>
    ///  Retrieves a caret within the block that represents the given
    ///  CaretMovementMode as if a cursor with the given mode was arriving from
    ///  the bottom of the block.
    /// </summary>
    /// <seealso cref="GetCaretFromTop"/>
    /// <param name="caretMovementMode"> The caret movement mode. </param>
    /// <returns> The given caret. </returns>
    public abstract IBlockContentCursor GetCaretFromTop(CaretMovementMode caretMovementMode);
  }
}