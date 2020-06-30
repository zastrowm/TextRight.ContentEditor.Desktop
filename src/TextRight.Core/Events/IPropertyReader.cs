using System;
using System.Collections.Generic;
using System.Linq;

namespace TextRight.Core.Events
{
  /// <summary> Interface for a serializer that can read in various properties. </summary>
  public interface IPropertyReader
  {
    bool TryRead(IPropertyDescriptor id, out int value);
    bool TryRead(IPropertyDescriptor id, out long value);
    bool TryRead(IPropertyDescriptor id, out string value);
    bool TryRead(IPropertyDescriptor id, out byte[] value);
    bool TryRead(IPropertyDescriptor id, out double value);
    bool TryRead(IPropertyDescriptor id, out bool value);
  }
}