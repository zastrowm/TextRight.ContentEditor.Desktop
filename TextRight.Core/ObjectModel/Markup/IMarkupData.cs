using System;
using System.Collections.Generic;
using System.Linq;

namespace TextRight.Core.ObjectModel
{
  public interface IMarkupData
  {
    IMarkupDescriptor Owner { get; }
  }
}