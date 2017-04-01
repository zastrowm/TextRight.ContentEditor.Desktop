using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TextRight.Core.Actions;
using TextRight.Core.Blocks;
using TextRight.Core.Editing;
using TextRight.Core.Editing.Actions;
using TextRight.Core.Editing.Commands;
using TextRight.Core.ObjectModel.Blocks.Text;

namespace TextRight.Core.Commands
{
  /// <summary> Converts a given TextBlock into a heading. </summary>
  public class ConvertToLevelHeadingCommand : IContextualCommand
  {
    private readonly int _level;

    /// <summary> Constructor. </summary>
    /// <param name="level"> The desired level to convert the blocks into. </param>
    public ConvertToLevelHeadingCommand(int level)
    {
      _level = level;
    }

    /// <inheritdoc />
    public string Id
      => "heading.convertTo" + _level;

    /// <inheritdoc/>
    public string GetName(DocumentEditorContext context)
      => $"Convert to level {_level} heading.";

    /// <inheritdoc/>
    public string GetDescription(DocumentEditorContext context)
      => $"Converts the given block into a level {_level} heading";

    /// <inheritdoc/>
    public bool CanActivate(DocumentEditorContext context)
    {
      var block = context.Cursor.Block;
      var headingBlock = block as HeadingBlock;

      return (headingBlock != null && headingBlock.HeadingLevel != _level) || block is ParagraphBlock;
    }

    /// <inheritdoc/>
    public void Activate(DocumentEditorContext context, IActionStack actionStack)
    {
      actionStack.Do(new ConvertTextBlockIntoHeadingAction(context.Cursor, _level));
    }
  }
}