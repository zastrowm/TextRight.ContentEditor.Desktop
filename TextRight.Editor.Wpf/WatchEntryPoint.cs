using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Xml.Linq;
using Guapr.ClientHosting;
using TextRight.Editor.Wpf;

[assembly: HostedTargetType(typeof(WatchEntryPoint))]

namespace TextRight.Editor.Wpf
{
  internal class WatchEntryPoint : HostedEntryPoint<DocumentEditor, WatchEntryPoint.State>
  {
    protected override DocumentEditor Startup(IEntryPointStartupApi startupApi, State data)
    {
      var editor = new DocumentEditor();

      data = data ?? new State();

      var sessionDoc = Path.Combine(startupApi.StateDirectory.FullName, data.DocumentPath);
      if (File.Exists(sessionDoc))
      {
        DevLoader.LoadInto(XElement.Load(sessionDoc), editor.EditorContext);
      }

      return editor;
    }

    protected override State Shutdown(DocumentEditor gui, IEntryPointShutdownApi shutdownApi)
    {
      var doc = DevLoader.SaveIntoElement(gui.EditorContext);
      doc.Save(Path.Combine(shutdownApi.StateDirectory.FullName, "document.xml"));
      return new State();
    }

    public class State
    {
      public string DocumentPath { get; set; }
        = "document.xml";
    }
  }
}