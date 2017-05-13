using System;
using System.Linq;
using System.Collections.Generic;
using FluentAssertions;
using TextRight.Core.ObjectModel.Blocks.Text;
using Xunit;

namespace TextRight.Core.Tests.Text
{
  public class GraphemeHelperTests
  {
    [Theory]
    [InlineData(0, 3)]
    [InlineData(3, 1)]
    [InlineData(4, 2)]
    public void VerifyGraphemeHelperLength(int index, int expectedLength)
    {
      var text = "a\u0304\u0308bc\u0327";

      int length = GraphemeHelper.GetGraphemeLength(text, index);

      DidYouKnow.That(length).Should().Be(expectedLength);
    }
  }
}