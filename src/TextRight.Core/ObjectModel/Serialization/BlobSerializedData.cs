using System;
using System.Collections.Generic;
using System.Linq;

namespace TextRight.Core.ObjectModel.Serialization
{
  /// <summary> Data that has been serialized into a blob format. </summary>
  public struct BlobSerializedData
  {
    internal byte[] Data;

    internal BlobSerializedData(byte[] data)
    {
      Data = data;
    }

    /// <summary> True if the structure contains data that can be deserialized. </summary>
    public bool IsValid
      => Data != null;
  }
}