using System;
using System.Collections.Generic;
using System.Linq;

namespace TextRight.Core.ObjectModel
{
  public interface IMarkupDescriptor
  {
    MarkupId MarkupId { get; }
    
    MarkupInvalidationResult HandleInvalidated(Markup markup);

    MarkupChangeBehavior ExpandBehavior { get; }
    
    MarkupChangeBehavior ShrinkBehavior { get; }
    
    MarkupChangeBehavior ShrinkToEmptyBehavior { get; }
  }

  public enum MarkupInvalidationResult
  {
    Keep,
    Remove,
  }

  public enum MarkupChangeBehavior
  {
    None,
    Invalidate,
    Delete,
  }
}