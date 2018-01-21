using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Xml.Linq;

namespace TextRight.Editor.Wpf
{
  /// <summary>
  /// Interaction logic for MainWindow.xaml
  /// </summary>
  public class MainWindow : Window
  {
    private readonly string TempFile = "temp.xml";
    private readonly DocumentEditor _theEditor;

    public MainWindow()
    {
      _theEditor = new DocumentEditor();
      Content = _theEditor;

      Loaded += delegate { LoadDocument(); };
      Closed += delegate { SaveDocument(); };
    }

    [STAThread]
    public static void Main()
    {
      var app = new Application();
      app.Run(new MainWindow());
    }

    private void LoadDocument()
    {
      if (File.Exists(TempFile))
      {
        var xml = XElement.Load(TempFile);
        DevLoader.LoadInto(xml, _theEditor.EditorContext);
      }
    }

    private void SaveDocument()
    {
      DevLoader.SaveIntoElement(_theEditor.EditorContext).Save(TempFile);
    }
  }
}