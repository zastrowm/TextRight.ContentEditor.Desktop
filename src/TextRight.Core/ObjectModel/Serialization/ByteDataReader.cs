using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using TextRight.Core.Events;

namespace TextRight.Core.ObjectModel.Serialization
{
  /// <summary>
  ///  Implementation of <see cref="IDataReader"/> that reads data from byte stream.
  /// </summary>
  public class ByteDataReader : IPropertyReader
  {
    private readonly BinaryReader _binaryReader;
    private readonly MemoryStream _stream;

    /// <summary> Default constructor. </summary>
    public ByteDataReader()
    {
      _stream = new MemoryStream();
      _binaryReader = new BinaryReader(_stream, Encoding.Unicode);
    }

    /// <summary> Resets the reader to read from the specified array. </summary>
    /// <param name="data"> The data to read. </param>
    public void Reset(byte[] data)
    {
      _stream.Position = 0;
      _stream.Write(data, 0, data.Length);
      _stream.Position = 0;
    }

    /// <inheritdoc />
    bool IPropertyReader.TryRead(IPropertyDescriptor name, out long value)
    {
      value = _binaryReader.ReadInt64();
      return true;
    }

    /// <inheritdoc />
    bool IPropertyReader.TryRead(IPropertyDescriptor name, out int value)
    {
      value = _binaryReader.ReadInt32();
      return true;
    }

    /// <inheritdoc />
    bool IPropertyReader.TryRead(IPropertyDescriptor name, out byte[] bytes)
    {
      int size = _binaryReader.ReadInt32();
      bytes = _binaryReader.ReadBytes(size);
      return true;
    }

    /// <inheritdoc />
    bool IPropertyReader.TryRead(IPropertyDescriptor name, out double value)
    {
      value = _binaryReader.ReadDouble();
      return true;
    }

    /// <inheritdoc />
    bool IPropertyReader.TryRead(IPropertyDescriptor name, out bool value)
    {
      value = _binaryReader.ReadBoolean();
      return true;
    }

    /// <inheritdoc />
    bool IPropertyReader.TryRead(IPropertyDescriptor name, out string value)
    {
      value = _binaryReader.ReadString();
      return true;
    }
  }
}