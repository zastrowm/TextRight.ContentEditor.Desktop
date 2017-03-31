using System;
using System.Collections.Generic;
using System.Linq;
using TextRight.Core.ObjectModel.Blocks;

namespace TextRight.Core.ObjectModel.Serialization
{
  public interface ICustomSerialized
  {
    void Read(IDataReader reader);
    void Write(IDataWriter writer);
  }

  public interface IDataReader
  {
    long ReadInt64(string name);
    string ReadString(string name);
    byte[] ReadBytes(string name);
    void ReadBytes(string name, ArraySegment<byte> arraySegment);
    double ReadDouble(string name);
    bool ReadBool(string name);
  }

  public interface IDataWriter
  {
    void Write(string name, long datum);
    void Write(string name, string datum);
    void Write(string name, byte[] datum);
    void Write(string name, ArraySegment<byte> data);
    void Write(string name, double datum);
    void Write(string name, bool datum);
  }

  public static class DataReaderWriterExtensions
  {
    public static void Read(this IDataReader reader, ICustomSerialized serializable) 
      => serializable.Read(reader);

    public static void Write(this IDataWriter writer, ICustomSerialized serializable)
      => serializable.Write(writer);
  }
}