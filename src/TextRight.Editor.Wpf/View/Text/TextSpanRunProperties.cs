using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.TextFormatting;

namespace TextRight.Editor.Wpf.View
{
  internal class TextSpanRunProperties : TextRunProperties
  {
    public TextSpanRunProperties(int fontSize = 24)
    {
      Typeface = new Typeface("Tahoma");
      FontRenderingEmSize = FontHintingEmSize = fontSize;
      TextDecorations = new TextDecorationCollection();
      ForegroundBrush = new SolidColorBrush() { Color = Colors.Black };
      BackgroundBrush = new SolidColorBrush() { Color = Colors.White };
      CultureInfo = CultureInfo.CurrentCulture;
      TextEffects = new TextEffectCollection();
    }

    public override Typeface Typeface { get; }
    public override double FontRenderingEmSize { get; }
    public override double FontHintingEmSize { get; }
    public override TextDecorationCollection TextDecorations { get; }
    public override Brush ForegroundBrush { get; }
    public override Brush BackgroundBrush { get; }
    public override CultureInfo CultureInfo { get; }
    public override TextEffectCollection TextEffects { get; }
  }
}