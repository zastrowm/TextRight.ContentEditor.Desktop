using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Windows.Media;
using System.Windows.Media.TextFormatting;
using TextRight.Core.ObjectModel.Blocks.Text;

namespace TextRight.Editor.Wpf.View
{
  /// <summary> Provides formatted text from a TextBlock. </summary>
  internal class BlockBasedTextSource : TextSource
  {
    private readonly TextBlock _block;

    public BlockBasedTextSource(TextBlock block)
    {
      _block = block;
    }

    public int FontSize { get; set; }
      = 24;

    /// <inheritdoc />
    public override TextRun GetTextRun(int desiredCharacterIndex)
    {
      int startIndex = 0;
      var fragment = _block.Content.FirstSpan;

      while (fragment != null)
      {
        int endIndex = startIndex + fragment.NumberOfChars;

        if (endIndex <= desiredCharacterIndex)
          break;

        var props = CreateTextSpanRunProperties();

        if (startIndex <= desiredCharacterIndex)
          return CreateCharactersObject(desiredCharacterIndex,
                                        fragment, startIndex, endIndex,
                                        props);

        startIndex += fragment.NumberOfChars;
        fragment = fragment.Next;
      }

      return new TextEndOfParagraph(1);
    }

    internal TextSpanRunProperties CreateTextSpanRunProperties()
      => new TextSpanRunProperties(FontSize);

    public static TextCharacters CreateCharactersObject(
      int characterStartIndex,
      TextSpan span,
      int startIndex, 
      int endIndex, 
      TextSpanRunProperties properties
      )
    {
      return new TextCharacters(span.GetText(),
                                characterStartIndex - startIndex,
                                endIndex - characterStartIndex,
                                properties);
    }

    /// <inheritdoc />
    public override TextSpan<CultureSpecificCharacterBufferRange> GetPrecedingText(int textSourceCharacterIndexLimit)
    {
      Debug.Fail("When is this called");
      // TODO?
      return new TextSpan<CultureSpecificCharacterBufferRange>(
        0,
        new CultureSpecificCharacterBufferRange(CultureInfo.CurrentCulture, CharacterBufferRange.Empty));
    }

    /// <inheritdoc />
    public override int GetTextEffectCharacterIndexFromTextSourceCharacterIndex(int textSourceCharacterIndex)
    {
      Debug.Fail("When is this called");
      return 0;
    }
  }
}