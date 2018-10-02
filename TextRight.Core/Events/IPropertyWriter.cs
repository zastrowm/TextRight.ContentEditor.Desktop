using System;
using System.Collections.Generic;
using System.Linq;

namespace TextRight.Core.Events
{
  /// <summary> Interface for a serializer that can write out various properties. </summary>
  public interface IPropertyWriter
  {
    void Write(IPropertyDescriptor id, int datum);
    void Write(IPropertyDescriptor id, long datum);
    void Write(IPropertyDescriptor id, string datum);
    void Write(IPropertyDescriptor id, byte[] datum);
    void Write(IPropertyDescriptor id, double datum);
    void Write(IPropertyDescriptor id, bool datum);
  }
}