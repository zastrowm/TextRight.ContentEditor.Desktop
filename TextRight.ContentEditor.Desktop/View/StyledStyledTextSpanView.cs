﻿using System;
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
    private readonly TextBlockView _textBlockView;
    private readonly StyledTextFragment _fragment;

    /// <summary> Constructor. </summary>
    /// <param name="textBlockView"></param>
    /// <param name="fragment"> The span to keep synchronized. </param>
    public StyledStyledTextSpanView(TextBlockView textBlockView, StyledTextFragment fragment)
    {
      _textBlockView = textBlockView;
      _fragment = fragment;
      _fragment.Target = this;

      TextUpdated(_fragment);
    }

    /// <inheritdoc />
    public IDocumentItem DocumentItem
      => _fragment;

    /// <summary>
    ///  This should only be set by <see cref="TextBlockView"/>.  Represents the offset into the view
    ///  that this span represents.
    /// </summary>
    internal int CharacterOffsetIntoTextView { get; set; }

    /// <inheritdoc/>
    public void TextUpdated(StyledTextFragment fragment)
    {
      Text = _fragment.Text;
      _textBlockView.MarkTextChanged(fragment);
    }

    /// <inheritdoc/>
    public MeasuredRectangle Measure(int offset)
    {
      return _textBlockView.MeasureCharacter(this, offset);
    }

    public void Detach()
    {
      _textBlockView.MarkRemoved(this);
      // TODO we should recycle this view (pool it)?
      _fragment.Target = null;
    }
  }
}