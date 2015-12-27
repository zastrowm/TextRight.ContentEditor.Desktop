﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using TextRight.ContentEditor.Core.Editing;
using TextRight.ContentEditor.Core.Editing.Commands;

namespace TextRight.ContentEditor.Desktop.View
{
  /// <summary>
  ///  Creates an editor from a FlowDocument and associated TextRight Document.
  /// </summary>
  public class DocumentEditorContextView : Canvas
  {
    private readonly FlowDocument _flowDocument;
    private readonly DocumentEditorContext _editor;
    private readonly CaretView _caretView;
    private readonly FlowDocumentScrollViewer _documentViewer;
    private readonly KeyboardShortcutCollection _keyCommands;

    public DocumentEditorContextView(DocumentEditorContext editor)
    {
      _editor = editor;

      _caretView = new CaretView(_editor.Caret);
      Children.Add(_caretView.Element);

      // clear out the existing content
      _flowDocument = new FlowDocumentBlockCollectionView(_editor.Document.Root);

      _documentViewer = new FlowDocumentScrollViewer()
                        {
                          Document = _flowDocument,
                        };

      Children.Add(_documentViewer);

      SetTop(_documentViewer, 0);
      SetLeft(_documentViewer, 0);
      SetZIndex(_documentViewer, 0);

      _keyCommands = new KeyboardShortcutCollection()
                     {
                       // editing commands
                       { Key.Enter, DocumentEditorContext.Commands.BreakBlock },
                       { Key.Delete, DocumentEditorContext.Commands.DeleteNextCharacter },
                       // caret commands
                       { Key.Left, CaretCommands.MoveBackward },
                       { Key.Right, CaretCommands.MoveForward },
                       { ModifierKeys.Control, Key.Left, CaretCommands.MoveToPreviousWord },
                       { ModifierKeys.Control, Key.Right, CaretCommands.MoveToNextWord },
                       { Key.Back, DocumentEditorContext.Commands.DeletePreviousCharacter },
                       { Key.Home, CaretCommands.MoveToBeginningOfLine },
                       { Key.End, CaretCommands.MoveToEndOfLine },
                       { Key.Up, CaretCommands.MoveUp },
                       { Key.Down, CaretCommands.MoveDown },
                     };

      InsertText("This is an example of a document within the editor.  It has many features that extend onto" +
                 "multiple lines enough that we can start to create paragraphs.  Don't also forget" +
                 "about X & Y and those other things that extend the line length for the X-Files.  " +
                 "Isn't that great");

      _editor.Execute(DocumentEditorContext.Commands.BreakBlock);

      InsertText("Another paragraph with addition text sits here, right where you need it to be.");
    }

    public void Initialize()
    {
      UpdateCaretPosition();
    }

    protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
    {
      base.OnRenderSizeChanged(sizeInfo);

      _documentViewer.Width = sizeInfo.NewSize.Width;
      _documentViewer.Height = sizeInfo.NewSize.Height;
    }

    protected override void OnTextInput(TextCompositionEventArgs e)
    {
      base.OnTextInput(e);

      InsertText(e.Text);
      UpdateCaretPosition();
    }

    protected override void OnPreviewKeyDown(KeyEventArgs e)
    {
      base.OnPreviewKeyDown(e);

      if (HandleKeyDown(e.Key))
      {
        e.Handled = true;
        UpdateCaretPosition();
      }
    }

    public bool HandleKeyDown(Key key)
    {
      ISimpleActionCommand command = _keyCommands.Lookup(Keyboard.Modifiers, key);
      if (command != null)
      {
        _editor.Execute(command);
        return true;
      }

      return false;
    }

    public void InsertText(string text)
    {
      new InsertTextCommand(text).Execute(_editor);
    }

    public void UpdateCaretPosition()
    {
      _caretView.SyncPosition();
    }
  }
}