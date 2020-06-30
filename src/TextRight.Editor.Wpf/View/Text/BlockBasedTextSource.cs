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
      = 16;

    /// <inheritdoc />
    public override TextRun GetTextRun(int desiredCharacterIndex)
    {
      if (_block.Content.TextLength > desiredCharacterIndex)
      {
        var props = CreateTextSpanRunProperties();
        return CreateCharactersObject(desiredCharacterIndex,
                                      _block.Content,
                                      props);
      }

      return new TextEndOfParagraph(1);
    }

    internal TextSpanRunProperties CreateTextSpanRunProperties()
      => new TextSpanRunProperties(FontSize);

    public static TextCharacters CreateCharactersObject(
      int characterStartIndex,
      TextBlockContent content,
      TextSpanRunProperties properties
      )
    {
      return new TextCharacters(content.GetText(),
                                characterStartIndex,
                                content.TextLength - characterStartIndex,
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