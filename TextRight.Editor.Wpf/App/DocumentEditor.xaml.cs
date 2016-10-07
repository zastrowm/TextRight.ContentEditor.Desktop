﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Controls;
using TextRight.Core.Editing;
using TextRight.Editor.Wpf.View;

namespace TextRight.Editor.Wpf
{
  /// <summary>
  /// Interaction logic for DocumentEditor.xaml
  /// </summary>
  public partial class DocumentEditor : UserControl
  {
    public DocumentEditor()
      : this(new DocumentEditorContext())
    {
    }

    public DocumentEditor(DocumentEditorContext context)
    {
      Loaded += delegate
                {
                  var view = ((DocumentEditorContextView)Content);
                  view.Initialize();
                  view.Focus();
                };

      EditorContext = context;
      Content = new DocumentEditorContextView(EditorContext);
    }

    /// <summary> The current editor context. </summary>
    public DocumentEditorContext EditorContext { get; }
  }
}