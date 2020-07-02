using System;
using System.Collections.Generic;
using System.Linq;
using TextRight.Core.Cursors;
using TextRight.Core.Events;
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
    public TextBlock()
    {
      Content = new TextBlockContent()
                {
                  Owner = this
                };
    }

    /// <summary> The textual content within the block </summary>
    public TextBlockContent Content
    {
      get => _content;
      set
      {
        var theValue = value ?? new TextBlockContent();
        if (theValue.Owner != null)
        {
          theValue.Owner = null;
        }

        var oldValue = _content;

        theValue.Owner = this;
        _content = value;

        FireEvent(new TextSourceChangedEventArgs(oldValue, value));
      }
    }

    /// <inheritdoc />
    public override BlockCaret GetCaretAtStart()
      => Content.GetCaretAtStart();

    /// <inheritdoc />
    public override BlockCaret GetCaretAtEnd()
      => Content.GetCaretAtEnd();

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
    public virtual void ReadProperties(IPropertyReader reader)
    {
      var descriptor = DescriptorHandle;

      foreach (var property in descriptor.Properties)
      {
        property.Deserialize(this, reader);
      }
    }

    /// <summary> Writes the properties of this block to the given data writer. </summary>
    /// <param name="writer"> The writer to which to write. </param>
    public virtual void WriteProperties(IPropertyWriter writer)
    {
      var descriptor = DescriptorHandle;

      foreach (var property in descriptor.Properties)
      {
        property.Serialize(this, writer);
      }
    }

    /// <inheritdoc/>
    public override Block Clone()
    {
      var clone = (TextBlock)DescriptorHandle.CreateInstance();
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

    /// <summary> Listens to events on the TextBlock. </summary>
    public interface ITextBlockListener : IEventListener
    {
      /// <summary> Invoked when the TextBlock content of the TextBlock has changed. </summary>
      void TextBlockChanged(TextBlockContent oldContent, TextBlockContent newContent);
    }

    /// <summary> Event Args for when the TextSource changes on a TextBlock. </summary>
    public class TextSourceChangedEventArgs : EventEmitterArgs<ITextBlockListener>
    {
      public TextSourceChangedEventArgs(TextBlockContent oldContent, TextBlockContent newContent)
      {
        OldContent = oldContent;
        NewContent = newContent;
      }

      /// <summary> The old TextBlockContent.  Might already be assigned to a different TextBlock. </summary>
      public TextBlockContent OldContent { get; }

      /// <summary> The new TextBlockContent that is currently assigned. </summary>
      public TextBlockContent NewContent { get; }

      /// <inheritdoc />
      protected override void Handle(object sender, ITextBlockListener reciever)
      {
        reciever.TextBlockChanged(OldContent, NewContent);
      }
    }
  }
}