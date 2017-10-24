using System;
using System.Collections.Generic;
using System.Linq;
using TextRight.Core.Commands;
using TextRight.Core.Editing.Commands;
using TextRight.Core.ObjectModel;
using TextRight.Core.ObjectModel.Blocks;
using TextRight.Core.ObjectModel.Blocks.Text;
using TextRight.Core.ObjectModel.Serialization;

namespace TextRight.Core.Blocks
{
  /// <summary> A block that holds text formatted as a heading. </summary>
  public sealed class HeadingBlock : TextBlock, IDocumentItem<IContentBlockView>
  {
    private int _headingLevel;

    /// <summary> Singleton-Instance of a descriptor. </summary>
    public static readonly RegisteredDescriptor DescriptorInstance
      = RegisteredDescriptor.Register<HeadingBlockDescriptor>();

    /// <inheritdoc />
    public override RegisteredDescriptor DescriptorHandle
      => DescriptorInstance;

    /// <summary> The level of heading that the block represents. </summary>
    [BlockProperty("HeadingLevel")]
    public int HeadingLevel
    {
      get => _headingLevel;
      set
      {
        if (_headingLevel == value)
          return;

        int oldLevel = _headingLevel;
        _headingLevel = value;

        FireEvent(new HeadingLevelChangedEventArgs(oldLevel, value));
      }
    }

    /// <inheritdoc/>
    public IContentBlockView Target { get; set; }

    /// <inheritdoc/>
    protected override IContentBlockView ContentBlockView
      => Target;

    /// <summary> Invoked when the heading level changes. </summary>
    public class HeadingLevelChangedEventArgs : EventEmitterArgs<IHeadingBlockListener>
    {
      public HeadingLevelChangedEventArgs(int oldLevel, int newLevel)
      {
        OldLevel = oldLevel;
        NewLevel = newLevel;
      }

      public int OldLevel { get; }

      public int NewLevel { get; }

      protected override void Handle(object sender, IHeadingBlockListener reciever)
      {
        reciever.NotifyLevelChanged(OldLevel, NewLevel);
      }
    }

    /// <summary> View interface for <see cref="HeadingBlock"/> </summary>
    public interface IHeadingBlockListener : IEventListener
    {
      /// <summary>
      ///  Indicates that the heading level of the associated heading-block has changed.
      /// </summary>
      void NotifyLevelChanged(int oldLevel, int newLevel);
    }

    /// <summary> BlockDescriptor for <see cref="HeadingBlock"/>. </summary>
    private class HeadingBlockDescriptor : BlockDescriptor<HeadingBlock>
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