using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using TextRight.ContentEditor.Core.Editing;
using TextRight.ContentEditor.Core.Editing.Actions;
using TextRight.ContentEditor.Core.Editing.Commands;
using TextRight.ContentEditor.Core.ObjectModel.Blocks;

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
      _flowDocument = new FlowDocumentBlockCollectionView((VerticalBlockCollection)_editor.Document.Root);

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
                       { Key.Enter, TextCommands.BreakBlock },
                       { Key.Delete, TextCommands.DeleteNextCharacter },
                       { Key.Back, TextCommands.DeletePreviousCharacter },
                       // caret commands
                       { Key.Left, BuiltInCaretNavigationCommand.Backward },
                       { Key.Right, BuiltInCaretNavigationCommand.Forward },
                       { ModifierKeys.Control, Key.Left, BuiltInCaretNavigationCommand.PreviousWord },
                       { ModifierKeys.Control, Key.Right, BuiltInCaretNavigationCommand.NextWord },
                       { Key.Home, BuiltInCaretNavigationCommand.Home },
                       { Key.End, BuiltInCaretNavigationCommand.End },
                       { Key.Up, BuiltInCaretNavigationCommand.Up },
                       { Key.Down, BuiltInCaretNavigationCommand.Down },
                     };

      InsertText("This is an example of a document within the editor.  It has many features that extend onto" +
                 "multiple lines enough that we can start to create paragraphs.  Don't also forget" +
                 "about X & Y and those other things that extend the line length for the X-Files.  " +
                 "Isn't that great");

      _editor.CommandPipeline.Execute(TextCommands.BreakBlock);

      InsertText("Another paragraph with addition text sits here, right where you need it to be.");
    }

    public new void Focus()
    {
      _flowDocument.Focus();
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
      var command = _keyCommands.Lookup(Keyboard.Modifiers, key);
      if (command != null)
      {
        _editor.CommandPipeline.Execute(command);
        return true;
      }

      return false;
    }

    public void InsertText(string text)
    {
      new InsertTextAction(new DocumentCursorHandle(_editor.Caret), text).Do(_editor);
      //new InsertTextCommand(text).Execute(_editor);
    }

    public void UpdateCaretPosition()
    {
      _caretView.SyncPosition();
    }
  }
}