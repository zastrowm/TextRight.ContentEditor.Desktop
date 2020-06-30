using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using TextRight.Core;
using TextRight.Core.Blocks;
using TextRight.Core.Commands;
using TextRight.Core.Commands.Caret;
using TextRight.Core.Commands.Text;
using TextRight.Core.ObjectModel;
using TextRight.Core.ObjectModel.Blocks;
using TextRight.Core.ObjectModel.Blocks.Text;
using Block = TextRight.Core.ObjectModel.Blocks.Block;
using SelectionMode = TextRight.Core.ObjectModel.Cursors.SelectionMode;

namespace TextRight.Editor.Wpf.View
{
  /// <summary>
  ///  Creates an editor from a FlowDocument and associated TextRight Document.
  /// </summary>
  public class DocumentEditorContextView : Canvas,
                                           IDocumentEditorView
  {
    private readonly VerticalBlockCollectionView _blockCollectionView;
    private readonly DocumentEditorContext _editor;
    private readonly ScrollViewer _scrollView;
    private readonly DocumentCursorView _cursorView;
    private readonly KeyboardShortcutCollection _keyCommands;
    private readonly ViewFactory _viewFactory;

    private readonly BlockSearchHitTester _blockSearchHitTester;
    private ChangeIndex _layoutChangeIndex;

    private readonly IContextualCommand<string> _insertText = new InsertTextCommand();

    public DocumentEditorContextView(DocumentEditorContext editor)                                      
    {
      _viewFactory = new ViewFactory();
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

      _cursorView = new DocumentCursorView(_editor.Selection);

      // clear out the existing content
      _blockCollectionView = new VerticalBlockCollectionView(this, _editor.Document.Root)
                             {
                               Background = Brushes.White,
                             };

      _scrollView = new ScrollViewer();
      Children.Add(_scrollView);

      SetTop(_scrollView, 0);
      SetLeft(_scrollView, 0);
      SetZIndex(_scrollView, 0);

      // use a grid simply because it's convenient.  What we really need is a canvas (for absolute
      // positioning) that overlays the content of the scroll viewer. 
      var layoutGrid = new Grid();
      _scrollView.Content = layoutGrid;

      // simply add the content in
      layoutGrid.Children.Add(new StackPanel
                              {
                                Background = Brushes.Transparent,
                                Children =
                                {
                                  _blockCollectionView
                                }
                              });

      var layoutAbsolute = new Canvas();
      layoutGrid.Children.Add(layoutAbsolute);

      UIElementCollection parentCollection = layoutAbsolute.Children;
      parentCollection.Add(_cursorView);

      var keyboardShortcutCollection = ConfigureCommands();

      _keyCommands = keyboardShortcutCollection;

      InsertText("This is an example of a document within the editor. జో It has many features that extend onto " +
                 "multiple lines enough that we can start to create paragraphs.  Don't also forget" +
                 "about X & Y and those other things that extend the line length for the X-Files.  " +
                 "Isn't that great? జో");

      ((IContextualCommand)new BreakTextBlockCommand()).Activate(_editor, _editor.UndoStack);

      InsertText("Another paragraph with addition text sits here, right where you need it to be.");

      //var listItem = new ListItemBlock();
      //document.Root.Append(listItem);
      //var cursor = ((Core.ObjectModel.Blocks.Text.TextBlock)listItem.FirstBlock).GetTextCursor();
      //cursor.MoveToBeginning();
      //cursor.InsertText("A list item");

      _editor.UndoStack.Clear();

      layoutGrid.PreviewMouseDown += HandlePreviewMouseDown;
      layoutGrid.PreviewMouseUp += HandlePreviewMouseUp;
      layoutGrid.PreviewMouseMove += HandlePreviewMouseMove;

      BlockSearchHitTester.SetShouldStopHitTesting(_cursorView, true);
      BlockSearchHitTester.SetShouldStopHitTesting(layoutAbsolute, true);
    }

    /// <summary>
    ///  True if the view has been laid out and character measurements would be valid.
    /// </summary>
    public bool IsLayoutValid
      => _blockCollectionView.IsMeasureValid;

    /// <summary> A ChangeIndex which marks when changes to the document layout has occurred. </summary>
    public ChangeIndex LayoutChangeIndex
      => _layoutChangeIndex;

    /// <summary> Constructs a new view for a given block instance. </summary>
    public UIElement CreateViewFor(Block block)
      => _viewFactory.GetViewFor(this, block);

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

    /// <summary> The editor context associated with the view. </summary>
    public DocumentEditorContext Context
      => _editor;

    /// <summary>
    ///  The visual that serves as the root of where carets are measured from.  This visual hosts all
    ///  content of the document.
    /// </summary>
    public FrameworkElement RootVisual
      => _blockCollectionView;

    /// <summary> Convert a Document Point to a relative/WPF point. </summary>
    public Point ToPoint(DocumentPoint point)
    {
      var localPoint = new Point(point.X, point.Y);
      return RootVisual.TranslatePoint(localPoint, _scrollView);
    }

    /// <summary> Convert a Wpf/relative point into a point within the document that is absolutely positioned. </summary>
    private DocumentPoint ToDocumentPoint(Point point)
    {
      var documentPoint = _scrollView.TranslatePoint(point, RootVisual);
      return new DocumentPoint(documentPoint.X, documentPoint.Y);
    }

    public new void Focus()
    {
      base.Focus();
    }

    public async void Initialize()
    {
      MarkChanged();

      await Task.Yield();

      _cursorView.MarkDirty();
    }

    protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
    {
      base.OnRenderSizeChanged(sizeInfo);

      _scrollView.Width = sizeInfo.NewSize.Width;
      _scrollView.Height = sizeInfo.NewSize.Height;

      MarkChanged();
    }

    protected override void OnTextInput(TextCompositionEventArgs e)
    {
      base.OnTextInput(e);

      if (e.Text == "")
        return;

      InsertText(e.Text);

      // This seem to bring the previous position into view, not the current. We may have to defer
      // this until the caret has been redrawn. 
      _cursorView.CaretElement.BringIntoView();
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

    protected void HandlePreviewMouseDown(object sender, MouseButtonEventArgs e)
    {
      UpdateSelection(e.GetPosition(this),
                      _editor.Selection.ShouldExtendSelection ? SelectionMode.Extend : SelectionMode.Replace);

      // TODO, do something with mouse events
      e.Handled = true;

      // start capturing (which will start calling the mouse overrides, not the preview methods) so
      // that even if they move the caret outside of the window/control, we still get mouse-move
      // events. 
      CaptureMouse();
    }

    protected void HandlePreviewMouseUp(object sender, MouseButtonEventArgs e)
    {
      if (IsMouseCaptured)
      {
        ReleaseMouseCapture();
      }
    }

    protected override void OnMouseUp(MouseButtonEventArgs e)
    {
      if (!IsMouseCaptured)
        return;

      HandlePreviewMouseUp(this, e);
    }

    protected void HandlePreviewMouseMove(object sender, MouseEventArgs e)
    {
      if (e.LeftButton != MouseButtonState.Pressed)
        return;

      UpdateSelection(e.GetPosition(this), SelectionMode.Extend);
      e.Handled = true;
    }

    protected override void OnMouseMove(MouseEventArgs e)
    {
      if (!IsMouseCaptured)
        return;

      HandlePreviewMouseMove(this, e);
    }

    /// <summary>
    ///   Updates the selection of the editor to point at the given location.
    /// </summary>
    private void UpdateSelection(Point position, SelectionMode mode)
    {
      var point = ToDocumentPoint(position);

      if (GetBlockFor(point) is IDocumentItem block)
      {
          var caret = ((BaseTextBlockView)block.DocumentItemView).GetCursor(point);
        _editor.Selection.MoveTo(caret, mode);
      }
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
        _cursorView.VerifyNotStale();

        return true;
      }

      return false;
    }

    public void InsertText(string text)
    {
      _insertText.Activate(_editor, _editor.UndoStack, text);
    }

    public Block GetBlockFor(DocumentPoint point)
    {
      return _blockSearchHitTester.GetBlockAt(point);
    }

    private KeyboardShortcutCollection ConfigureCommands()
    {
      var configuration = File.ReadAllLines(@"keyboard.trcfg");

      var descriptors = new BlockDescriptor[]
                        {
                          ParagraphBlock.Descriptor,
                          HeadingBlock.Descriptor,
                        };

      var addedCommands = new IContextualCommand[]
                          {
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
                          };

      var allCommands = addedCommands
        .Concat(descriptors.SelectMany(d => d.GetCommands(_editor.Document)))
        .ToDictionary(c => c.Id, c => c, StringComparer.InvariantCultureIgnoreCase);

      var converter = new KeyGestureConverter();

      var items = configuration
          .Where(s => !string.IsNullOrWhiteSpace(s))
          .Select(s => s.Trim())
          .Select(s => s.Split(new[] { " => " }, StringSplitOptions.RemoveEmptyEntries))
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