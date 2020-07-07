using System;
using System.Linq;
using System.Collections.Generic;
using TextRight.Core.ObjectModel.Blocks.Collections;

namespace TextRight.Editor
{
  /// <summary>
  ///   A view for <see cref="BlockCollection"/>.
  /// </summary>
  public interface IBlockCollectionView : IBlockCollectionEventListener, IBlockView
  {
    
  }
}