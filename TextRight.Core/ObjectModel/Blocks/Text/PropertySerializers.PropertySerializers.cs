using System;
using System.Collections.Generic;
using System.Linq;
using TextRight.Core.ObjectModel.Serialization;

namespace TextRight.Core.ObjectModel.Blocks.Text
{
  /// <summary> Byte readers that can be used for serialize data. </summary>
  internal class PropertySerializers
  {
    internal static readonly ByteDataWriter DataWriter
      = new ByteDataWriter();

    internal static readonly ByteDataReader DataReader
      = new ByteDataReader();
  }
}