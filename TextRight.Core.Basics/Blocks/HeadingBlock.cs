using System;
using System.Collections.Generic;
using System.Linq;
using TextRight.Core.Commands;
using TextRight.Core.ObjectModel;
using TextRight.Core.ObjectModel.Blocks;
using TextRight.Core.ObjectModel.Blocks.Text;
using TextRight.Core.ObjectModel.Serialization;
using TextRight.Core.Events;

namespace TextRight.Core.Blocks
{
  /// <summary> A block that holds text formatted as a heading. </summary>
  public sealed class HeadingBlock : TextBlock, IDocumentItem<IContentBlockView>
  {
    private int _headingLevel;

    /// <summary> Singleton-Instance of a descriptor. </summary>
    public static readonly HeadingBlockDescriptor DescriptorInstance
      = new HeadingBlockDescriptor();

    /// <inheritdoc />
    public override BlockDescriptor DescriptorHandle
      => DescriptorInstance;

    /// <summary> The level of heading that the block represents. </summary>
    [BlockProperty("HeadingLevel")]
    public int HeadingLevel
    {
      get => _headingLevel;
      set => SetValue(DescriptorInstance.HeadingLevelProperty, ref _headingLevel, value);
    }

    /// <inheritdoc/>
    public IContentBlockView Target { get; set; }

    /// <inheritdoc/>
    protected override IContentBlockView ContentBlockView
      => Target;


    public void MarkChanged<T>(IPropertyDescriptor<T> descriptor, T oldValue, T newValue)
    {
      if (Target is IChangeListener listener)
      {
        var changeEvent = new PropertyChangedEvent<T>(this, descriptor, oldValue, newValue);
        listener.HandleEvent(changeEvent);
      }
    }

    public bool SetValue<T>(IPropertyDescriptor<T> descriptor, ref T field, T value)
    {
      if (EqualityComparer<T>.Default.Equals(field, value))
        return false;

      var oldValue = field;
      field = value;

      MarkChanged(descriptor, oldValue, value);
      return true;
    }

    /// <summary> BlockDescriptor for <see cref="HeadingBlock"/>. </summary>
    public class HeadingBlockDescriptor : BlockDescriptor<HeadingBlock>
    {
      internal HeadingBlockDescriptor()
      {
        HeadingLevelProperty = RegisterProperty(it => it.HeadingLevel, "HeadingLevel");
      }

      /// <inheritdoc />
      public override string Id
        => "heading+multilevel";

      /// <see cref="HeadingLevel"/>
      public IPropertyDescriptor<int> HeadingLevelProperty { get; }

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