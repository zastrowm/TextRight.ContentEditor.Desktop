using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Documents;
using TextRight.ContentEditor.Core.ObjectModel;
using TextRight.ContentEditor.Core.ObjectModel.Blocks;
using TextRight.ContentEditor.Core.Utilities;

namespace TextRight.ContentEditor.Desktop.View
{
  /// <summary>
  ///  Associates a WPF Run with a TextSpan and keeps them in sync.
  /// </summary>
  public class StyledStyledTextSpanView : Run,
                                          IStyledTextSpanView
  {
    private readonly BaseTextBlockView _paragraphView;
    private readonly StyledTextFragment _fragment;

    /// <summary> Constructor. </summary>
    /// <param name="paragraphView"></param>
    /// <param name="fragment"> The span to keep synchronized. </param>
    public StyledStyledTextSpanView(BaseTextBlockView paragraphView, StyledTextFragment fragment)
    {
      _paragraphView = paragraphView;
      _fragment = fragment;
      _fragment.Target = this;

      TextUpdated(_fragment);
    }

    /// <inheritdoc />
    public IDocumentItem DocumentItem
      => _fragment;

    /// <summary>
    ///  This should only be set by <see cref="ParagraphView"/>.  Represents the offset into the view
    ///  that this span represents.
    /// </summary>
    internal int CharacterOffsetIntoTextView { get; set; }

    /// <inheritdoc/>
    public void TextUpdated(StyledTextFragment fragment)
    {
      Text = _fragment.Text;
      _paragraphView.MarkTextChanged(fragment);
    }

    /// <inheritdoc/>
    public MeasuredRectangle Measure(int offset)
    {
      var rect = _paragraphView.MeasureCharacter(this, offset);
      return rect;
    }

    public void Detach()
    {
      _paragraphView.MarkRemoved(this);
      // TODO we should recycle this view (pool it)?
      _fragment.Target = null;
    }
  }
}