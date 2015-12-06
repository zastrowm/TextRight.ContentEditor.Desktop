using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;
using TextRight.ContentEditor.Desktop.Blocks;
using TextRight.ContentEditor.Desktop.ObjectModel.Blocks;

namespace TextRight.ContentEditor.Desktop.Tests.ObjectModel
{
  internal class DocumentOwnerTests
  {
    [Test]
    public void ByDefault_SingleTextBlockExists()
    {
      var documentOwner = new DocumentOwner();

      Assert.That(documentOwner.Root.FirstBlock, Is.InstanceOf<TextBlock>());
      Assert.That(((TextBlock)documentOwner.Root.FirstBlock).First().Text, Is.EqualTo(""));
    }
  }
}