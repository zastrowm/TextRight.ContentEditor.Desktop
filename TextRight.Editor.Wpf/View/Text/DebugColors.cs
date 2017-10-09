using System.Windows.Media;

namespace TextRight.Editor.Wpf.View
{
  /// <summary> Colors to use when debugging rendering. </summary>
  internal class DebugColors
  {
    internal static readonly Pen CharacterBorders
      = new Pen(new SolidColorBrush(Color.FromArgb(126,0,83,252)),.5);
  }
}