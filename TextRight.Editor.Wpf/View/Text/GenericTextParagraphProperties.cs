using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media.TextFormatting;

namespace TextRight.Editor.Wpf.View
{
  internal class GenericTextParagraphProperties : TextParagraphProperties
  {
    public GenericTextParagraphProperties(bool isFirst)
    {
      FlowDirection = FlowDirection.LeftToRight;
      TextAlignment = TextAlignment.Left;
      LineHeight = 0; // AUTO
      FirstLineInParagraph = isFirst;
      DefaultTextRunProperties = new StyledTextRunProperties();
      TextWrapping = TextWrapping.Wrap;
      TextMarkerProperties = null;
      Indent = 0;
    }

    public override FlowDirection FlowDirection { get; }
    public override TextAlignment TextAlignment { get; }
    public override double LineHeight { get; }
    public override bool FirstLineInParagraph { get; }
    public override TextRunProperties DefaultTextRunProperties { get; }
    public override TextWrapping TextWrapping { get; }
    public override TextMarkerProperties TextMarkerProperties { get; }
    public override double Indent { get; }
  }
}