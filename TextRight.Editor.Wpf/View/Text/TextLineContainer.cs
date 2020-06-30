using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Media.TextFormatting;
using TextRight.Core.ObjectModel.Blocks.Text;

namespace TextRight.Editor.Wpf.View
{
  /// <summary> Container for holding the cached line and point for rendering. </summary>
  internal struct TextLineContainer 
  {
    public TextLineContainer(Point point,
                             TextLine line,
                             TextOffset offset,
                             TextBlockContent content
      )
    {
      Line = line;
      Point = point;
      Offset = offset;
      Content = content;
    }

    public TextLine Line { get; }

    public TextOffset Offset { get; }

    public Point Point { get; }
    
    public TextBlockContent Content { get; }
  }
}