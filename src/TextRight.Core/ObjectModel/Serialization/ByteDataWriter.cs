using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using TextRight.Core.Events;

namespace TextRight.Core.ObjectModel.Serialization
{
  /// <summary>
  ///  An implementation of <see cref="IDataWriter"/> that writes data to a byte stream.
  /// </summary>
  public class ByteDataWriter : IPropertyWriter
  {
    private readonly BinaryWriter _binaryWriter;
    private readonly MemoryStream _stream;

    /// <summary> Default constructor. </summary>
    public ByteDataWriter()
    {
      _stream = new MemoryStream();
      _binaryWriter = new BinaryWriter(_stream, Encoding.Unicode);
    }

    /// <summary> Resets the writer so that fresh data can be written to it. </summary>
    public void Reset()
      => _stream.Position = 0;

    /// <summary>
    ///  Get the bytes that represent the data that has been written to this writer.
    /// </summary>
    /// <returns> An array that represents the data in this instance. </returns>
    public byte[] ToArray()
      => _stream.ToArray();

    /// <inheritdoc />
    void IPropertyWriter.Write(IPropertyDescriptor name, long datum)
      => _binaryWriter.Write(datum);

    /// <inheritdoc />
    void IPropertyWriter.Write(IPropertyDescriptor name, int datum)
      => _binaryWriter.Write(datum);

    /// <inheritdoc />
    void IPropertyWriter.Write(IPropertyDescriptor name, string datum)
      => _binaryWriter.Write(datum);

    /// <inheritdoc />
    void IPropertyWriter.Write(IPropertyDescriptor name, byte[] datum)
    {
      _binaryWriter.Write(datum.Length);
      _binaryWriter.Write(datum);
    }

    /// <inheritdoc />
    void IPropertyWriter.Write(IPropertyDescriptor name, double datum)
      => _binaryWriter.Write(datum);

    /// <inheritdoc />
    void IPropertyWriter.Write(IPropertyDescriptor name, bool datum)
      => _binaryWriter.Write(datum);
  }
}