﻿using System;
using System.Collections.Generic;
using System.Linq;
using TextRight.ContentEditor.Core.Editing;
using TextRight.ContentEditor.Core.ObjectModel.Serialization;

namespace TextRight.ContentEditor.Core.ObjectModel.Blocks
{
  /// <summary> View interface for <see cref="HeadingBlock"/> </summary>
  public interface IHeadingBlockView : ITextBlockView
  {
    /// <summary>
    ///  Indicates that the heading level of the associated heading-block has changed.
    /// </summary>
    void NotifyLevelChanged();
  }

  /// <summary> A block that holds text formatted as a heading. </summary>
  public sealed class HeadingBlock : TextBlockBase<IHeadingBlockView>
  {
    private int _headingLevel;

    /// <summary> Singleton-Instance of a descriptor. </summary>
    public static readonly RegisteredDescriptor RegisteredDescriptor
      = RegisteredDescriptor.Register<BlockDescriptor>();

    /// <inheritdoc />
    public override RegisteredDescriptor Descriptor
      => RegisteredDescriptor;

    /// <inheritdoc/>
    protected override TextBlock SuperClone()
    {
      return new HeadingBlock();
    }

    /// <inheritdoc/>
    protected override void SerializeToNode(SerializeNode node)
    {
      node.Attributes.Add("HeadingLevel", HeadingLevel.ToString());
    }

    /// <summary> The level of heading that the block represents. </summary>
    public int HeadingLevel
    {
      get { return _headingLevel; }
      set
      {
        _headingLevel = value;
        Target?.NotifyLevelChanged();
      }
    }

    /// <inheritdoc />
    public override TextBlockAttributes GetAttributes()
    {
      return new Attributes(this);
    }

    /// <summary />
    private class Attributes : TextBlockAttributes
    {
      private readonly int _level;

      public Attributes(HeadingBlock block)
      {
        _level = block.HeadingLevel;
      }

      public override TextBlock CreateInstance()
      {
        return new HeadingBlock()
               {
                 HeadingLevel = _level
               };
      }
    }

    /// <summary> BlockDescriptor for <see cref="HeadingBlock"/>. </summary>
    private class BlockDescriptor : ContentBlockDescriptor<HeadingBlock>
    {
      /// <inheritdoc />
      public override string Id
        => "heading+multilevel";

      /// <inheritdoc />
      public override IEnumerable<IContextualCommand> GetCommands(DocumentOwner document)
      {
        yield return new ConvertToLevelHeadingCommand(1);
        yield return new ConvertToLevelHeadingCommand(2);
        yield return new ConvertToLevelHeadingCommand(3);
        yield return new ConvertToLevelHeadingCommand(4);
        yield return new ConvertToLevelHeadingCommand(5);
      }
    }
  }
}