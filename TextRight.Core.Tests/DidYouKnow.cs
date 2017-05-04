using System;
using System.Collections.Generic;
using System.Linq;

namespace TextRight.Core.Tests
{
  /// <summary> Helper class to use instead of using .Should() directly. </summary>
  /// <example>
  ///  Use instead of Should() extension method:
  ///  <code><![CDATA[
  ///   DidYouKnow.That(value).Should().Be("Hi There");
  ///  ]]></code>
  ///  vs
  ///  <code><![CDATA[
  ///   value.Should().Be("Hi There");
  ///  ]]></code>
  /// </example>
  public static class DidYouKnow
  {
    /// <see cref="DidYouKnow"/>>
    public static T That<T>(T item) 
      => item;
  }
}