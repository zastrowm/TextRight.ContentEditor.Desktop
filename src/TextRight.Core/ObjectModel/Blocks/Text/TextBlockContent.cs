using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using JetBrains.Annotations;
using TextRight.Core.ObjectModel.Blocks.Text.View;
using TextRight.Core.ObjectModel.Serialization;

namespace TextRight.Core.ObjectModel.Blocks.Text
{
  /// <summary>
  ///  Contains various <see cref="TextSpan"/> parts that is presumed to be part of a
  ///  larger block.
  /// </summary>
  public sealed class TextBlockContent : DocumentItem,
                                         IDocumentItem<ITextBlockContentView>
  {
    private readonly StringFragmentBuffer _buffer;

    /// <summary> TextBlockContent constructor. </summary>
    public TextBlockContent()
      : this("")
    {
    }
    
    /// <summary> TextBlockContent constructor. </summary>
    internal TextBlockContent(TextBlockContent content)
      : this(content.GetText())
    {
    }
    
    /// <summary> TextBlockContent constructor. </summary>
    private TextBlockContent(string text)
    {
      _buffer = new StringFragmentBuffer(text);
    }

    public TextBlock Owner { get; internal set; }
    
    /// <summary> The buffer associated with this fragment.. </summary>
    internal StringFragmentBuffer Buffer
      => _buffer;
    
    /// <summary> The total number of graphemes in this fragment. </summary>
    public int GraphemeLength
      => _buffer.GraphemeLength;
    
    /// <inheritdoc />
    protected override EventEmitter ParentEmitter
      => Owner;

    /// <summary> Gets a cursor that is looking at the beginning of this content. </summary>
    public TextCaret GetCaretAtStart()
      => TextCaret.FromBeginning(this);

    /// <summary> Gets a cursor that is looking at the end of this content. </summary>
    public TextCaret GetCaretAtEnd()
      => TextCaret.FromEnd(this);

    /// <summary> Retrieves a cursor that points at the given character. </summary>
    /// <exception cref="Exception"> Thrown when an exception error condition occurs. </exception>
    /// <param name="graphemeIndex"> The index of the grapheme to point at. </param>
    /// <returns> A TextBlockValueCursor that is pointing at the given grapheme. </returns>
    public TextCaret CursorFromGraphemeIndex(int graphemeIndex)
    {
      if (graphemeIndex < 0 || graphemeIndex > _buffer.GraphemeLength)
        throw new ArgumentException($"Invalid index for cursor; index={graphemeIndex}; maximum={_buffer.GraphemeLength}", nameof(graphemeIndex));

      return TextCaret.FromOffset(this, graphemeIndex);
    }

    /// <summary> Inserts the given content into this instance. </summary>
    /// <param name="caret"> The caret which represents the point at which the content should be
    ///  inserted. </param>
    /// <param name="newContent"> The content to insert. </param>
    /// <param name="autoMerge"> (Optional) True to automatically merge similar fragments together. </param>
    public TextCaret Insert(TextCaret caret, TextBlockContent newContent, bool autoMerge = true)
    {
      if (newContent == this)
        throw new ArgumentException("Content cannot be inserted into itself", nameof(newContent));

      return Insert(caret, newContent.GetText());
    }

    /// <summary> Inserts the given content into this instance. </summary>
    /// <param name="caret"> The caret which represents the point at which the content should be
    ///  inserted. </param>
    /// <param name="text"> The text that should be inserted into the text fragment </param>
    public TextCaret Insert(TextCaret caret, string text)
    {
      if (caret.Content != this)
        throw new ArgumentException("Caret does not refer to this Content instance", nameof(caret));
      if (text == null)
          throw new ArgumentNullException(nameof(text));

      // easy, it's empty
      if (text == "")
        return caret;

      var originalNumGraphemes = _buffer.GraphemeLength;      
      _buffer.InsertText(caret.Offset.CharOffset, text);
      var nowNumGraphemes = _buffer.GraphemeLength;

      NotifyChanged();
      
      return TextCaret.FromOffset(this, caret.Offset.GraphemeOffset + nowNumGraphemes - originalNumGraphemes);
    }

    /// <summary> Removes all of the given text. </summary>
    public void RemoveAll()
    {
      _buffer.DeleteAllText();
      NotifyChanged();
    }
    
    /// <summary> Removes the given number of characters. </summary>
    public void DeleteText(TextOffset start, TextOffset end)
    {
      var numberOfCharactersToRemove = end.CharOffset - start.CharOffset;
      if (numberOfCharactersToRemove == 0)
        return;
    
      _buffer.DeleteText(start, end);
      NotifyChanged();
    }
    
    /// <summary> Retrieves the text within the fragment. </summary>
    public string GetText()
      => _buffer.GetText();

    /// <summary> The number of characters in this content. </summary>
    public int TextLength
      => _buffer.NumberOfChars;

    /// <summary> Extracts the textual content of this block into a separate content object. </summary>
    /// <param name="start"> The position at which extraction should start. </param>
    /// <param name="end"> The position at which the content extraction should end. </param>
    /// <returns> The extracted content. </returns>
    public TextBlockContent ExtractContent(TextCaret start, TextCaret end)
      => ExtractOrCloneContent(start, end, shouldRemoveContent: true);

    public TextBlockContent CloneContent(TextCaret start, TextCaret end)
      => ExtractOrCloneContent(start, end, shouldRemoveContent: false);

    public TextBlockContent ExtractOrCloneContent(TextCaret caretStart, TextCaret caretEnd, bool shouldRemoveContent)
    {
      VerifyExtractParameters(caretStart, caretEnd);

      NormalizePositioning(ref caretStart, ref caretEnd);

      // zero-width; this check is needed, as the normalization process might shift the end to be
      // before the start when both are pointing at the end of a fragment (the start is normalized to
      // point at the beginning of the next fragment instead). 
      if (caretStart == caretEnd)
        return new TextBlockContent();

      var start = caretStart.Offset;
      var end = caretEnd.Offset;

      if (start == end)
      {
        return new TextBlockContent();
      }
      else if (caretStart.IsAtBlockStart && caretEnd.IsAtBlockEnd)
      {
        var newContent = new TextBlockContent(this);

        if (shouldRemoveContent)
        {
          RemoveAll();
        }

        return newContent;
      }
      else
      {
        var clone = new TextBlockContent(_buffer.GetText(start, end));

        if (shouldRemoveContent)
        {
          _buffer.DeleteText(start, end);
          NotifyChanged();
        }

        return clone;
      }
    }

    [AssertionMethod]
    private void VerifyExtractParameters(TextCaret caretStart,
                                         TextCaret caretEnd)
    {
      if (!caretStart.IsValid || caretStart.Content != this)
        throw new ArgumentException("Start cursor is not pointing at this content", nameof(caretStart));
      if (!caretEnd.IsValid || caretStart.Content != this)
        throw new ArgumentException("End cursor is not pointing at this content", nameof(caretEnd));
    }

    /// <summary> Makes sure that <paramref name="caretStart"/> comes before <paramref name="caretEnd"/>. </summary>
    private void NormalizePositioning(ref TextCaret caretStart, ref TextCaret caretEnd)
    {
      if (caretStart.Offset.GraphemeOffset > caretEnd.Offset.GraphemeOffset)
      {
        var temp = caretStart;
        caretStart = caretEnd;
        caretEnd = temp;
      }
    }

    // TODO

    public TextBlockContent Clone() 
      => CloneContent(TextCaret.FromBeginning(this), TextCaret.FromEnd(this));

    /// <summary />
    public void SerializeInto(SerializeNode node)
    {
      node.AddData("Body", _buffer.GetText());
    }

    /// <summary />
    public void Deserialize(SerializationContext context, SerializeNode node)
    {
      var text = node.GetDataOrDefault<string>("Body");
      Insert(GetCaretAtStart(), new TextBlockContent(text), autoMerge: false);
    }

    /// <inheritdoc />
    IDocumentItemView IDocumentItem.DocumentItemView
      => Target;

    /// <inheritdoc />
    public ITextBlockContentView Target { get; set; }

    /// <summary> Notifies listeners that the given fragment has changed. </summary>
    internal void NotifyChanged()
    {
      FireEvent(new TextBlockContentChangedEventArgs(this));
    }

    /// <summary> EventArgs for when the text inside of a StyledTextFragment is changed. </summary>
    public class TextBlockContentChangedEventArgs : EventEmitterArgs<ITextBlockContentEventListener>
    {
      public TextBlockContentChangedEventArgs(TextBlockContent changedBlockContent)
      {
        ChangedBlockContent = changedBlockContent;
      }

      public TextBlockContent ChangedBlockContent { get; }

      /// <inheritdoc />
      protected override void Handle(object sender, ITextBlockContentEventListener listener)
        => listener.NotifyTextChanged(ChangedBlockContent);
    }
  }
}