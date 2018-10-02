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
  internal class WatchEntryPoint : SimpleStateHostedEntryPoint<DocumentEditor, WatchEntryPoint.State>
  {
    /// <inheritdoc />
    public override State CreateDefaultState()
      => new State();

    /// <inheritdoc />
    public override DocumentEditor CreateControl()
    {
      var editor = new DocumentEditor();
      var sessionDoc = Path.Combine(StateDirectory.FullName, CurrentState.DocumentPath);

      if (File.Exists(sessionDoc))
      {
        DevLoader.LoadInto(XElement.Load(sessionDoc), editor.EditorContext);
      }

      return editor;
    }

    /// <inheritdoc />
    public override State GetStateToPersist()
    {
      var doc = DevLoader.SaveIntoElement(Control.EditorContext);
      doc.Save(Path.Combine(StateDirectory.FullName, CurrentState.DocumentPath));

      return base.GetStateToPersist();
    }

    public class State
    {
      public string DocumentPath { get; set; }
        = "document.xml";
    }
  }
}