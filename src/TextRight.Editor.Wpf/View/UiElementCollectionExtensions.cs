using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace TextRight.Editor.Wpf.View
{
  public static class UiElementCollectionExtensions
  {
    public static UIElement First(this UIElementCollection collection)
      => collection[0];

    public static UIElement Last(this UIElementCollection collection)
      => collection[collection.Count - 1];
  }
}