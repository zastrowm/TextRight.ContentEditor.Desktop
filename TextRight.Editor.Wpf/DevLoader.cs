using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using TextRight.ContentEditor.Core.Editing;
using TextRight.ContentEditor.Core.ObjectModel.Blocks;
using TextRight.ContentEditor.Core.ObjectModel.Cursors;
using TextRight.ContentEditor.Core.ObjectModel.Serialization;
using TextRight.Editor.Wpf.Serialization;

namespace TextRight.EditorApp.Wpf
{
  public static class DevLoader
  {
    public static void LoadInto(XElement fullXml, DocumentEditorContext editorContext)
    {
      try
      {
        var contentXml = fullXml;

        var node = XmlNodeSerializer.Deserialize(contentXml);

        var serializationContext = new SerializationContext(
                                     new DescriptorsLookup(ParagraphBlock.RegisteredDescriptor.Descriptor,
                                                           HeadingBlock.RegisteredDescriptor.Descriptor
                                     )
                                   );

        var mode = new CaretMovementMode();
        mode.SetModeToEnd();

        editorContext.Document.Root.Deserialize(serializationContext, node.Children.First());
        editorContext.Caret.MoveTo(
                       editorContext.Document.Root.GetCaretFromBottom(mode));
      }
      catch (Exception)
      {
        // ignored
      }
    }

    public static XElement SaveIntoElement(DocumentEditorContext context)
    {
      var node = context.Document.SerializeAsNode();
      var xml = XmlNodeSerializer.Serialize(node);
      return xml;
    }
  }
}