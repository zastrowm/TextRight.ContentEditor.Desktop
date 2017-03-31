using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace TextRight.Core.ObjectModel.Serialization
{
  /// <summary>
  ///  Implementation of <see cref="IDataReader"/> that reads data from byte stream.
  /// </summary>
  public class ByteDataReader : IDataReader
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
    long IDataReader.ReadInt64(string name)
      => _binaryReader.ReadInt64();

    /// <inheritdoc />
    string IDataReader.ReadString(string name)
      => _binaryReader.ReadString();

    /// <inheritdoc />
    byte[] IDataReader.ReadBytes(string name)
    {
      int length = _binaryReader.ReadInt32();
      return _binaryReader.ReadBytes(length);
    }

    /// <inheritdoc />
    void IDataReader.ReadBytes(string name, ArraySegment<byte> arraySegment)
    {
      // not currently used
      long length = _binaryReader.ReadInt32();

      if (arraySegment.Count != length)
        throw new ArgumentException($"Actual length ({length}) does not match expected length ({arraySegment.Count}.",
                                    nameof(arraySegment));

      var bytes = _binaryReader.ReadBytes(arraySegment.Count);
      bytes.CopyTo(arraySegment.Array, arraySegment.Offset);
    }

    /// <inheritdoc />
    double IDataReader.ReadDouble(string name)
      => _binaryReader.ReadDouble();

    /// <inheritdoc />
    bool IDataReader.ReadBool(string name)
      => _binaryReader.ReadBoolean();
  }
}