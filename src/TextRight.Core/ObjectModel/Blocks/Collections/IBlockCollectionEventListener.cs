using System;
using System.Collections.Generic;
using System.Linq;

namespace TextRight.Core.ObjectModel.Blocks.Collections
{
  /// <summary>
  ///   Convenience interface for anyone who is interested in BlockCollection events. 
  /// </summary>
  public interface IBlockCollectionEventListener : IEventListener
  {
    /// <summary> Invoked when <see cref="BlockRemovedEventArgs"/> is invoked. </summary>
    void NotifyBlockRemoved(BlockRemovedEventArgs args);
    
    /// <summary> Invoked when <see cref="BlockInsertedEventArgs"/> is invoked. </summary>
    void NotifyBlockInserted(BlockInsertedEventArgs args);
  }
}