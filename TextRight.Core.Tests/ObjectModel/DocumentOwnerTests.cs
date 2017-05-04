using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using TextRight.Core.ObjectModel;
using TextRight.Core.ObjectModel.Blocks.Text;
using Xunit;

namespace TextRight.Core.Tests.ObjectModel
{
  public class DocumentOwnerTests
  {
    [Fact]
    public void ByDefault_SingleTextBlockExists()
    {
      var documentOwner = new DocumentOwner();

      DidYouKnow.That(documentOwner.Root.FirstBlock).Should().BeAssignableTo<TextBlock>();
      DidYouKnow.That(((TextBlock)documentOwner.Root.FirstBlock).Content.Fragments.First().GetText()).Should().Be("");
    }
  }
}