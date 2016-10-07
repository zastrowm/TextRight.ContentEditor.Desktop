using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TextRight.Core.ObjectModel.Blocks.Collections;

namespace TextRight.Core.Tests.ObjectModel.Blocks
{
  /// <summary> A BlockCollection that can be initialized via collection intializer. </summary>
  public class AddableBlockCollection : RootBlockCollection,
                                        IEnumerable
  {
    public IEnumerator GetEnumerator()
    {
      throw new NotImplementedException();
    }
  }
}