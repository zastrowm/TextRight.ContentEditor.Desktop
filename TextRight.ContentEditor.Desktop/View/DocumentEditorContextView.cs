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
using TextRight.ContentEditor.Core.ObjectModel;
using TextRight.ContentEditor.Core.ObjectModel.Blocks;
using Block = TextRight.ContentEditor.Core.ObjectModel.Blocks.Block;
using BlockCollection = TextRight.ContentEditor.Core.ObjectModel.Blocks.BlockCollection;

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
    private readonly ScrollViewer _rootView;
    private readonly DocumentCursorView _cursorView;
    private readonly KeyboardShortcutCollection _keyCommands;

    private readonly BlockSearchHitTester _blockSearchHitTester;
    private ChangeIndex _layoutChangeIndex;

    public DocumentEditorContextView(DocumentEditorContext editor)
    {
      _blockSearchHitTester = new BlockSearchHitTester(this);

      editor.Target = this;
      Focusable = true;

      TextElement.SetFontSize(this, 16);
      TextElement.SetFontFamily(this, new FontFamily("Times New Roman"));
      TextOptions.SetTextFormattingMode(this, TextFormattingMode.Display);
      TextOptions.SetTextHintingMode(this, TextHintingMode.Fixed);
      TextOptions.SetTextRenderingMode(this, TextRenderingMode.ClearType);

      Cursor = Cursors.IBeam;

      _editor = editor;

      _cursorView = new DocumentCursorView(_editor.Caret);
      _cursorView.Attach(this);

      // clear out the existing content
      _blockCollectionView = new VerticalBlockCollectionView(this, (VerticalBlockCollection)_editor.Document.Root);

      _rootView = new ScrollViewer()
                  {
                    Content = _blockCollectionView,
                  };

      Children.Add(_rootView);

      SetTop(_rootView, 0);
      SetLeft(_rootView, 0);
      SetZIndex(_rootView, 0);

      var keyboardShortcutCollection = ConfigureCommands();

      _keyCommands = keyboardShortcutCollection;

      InsertText("This is an example of a document within the editor.  It has many features that extend onto " +
                 "multiple lines enough that we can start to create paragraphs.  Don't also forget" +
                 "about X & Y and those other things that extend the line length for the X-Files.  " +
                 "Isn't that great");

      ((IContextualCommand)new BreakTextBlockCommand()).Activate(_editor, _editor.UndoStack);

      InsertText("Another paragraph with addition text sits here, right where you need it to be.");

      _editor.UndoStack.Clear();
    }

    /// <summary>
    ///  True if the view has been laid out and character measurements would be valid.
    /// </summary>
    public bool IsLayoutValid
      => _blockCollectionView.IsMeasureValid;

    /// <summary> A ChangeIndex which marks when changes to the document layout has occurred. </summary>
    public ChangeIndex LayoutChangeIndex
      => _layoutChangeIndex;

    /// <summary> The view-factory for this instance. </summary>
    public ViewFactory ViewFactory
      = new ViewFactory();

    /// <summary>
    ///  Changes <see cref="LayoutChangeIndex"/> to indicate that a change to the layout of the document has
    ///  been made.
    /// </summary>
    public void MarkChanged()
    {
      _layoutChangeIndex = _layoutChangeIndex.Next();
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

      if (e.Key == Key.LeftShift || e.Key == Key.RightShift)
      {
        _editor.IsSelectionExtendActive = (Keyboard.Modifiers & ModifierKeys.Shift) != 0;
      }

      if (HandleKeyDown(e.Key))
      {
        UpdateCaretPosition();
        e.Handled = true;
      }
    }

    protected override void OnPreviewKeyUp(KeyEventArgs e)
    {
      base.OnPreviewKeyUp(e);

      if (e.Key == Key.LeftShift || e.Key == Key.RightShift)
      {
        _editor.IsSelectionExtendActive = (Keyboard.Modifiers & ModifierKeys.Shift) != 0;
      }
    }

    protected override void OnPreviewMouseDown(MouseButtonEventArgs e)
    {
      base.OnPreviewMouseDown(e);

      var position = e.GetPosition(this);
      _editor.HandleMouseDown(new DocumentPoint(position.X, position.Y));

      // TODO, do something with mouse events
      e.Handled = true;

      UpdateCaretPosition();
    }

    public bool HandleKeyDown(Key key)
    {
      var action = _keyCommands.LookupContextAction(Keyboard.Modifiers, key, _editor);

      // if we didn't find a command, but we had SHIFT, check if we have a Caret command that matches
      // without SHIFT as SHIFT could indicate that we want to extend selection. 
      if (action == null && (Keyboard.Modifiers & ModifierKeys.Shift) != 0)
      {
        action =
          _keyCommands.LookupContextAction(Keyboard.Modifiers & ~ModifierKeys.Shift, key, _editor) as CaretCommand;
      }

      if (action != null)
      {
        action.Activate(_editor, _editor.UndoStack);
        return true;
      }

      return false;
    }

    public void InsertText(string text)
    {
      var action = new InsertTextUndoableAction(new DocumentCursorHandle(_editor.Caret), text);
      _editor.UndoStack.Do(action);
    }

    public void UpdateCaretPosition()
    {
      _cursorView.Refresh();
    }

    public Block GetBlockFor(DocumentPoint point)
    {
      return _blockSearchHitTester.GetBlockAt(point);
    }

    /// <summary> Performs a HitTest to determine which block a point belongs to. </summary>
    private class BlockSearchHitTester
    {
      private readonly DocumentEditorContextView _ownerView;
      private readonly HitTestFilterCallback _hitTestFilterCallback;
      private readonly HitTestResultCallback _hitTestResultCallback;
      private Block _block;

      /// <summary> Constructor. </summary>
      /// <param name="ownerView"> The visual that is performing the hit test. </param>
      public BlockSearchHitTester(DocumentEditorContextView ownerView)
      {
        _ownerView = ownerView;
        _hitTestFilterCallback = FilterCallback;
        _hitTestResultCallback = ResultCallback;
      }

      /// <summary> Uses HitTesting to determine the block at the specified point. </summary>
      /// <param name="point"> The point at which the hit test should be performed. </param>
      /// <returns> The block at a the given point. </returns>
      public Block GetBlockAt(DocumentPoint point)
      {
        _block = null;
        VisualTreeHelper.HitTest(_ownerView,
                                 _hitTestFilterCallback,
                                 _hitTestResultCallback,
                                 new PointHitTestParameters(new Point(point.X, point.Y)));
        return _block;
      }

      /// <summary>
      ///  Callback for use with
      ///  <see cref="VisualTreeHelper.HitTest(Visual, HitTestFilterCallback, HitTestResultCallback, HitTestParameters)"/>
      /// </summary>
      private HitTestFilterBehavior FilterCallback(DependencyObject potentialhittesttarget)
      {
        var documentItemView = potentialhittesttarget as IDocumentItemView;
        var block = documentItemView?.DocumentItem as Block;
        if (block != null && !(block is BlockCollection))
        {
          _block = block;
          return HitTestFilterBehavior.Stop;
        }

        return HitTestFilterBehavior.Continue;
      }

      /// <summary>
      ///  Callback for use with
      ///  <see cref="VisualTreeHelper.HitTest(Visual, HitTestFilterCallback, HitTestResultCallback, HitTestParameters)"/>
      /// </summary>
      private HitTestResultBehavior ResultCallback(HitTestResult result)
      {
        return HitTestResultBehavior.Continue;
      }
    }

    private static KeyboardShortcutCollection ConfigureCommands()
    {
      var configuration = @"
Enter => block.split
Ctrl+1 => heading.convertTo1
Ctrl+2 => heading.convertTo2
Ctrl+3 => heading.convertTo3
Ctrl+4 => heading.convertTo4
Ctrl+5 => heading.convertTo5
Delete => text.deleteNextChar
Delete => block.merge
Backspace => text.deletePreviousChar
Backspace => block.merge
Left => caret.moveBackward
Right => caret.moveForward
Up => caret.moveUp
Down => caret.moveDown
Home => caret.moveHome
End => caret.moveEnd
Ctrl+Left => caret.MoveBackwardByWord
Ctrl+Left => caret.MoveBackward
Ctrl+Right => caret.moveForwardByWord
Ctrl+Right => caret.moveForward
Ctrl+Z => undo
Ctrl+Y => redo
";

      var allCommands = new IContextualCommand[]
                        {
                          new ConvertToLevelHeadingCommand(1),
                          new ConvertToLevelHeadingCommand(2),
                          new ConvertToLevelHeadingCommand(3),
                          new ConvertToLevelHeadingCommand(4),
                          new ConvertToLevelHeadingCommand(5),
                          new BreakTextBlockCommand(),
                          new DeleteNextCharacterCommand(),
                          new DeletePreviousCharacterCommand(),
                          new MergeTextBlocksCommand(),
                          new MoveCaretBackwardCommand(),
                          new MoveCaretForwardCommand(),
                          new MoveCaretUpCommand(),
                          new MoveCaretDownCommand(),
                          new MoveCaretHomeCommand(),
                          new MoveCaretEndCommand(),
                          new MoveCaretPreviousWordCommand(),
                          new MoveCaretNextWordCommand(),
                          new UndoCommand(),
                          new RedoCommand(),
                        }.ToDictionary(c => c.Id, c => c, StringComparer.InvariantCultureIgnoreCase);

      var converter = new KeyGestureConverter();

      var items = configuration
        .Split("\r\n".ToCharArray(), StringSplitOptions.RemoveEmptyEntries)
        .Select(s => s.Trim())
        .Select(s => s.Split(new string[] { " => " }, StringSplitOptions.RemoveEmptyEntries))
        .Select(p => new { StringKey = p[0], Id = p[1] })
        .Select(i => new { Key = (KeyGesture)converter.ConvertFromString(i.StringKey), Command = allCommands[i.Id] })
        .GroupBy(i => i.Key)
        ;
      var keyboardShortcutCollection = new KeyboardShortcutCollection();
      foreach (var aGroup in items)
      {
        keyboardShortcutCollection.Add(aGroup.Key.Modifiers, aGroup.Key.Key, aGroup.Select(it => it.Command).ToArray());
      }
      return keyboardShortcutCollection;
    }
  }
}