using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Xml.Linq;

namespace TextRight.EditorApp.Wpf
{
  /// <summary>
  /// Interaction logic for MainWindow.xaml
  /// </summary>
  public partial class MainWindow : Window
  {
    private readonly string TempFile = "temp.xml";

    public MainWindow()
    {
      InitializeComponent();

      Loaded += delegate { LoadDocument(); };
      Closed += delegate { SaveDocument(); };
    }

    private void LoadDocument()
    {
      if (File.Exists(TempFile))
      {
        var xml = XElement.Load(TempFile);
        DevLoader.LoadInto(xml, TheEditor.EditorContext);
      }
    }

    private void SaveDocument()
    {
      DevLoader.SaveIntoElement(TheEditor.EditorContext).Save(TempFile);
    }
  }
}