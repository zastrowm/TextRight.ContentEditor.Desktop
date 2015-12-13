using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;
using TextRight.ContentEditor.Core.ObjectModel.Blocks;

namespace TextRight.ContentEditor.Core.Tests.ObjectModel.Blocks
{
  internal class TextBlockTests
  {
    [Test]
    public void ByDefault_HasSingleSpan()
    {
      var block = new TextBlock();

      Assert.That(block.Count(), Is.EqualTo(1));
    }
  }
}