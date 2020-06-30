using System;
using System.Collections.Generic;
using System.Composition;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TextRight.Core.Addons;
using TextRight.Core.ObjectModel;
using TextRight.Core.ObjectModel.Blocks;
using TextRight.Core.ObjectModel.Blocks.Collections;
using TextRight.Core.ObjectModel.Blocks.Text;

namespace TextRight.Core.Additions
{
  [Export(typeof(DocumentAddon))]
  [ExportMetadata("Name", "Text.SpellCheck")]
  public class TextBlockSpellCheckAddon : DocumentAddon,
                                          ITextBlockContentEventListener,
                                          IMarkupDescriptor
  {
    private readonly HashSet<TextBlock> _waitingBlocks;
    private readonly DocumentEditorContext _context;

    private TextBlock _currentBlockBeingProcessed;
    private CancellationTokenSource _currentlyProcessingToken;

    /// <summary>
    ///   The shared id to use for all instances of the descriptor.
    /// </summary>
    public static MarkupId SharedId { get; }
      = MarkupIdRegistry.RegisterNew("Text.SpellCheck");

    [ImportingConstructor]
    public TextBlockSpellCheckAddon(DocumentEditorContext context)
    {
      _context = context;
      _waitingBlocks = new HashSet<TextBlock>();
      _context.SubscribeListener(this);
    }

    /// <inheritdoc />
    public MarkupId MarkupId
      => SharedId;

    private void ReParse(TextBlock textBlock)
    {
      _waitingBlocks.Add(textBlock);
    }

    private async Task StartProcessing()
    {
      _currentlyProcessingToken?.Dispose();
      _currentlyProcessingToken = new CancellationTokenSource();

        var block = _waitingBlocks.FirstOrDefault();
      if (block == null)
        return;

      // TODO

      _waitingBlocks.Remove(block);
      _currentBlockBeingProcessed = block;

      try
      {
        var wordBuilder = new StringBuilder();
        var caret = block.GetCaretAtStart().As<TextCaret>();

        while (!_currentlyProcessingToken.IsCancellationRequested
               && !caret.IsAtBlockEnd)
        {
          var currentCharacter = caret.CharacterAfter.Character;

          if (char.IsWhiteSpace(currentCharacter)
              || char.IsSeparator(currentCharacter)
              || currentCharacter == '.')
          {

            // TODO
            wordBuilder.Clear();
          }
          else
          {
            wordBuilder.Append(currentCharacter);
          }
        }

        caret = caret.GetNextPosition();
      }
      finally
      {
        _currentBlockBeingProcessed = null;
      }

      //block.AddSideChannelData(new SpellCheckSideData())

      return;
    }
    
    
    public void NotifyTextChanged(TextBlockContent changedContent)
      => ReParse(changedContent.Owner);

    public MarkupInvalidationResult HandleInvalidated(Markup markup)
    {
      // TODO
      return MarkupInvalidationResult.Remove;
    }

    public MarkupChangeBehavior ExpandBehavior
      => MarkupChangeBehavior.Invalidate;

    public MarkupChangeBehavior ShrinkBehavior
      => MarkupChangeBehavior.Invalidate;

    public MarkupChangeBehavior ShrinkToEmptyBehavior
      => MarkupChangeBehavior.Delete;
  }
}
