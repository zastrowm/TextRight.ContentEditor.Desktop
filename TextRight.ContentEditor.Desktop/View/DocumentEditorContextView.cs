using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using TextRight.ContentEditor.Core;
using TextRight.ContentEditor.Core.Editing;
using TextRight.ContentEditor.Core.Editing.Actions;
using TextRight.ContentEditor.Core.Editing.Commands;
using TextRight.ContentEditor.Core.ObjectModel;
using TextRight.ContentEditor.Core.ObjectModel.Blocks;
using Block = TextRight.ContentEditor.Core.ObjectModel.Blocks.Block;

namespace TextRight.ContentEditor.Desktop.View
{
  /// <summary>
  ///  Creates an editor from a FlowDocument and associated TextRight Document.
  /// </summary>
  public class DocumentEditorContextView : Canvas,
                                           IDocumentEditorView
  {
    private readonly VerticalBlockCollectionView _blockCollectionView;
    private readonly DocumentEditorContext _editor;
    private readonly CaretView _caretView;
    private readonly ScrollViewer _rootView;
    private readonly KeyboardShortcutCollection _keyCommands;

    private readonly ActionStack _undoStack;

    public DocumentEditorContextView(DocumentEditorContext editor)
    {
      editor.Target = this;
      Focusable = true;

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
                       {
                         Key.Enter, new IContextualCommand[]
                                    {
                                      new BreakTextBlockAction()
                                    }
                       },
                       {
                         Key.Delete, new IContextualCommand[]
                                     {
                                       new DeleteNextCharacterCommand(),
                                       new MergeTextBlocksCommand()
                                     }
                       },
                       {
                         Key.Back, new IContextualCommand[]
                                   {
                                     new DeletePreviousCharacterCommand(),
                                     new MergeTextBlocksCommand()
                                   }
                       },
                     };

      InsertText("This is an example of a document within the editor.  It has many features that extend onto " +
                 "multiple lines enough that we can start to create paragraphs.  Don't also forget" +
                 "about X & Y and those other things that extend the line length for the X-Files.  " +
                 "Isn't that great");

      ((IContextualCommand)new BreakTextBlockAction()).Activate(_editor, _undoStack);

      InsertText("Another paragraph with addition text sits here, right where you need it to be.");
    }

    /// <inheritdoc />
    public IDocumentItem DocumentItem
      => _editor;

    public new void Focus()
    {
      base.Focus();
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
        UpdateCaretPosition();
        e.Handled = true;
      }
      else if (e.Key == Key.Z && (e.KeyboardDevice.Modifiers & ModifierKeys.Control) != 0)
      {
        _undoStack.Undo();
        UpdateCaretPosition();
        e.Handled = true;
      }
      else if (e.Key == Key.Y && (e.KeyboardDevice.Modifiers & ModifierKeys.Control) != 0)
      {
        _undoStack.Redo();
        UpdateCaretPosition();
        e.Handled = true;
      }
    }

    protected override void OnPreviewMouseDown(MouseButtonEventArgs e)
    {
      base.OnPreviewMouseDown(e);

      var position = e.GetPosition(this);
      _editor.HandleMouseDown(new DocumentPoint(position.X, position.Y));

      // TODO, do something with mouse events
      e.Handled = true;
    }

    public bool HandleKeyDown(Key key)
    {
      var action = _keyCommands.LookupContextAction(Keyboard.Modifiers, key, _editor);
      if (action != null)
      {
        action.Activate(_editor, _undoStack);
        return true;
      }

      var command = GetEditorCommand(Keyboard.Modifiers, key);
      if (command != null)
      {
        _editor.CommandPipeline.Execute(command);
        return true;
      }

      return false;
    }

    private EditorCommand GetEditorCommand(ModifierKeys modifiers, Key key)
    {
      switch (key)
      {
        case Key.Left:
          return modifiers.HasFlag(ModifierKeys.Control)
            ? BuiltInCaretNavigationCommand.PreviousWord
            : BuiltInCaretNavigationCommand.Backward;
        case Key.Right:
          return modifiers.HasFlag(ModifierKeys.Control)
            ? BuiltInCaretNavigationCommand.NextWord
            : BuiltInCaretNavigationCommand.Forward;
        case Key.Home:
          return BuiltInCaretNavigationCommand.Home;
        case Key.End:
          return BuiltInCaretNavigationCommand.End;
        case Key.Up:
          return BuiltInCaretNavigationCommand.Up;
        case Key.Down:
          return BuiltInCaretNavigationCommand.Down;
      }

      return null;
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

    public Block GetBlockFor(DocumentPoint point)
    {
      return null;
    }
  }
}