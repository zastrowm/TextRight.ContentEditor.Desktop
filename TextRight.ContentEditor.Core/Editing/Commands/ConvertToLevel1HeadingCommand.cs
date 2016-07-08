using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TextRight.ContentEditor.Core.Editing.Actions;
using TextRight.ContentEditor.Core.ObjectModel.Blocks;

namespace TextRight.ContentEditor.Core.Editing
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
    public void Activate(DocumentEditorContext context, ActionStack actionStack)
    {
      actionStack.Do(new ConvertTextBlockIntoHeadingAction(context.Cursor, _level));
    }
  }
}