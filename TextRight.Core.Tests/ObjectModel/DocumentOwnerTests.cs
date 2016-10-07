using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;
using TextRight.Core.ObjectModel;
using TextRight.Core.ObjectModel.Blocks.Text;

namespace TextRight.Core.Tests.ObjectModel
{
  internal class DocumentOwnerTests
  {
    [Test]
    public void ByDefault_SingleTextBlockExists()
    {
      var documentOwner = new DocumentOwner();

      Assert.That(documentOwner.Root.FirstBlock, Is.InstanceOf<TextBlock>());
      Assert.That(((TextBlock)documentOwner.Root.FirstBlock).Fragments.First().GetText(), Is.EqualTo(""));
    }
  }
}