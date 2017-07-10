using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using TextRight.Core;
using TextRight.Core.Tests;
using TextRight.Core.Tests.Framework;
using TextRight.Core.Utilities;
using TextRight.Editor.View.Blocks;
using Xunit;
using Xunit.Abstractions;

namespace TextRight.Editor.View.Tests
{
  public class TextCaretExtensionsTests
  {
    private readonly SimpleTextRenderer _renderer;
    private static readonly int CaretHeight = 10;
    private static readonly int CaretWidth = 0;

    public TextCaretExtensionsTests()
    {
      _renderer = new SimpleTextRenderer(
        "this # 1 ",
        "this # 2",
        "this # 3");
    }

    public static TheoryData<MeasurementsData> MeasurementsTheoryData()
    {
      return new TheoryData<MeasurementsData>()
             {
               new MeasurementsData()
               {
                 Position = 0,
                 Reason = "Beginning",
                 Measurement = new MeasuredRectangle(new DocumentPoint(0, 0), CaretWidth, CaretHeight),
               },
               new MeasurementsData()
               {
                 Position = 9,
                 Reason = "End of line w/space",
                 Measurement = new MeasuredRectangle(new DocumentPoint(0, 10), CaretWidth, CaretHeight),
               },
               new MeasurementsData()
               {
                 Position = 17,
                 Reason = "End of line without space",
                 Measurement = new MeasuredRectangle(new DocumentPoint(80, 10), CaretWidth, CaretHeight),
               },
               new MeasurementsData()
               {
                 Position = 25,
                 Reason = "End of block",
                 Measurement = new MeasuredRectangle(new DocumentPoint(80, 20), CaretWidth, CaretHeight),
               }
             };
    }

    [Theory]
    [MemberData(nameof(MeasurementsTheoryData))]
    public void MeasurementsAreCorrect(MeasurementsData data)
    {
      var caret = _renderer.Content.GetCaretAtStart()
                           .MoveCursorForwardBy(data.Position);
      var measurement = _renderer.MeasureCaret(caret);

      DidYouKnow.That(measurement).ShouldBeEquivalentTo(data.Measurement, data.Reason);
    }

    public class MeasurementsData : SerializableTestData<MeasurementsData>
    {
      public string Reason;
      public int Position;
      public MeasuredRectangle Measurement;

      public override string ToString()
        => $"Position={Position}, Reason={Reason}";

      public override bool IsRecursive
        => true;
    }
  }
}