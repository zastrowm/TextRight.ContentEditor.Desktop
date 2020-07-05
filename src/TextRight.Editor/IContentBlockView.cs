using System;
using System.Collections.Generic;
using System.Linq;
using TextRight.Core.ObjectModel;
using TextRight.Core.ObjectModel.Blocks;

namespace TextRight.Editor
{
  /// <summary>
  ///   The editor view for a <see cref="ContentBlock"/>
  /// </summary>
  public interface IContentBlockView : IBlockView, IEditorData
  {
    
  }
}