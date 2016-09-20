using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Xml.Linq;
using TextRight.ContentEditor.Desktop.App;
using TextRight.Editor.Wpf;
using TextRight.EditorApp.Wpf;
using WpfHosting;

[assembly: HostedTargetType(typeof(WatchEntryPoint))]

namespace TextRight.Editor.Wpf
{
  public class WatchEntryPoint : HostedEntryPoint
  {
    /// <inheritdoc/>
    public override FrameworkElement Initialize(Dictionary<string, string> configuration)
    {
      var editor = new DocumentEditor();

      string originalText;
      if (configuration.TryGetValue("document", out originalText))
      {
        DevLoader.LoadInto(XElement.Parse(originalText), editor.EditorContext);
      }

      return editor;
    }

    /// <inheritdoc/>
    public override void Shutdown(FrameworkElement originalInstance, Dictionary<string, string> configuration)
    {
      var editor = (DocumentEditor)originalInstance;

      var doc = DevLoader.SaveIntoElement(editor.EditorContext);
      var text = doc.ToString();

      configuration["document"] = text;
    }
  }
}