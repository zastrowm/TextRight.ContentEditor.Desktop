using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TextRight.ContentEditor.Core.ObjectModel.Blocks;

namespace TextRight.ContentEditor.Core.Tests.ObjectModel.Blocks
{
  /// <summary> A BlockCollection that can be initialized via collection intializer. </summary>
  public class AddableBlockCollection : VerticalBlockCollection,
                                        IEnumerable
  {
    public IEnumerator GetEnumerator()
    {
      throw new NotImplementedException();
    }
  }
}