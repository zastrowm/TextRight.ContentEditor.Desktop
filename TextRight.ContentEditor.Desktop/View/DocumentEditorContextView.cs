using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
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
    private readonly VerticalBlockCollectionView _blockCollectionView;
    private readonly DocumentEditorContext _editor;
    private readonly CaretView _caretView;
    private readonly ScrollViewer _rootView;
    private readonly KeyboardShortcutCollection _keyCommands;

    private readonly ActionStack _undoStack;

    public DocumentEditorContextView(DocumentEditorContext editor)
    {
      TextElement.SetFontSize(this, 16);
      TextElement.SetFontFamily(this, new FontFamily("Times New Roman"));
      TextOptions.SetTextFormattingMode(this, TextFormattingMode.Display);
      TextOptions.SetTextHintingMode(this, TextHintingMode.Fixed);
      TextOptions.SetTextRenderingMode(this, TextRenderingMode.ClearType);

      _editor = editor;

      _undoStack = new ActionStack(editor);

      _caretView = new CaretView(_editor.Caret);
      Children.Add(_caretView.Element);

      // clear out the existing content
      _blockCollectionView = new VerticalBlockCollectionView((VerticalBlockCollection)_editor.Document.Root);

      _rootView = new ScrollViewer()
                  {
                    Content = _blockCollectionView,
                  };

      Children.Add(_rootView);

      SetTop(_rootView, 0);
      SetLeft(_rootView, 0);
      SetZIndex(_rootView, 0);

      _keyCommands = new KeyboardShortcutCollection()
                     {
                       // editing commands
                       { Key.Enter, new BreakTextBlockAction() },
                       { Key.Delete, new DeleteNextCharacterCommand() },
                       { Key.Back, new DeletePreviousCharacterCommand() },
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

      InsertText("This is an example of a document within the editor.  It has many features that extend onto " +
                 "multiple lines enough that we can start to create paragraphs.  Don't also forget" +
                 "about X & Y and those other things that extend the line length for the X-Files.  " +
                 "Isn't that great");

      ((IContextualCommand)new BreakTextBlockAction()).Activate(_editor, _undoStack);

      InsertText("Another paragraph with addition text sits here, right where you need it to be.");
    }

    public new void Focus()
    {
      _blockCollectionView.Focus();
    }

    public void Initialize()
    {
      UpdateCaretPosition();
    }

    protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
    {
      base.OnRenderSizeChanged(sizeInfo);

      _rootView.Width = sizeInfo.NewSize.Width;
      _rootView.Height = sizeInfo.NewSize.Height;
    }

    protected override void OnTextInput(TextCompositionEventArgs e)
    {
      base.OnTextInput(e);

      if (e.Text == "")
        return;

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
      else if (e.Key == Key.Z && (e.KeyboardDevice.Modifiers & ModifierKeys.Control) != 0)
      {
        _undoStack.Undo();
        UpdateCaretPosition();
      }
    }

    public bool HandleKeyDown(Key key)
    {
      var action = _keyCommands.LookupContextAction(Keyboard.Modifiers, key);
      if (action != null)
      {
        if (action.CanActivate(_editor))
        {
          action.Activate(_editor, _undoStack);
          return true;
        }
        return false;
      }

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
      var action = new InsertTextUndoableAction(new DocumentCursorHandle(_editor.Caret), text);
      _undoStack.Do(action);
    }

    public void UpdateCaretPosition()
    {
      _caretView.SyncPosition();
    }
  }
}