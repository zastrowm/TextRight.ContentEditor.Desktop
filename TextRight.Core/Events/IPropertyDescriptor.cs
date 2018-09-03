using System;
using System.Collections.Generic;
using System.Linq;

namespace TextRight.Core.Events
{
  /// <summary> A description of a property for a specific class. </summary>
  public interface IPropertyDescriptor
  {
    string Id { get; }
    Type DataType { get; }
  }
}