using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using JetBrains.Annotations;
using TextRight.ContentEditor.Core.Utilities;

namespace TextRight.ContentEditor.Core.ObjectModel.Blocks
{
  /// <summary> Hosts the view for the TextSpan. </summary>
  public interface IStyledTextSpanView
  {
    /// <summary> Invoked when the TextSpan's text changes. </summary>
    /// <param name="fragment"> The span whose text has changed. </param>
    void TextUpdated(StyledTextFragment fragment);

    /// <summary> Measures the text at the given location. </summary>
    /// <param name="offset"> The offset at which the text should be measured. </param>
    MeasuredRectangle Measure(int offset);

    /// <summary> Notifies the view that the representation is detached from the original TextBlock. </summary>
    void Detach();
  }

  /// <summary>
  ///  Contains a span of single run of text that is styled or has some sort of other data
  ///  associated with it.
  /// </summary>
  [DebuggerDisplay("StyledTextFragment(text={Text}, Index={Index})")]
  public class StyledTextFragment : IViewableObject<IStyledTextSpanView>,
                                    IEquatable<StyledTextFragment>
  {
    /// <summary> Default constructor. </summary>
    public StyledTextFragment(string text)
    {
      Text = text;
      Index = -1;
    }

    /// <summary> The index of the span within a TextBlock. </summary>
    public int Index { get; internal set; }

    /// <summary> The TextBlock that owns the span. </summary>
    public TextBlock Parent { get; internal set; }

    /// <summary> The sibling fragment that follows this fragment. </summary>
    public StyledTextFragment Next { get; internal set; }

    /// <summary> The sibling fragment that this fragment follows. </summary>
    public StyledTextFragment Previous { get; internal set; }

    /// <summary> The text in the span. </summary>
    public string Text
    {
      get { return _text; }
      internal set
      {
        _text = value;
        Target?.TextUpdated(this);
      }
    }

    private string _text;

    /// <summary> The number of characters in the TextSpan. </summary>
    public int Length
      => Text.Length;

    /// <summary> Makes a deep copy of this instance. </summary>
    /// <returns> A copy of this instance. </returns>
    public StyledTextFragment Clone()
    {
      return new StyledTextFragment(_text);
    }

    /// <summary>
    ///  The object that receives all notifications of changes from this instance.
    /// </summary>
    [CanBeNull]
    public IStyledTextSpanView Target { get; set; }

    /// <summary> Detach the fragment from any views. </summary>
    public void Detach()
    {
      Target?.Detach();
    }

    /// <inheritdoc />
    public bool Equals(StyledTextFragment other)
    {
      if (ReferenceEquals(null, other))
        return false;
      if (ReferenceEquals(this, other))
        return true;

      return string.Equals(_text, other._text) && Index == other.Index;
    }

    /// <inheritdoc />
    public override bool Equals(object obj)
    {
      if (ReferenceEquals(null, obj))
        return false;
      if (ReferenceEquals(this, obj))
        return true;
      if (obj.GetType() != GetType())
        return false;
      return Equals((StyledTextFragment)obj);
    }

    /// <inheritdoc />
    public override int GetHashCode()
    {
      unchecked
      {
        return (_text.GetHashCode() * 397) ^ Index;
      }
    }
  }
}