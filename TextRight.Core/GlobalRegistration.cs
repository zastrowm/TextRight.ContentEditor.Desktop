using System;
using System.Collections.Generic;
using System.Linq;
using TextRight.Core.Events;

namespace TextRight.Core
{
  /// <summary> Global services available to the app domain. </summary>
  internal static class GlobalRegistration
  {
    /// <summary> All value serializers that should be available to property converters. </summary>
    public static ValueSerializerCollection ValueSerializers { get; }
      = new ValueSerializerCollection();
  }
}