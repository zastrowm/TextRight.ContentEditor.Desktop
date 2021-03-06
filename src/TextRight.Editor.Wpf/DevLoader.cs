﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using TextRight.Core;
using TextRight.Core.Blocks;
using TextRight.Core.ObjectModel.Blocks;
using TextRight.Core.ObjectModel.Blocks.Text;
using TextRight.Core.ObjectModel.Cursors;
using TextRight.Core.ObjectModel.Serialization;
using TextRight.Editor.Text;
using TextRight.Editor.Wpf.Serialization;

namespace TextRight.Editor.Wpf
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
                                     new DescriptorsLookup(ParagraphBlock.Descriptor,
                                                           (BlockDescriptor)HeadingBlock.Descriptor
                                     )
                                   );

        var mode = new CaretMovementMode();
        mode.SetModeToEnd();

        editorContext.Document.Root.Deserialize(serializationContext, node.Children.First());
        editorContext.Selection.Replace(editorContext.Document.Root.GetView<IBlockView>().GetCaretFromBottom(mode));
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