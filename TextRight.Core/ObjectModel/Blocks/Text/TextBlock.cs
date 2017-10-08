using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using TextRight.Core.Cursors;
using TextRight.Core.Editing;
using TextRight.Core.ObjectModel.Cursors;
using TextRight.Core.ObjectModel.Serialization;

namespace TextRight.Core.ObjectModel.Blocks.Text
{
  /// <summary>
  ///  A block that contains a collection of TextSpans making up a single paragraph of text.
  /// </summary>
  public abstract class TextBlock : ContentBlock
  {
    private TextBlockContent _content;

    /// <summary> Default constructor. </summary>
    internal TextBlock()
    {
      Content = new TextBlockContent()
                {
                  Owner = this
                };

      (var x, var y) = (Content, this);
    }

    /// <summary> The textual content within the block </summary>
    public TextBlockContent Content
    {
      get { return _content; }
      set
      {
        var theValue = value ?? new TextBlockContent();
        if (theValue.Owner != null)
        {
          theValue.Owner = null;
        }

        theValue.Owner = this;
        _content = value;

        // TODO notify others
      }
    }

    /// <inheritdoc />
    public override BlockCaret GetCaretAtStart()
      => Content.GetCaretAtStart();

    /// <inheritdoc />
    public override BlockCaret GetCaretAtEnd()
      => Content.GetCaretAtEnd();

    /// <inheritdoc />
    public override ICursorPool CursorPool
      => TextBlockCursor.CursorPool;

    /// <inheritdoc />
    protected override IBlockContentCursor CreateCursorOverride()
    {
      return new TextBlockCursor(this);
    }

    /// <summary>
    ///  Serializes the properties of the given text block into a chunk of data that can be stored for
    ///  later use.
    /// </summary>
    /// <returns> A SerializedData structure containg the serialized data. </returns>
    public BlobSerializedData SerializeProperties()
    {
      PropertySerializers.DataWriter.Reset();
      WriteProperties(PropertySerializers.DataWriter);

      return new BlobSerializedData(PropertySerializers.DataWriter.ToArray());
    }

    /// <summary>
    ///  Deserializes the properties contained in <paramref name="data"/> back into the block.
    /// </summary>
    /// <param name="data"> The data to deserialize. </param>
    public void DeserializeProperties(BlobSerializedData data)
    {
      PropertySerializers.DataReader.Reset(data.Data);
      ReadProperties(PropertySerializers.DataReader);
    }

    /// <summary>
    ///  Reads data from the given data reader to repopulate the properties of this block.
    /// </summary>
    /// <param name="reader"> The reader from which to read . </param>
    public virtual void ReadProperties(IDataReader reader)
    {
      DescriptorHandle.Descriptor.DefaultPropertySerializer.Read(this, reader);
    }

    /// <summary> Writes the properties of this block to the given data writer. </summary>
    /// <param name="writer"> The writer to which to write. </param>
    public virtual void WriteProperties(IDataWriter writer)
    {
      DescriptorHandle.Descriptor.DefaultPropertySerializer.Write(this, writer);
    }

    /// <summary> Invoked when a new fragment is inserted into the textblock. </summary>
    /// <param name="previous"> The fragment that precedes the inserted fragment, can be null. </param>
    /// <param name="fragment"> The fragment that was inserted. </param>
    /// <param name="next"> The fragment that follows the inserted fragment, can be null. </param>
    protected internal abstract void OnFragmentInserted(StyledTextFragment previous,
                                               StyledTextFragment fragment,
                                               StyledTextFragment next);


    public TextBlockCursor GetTextCursor()
      => new TextBlockCursor(this);

    /// <inheritdoc/>
    public override Block Clone()
    {
      var clone = (TextBlock)DescriptorHandle.Descriptor.CreateInstance();
      clone.Content = Content.Clone();
      return clone;
    }

    /// <inheritdoc />
    protected override void SerializeInto(SerializeNode node)
    {
      Content.SerializeInto(node);
    }

    /// <inheritdoc />
    public override void Deserialize(SerializationContext context, SerializeNode node)
    {
      Content.Deserialize(context, node);
    }

    /// <summary> Extracts the content starting at the cursor and continuing to the end of the block. </summary>
    /// <param name="cursor"> The position at which extraction should start. </param>
    /// <returns> The fragments that have been extracted. </returns>
    public StyledTextFragment[] ExtractContentToEnd(TextBlockCursor cursor)
    {
      var endCursor = cursor.Block.Content.GetCaretAtEnd();
      return Content.ExtractContent(cursor.ToValue(), endCursor).Fragments.ToArray();
    }

    /// <inheritdoc />
    public override IBlockContentCursor GetCaretFromBottom(CaretMovementMode caretMovementMode)
    {
      var cursor = (TextBlockCursor)GetCursor();
      cursor.MoveToEnd();

      switch (caretMovementMode.CurrentMode)
      {
        case CaretMovementMode.Mode.Position:
          MoveCursorToPosition(cursor, caretMovementMode.Position);
          break;
        case CaretMovementMode.Mode.End:
          // already done
          break;
        case CaretMovementMode.Mode.Home:
          BlockCursorMover.BackwardMover.MoveCaretTowardsLineEdge(cursor);
          break;
        default:
          throw new ArgumentOutOfRangeException();
      }

      return cursor;
    }

    /// <inheritdoc />
    public override IBlockContentCursor GetCaretFromTop(CaretMovementMode caretMovementMode)
    {
      var cursor = (TextBlockCursor)GetCursor();

      cursor.MoveToBeginning();

      switch (caretMovementMode.CurrentMode)
      {
        case CaretMovementMode.Mode.Position:
          MoveCursorToPosition(cursor, caretMovementMode.Position);
          break;
        case CaretMovementMode.Mode.Home:
          // already done
          break;
        case CaretMovementMode.Mode.End:
          BlockCursorMover.ForwardMover.MoveCaretTowardsLineEdge(cursor);
          break;
        default:
          throw new ArgumentOutOfRangeException();
      }

      return cursor;
    }

    private void MoveCursorToPosition(TextBlockCursor cursor, double position)
    {
      // TODO find better way of doing this The problem is that we don't know
      // which way to go, so as a hack, we go both ways and rely on the
      // implementation of MoveToPosition to ultimately choose the closest one. 
      BlockCursorMover.ForwardMover.MoveTowardsLineOffset(cursor, position);
      BlockCursorMover.BackwardMover.MoveTowardsLineOffset(cursor, position);
    }
  }
}