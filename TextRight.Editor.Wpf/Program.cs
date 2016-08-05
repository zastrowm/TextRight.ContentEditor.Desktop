using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using TextRight.ContentEditor.Desktop.App;

namespace TextRight.Editor.Wpf
{
  public class Program
  {
    [STAThread]
    public static void Main(string[] args)
    {
      var application = new Application();
      application.Run(new MainWindow());
    }
  }
}