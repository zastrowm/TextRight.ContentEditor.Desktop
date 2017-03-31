using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace TextRight.Core.ObjectModel.Serialization
{
  /// <summary> Block property for typeof(long). </summary>
  internal class LongBlockProperty : SimpleValueBlockProperty<long>
  {
    internal LongBlockProperty(string id, PropertyInfo propertyInfo)
      : base(id, propertyInfo)
    {
    }

    /// <inheritdoc />
    public override long Read(IDataReader reader)
      => reader.ReadInt64(Id);

    /// <inheritdoc />
    public override void Write(IDataWriter writer, long value)
      => writer.Write(Id, value);
  }

  /// <summary> Block property for typeof(int). </summary>
  internal class IntBlockProperty : SimpleValueBlockProperty<int>
  {
    internal IntBlockProperty(string id, PropertyInfo propertyInfo)
      : base(id, propertyInfo)
    {
    }

    /// <inheritdoc />
    public override int Read(IDataReader reader)
      => (int)reader.ReadInt64(Id);

    /// <inheritdoc />
    public override void Write(IDataWriter writer, int value)
      => writer.Write(Id, value);
  }

  /// <summary> Block property for typeof(double). </summary>
  internal class DoubleBlockProperty : SimpleValueBlockProperty<double>
  {
    internal DoubleBlockProperty(string id, PropertyInfo propertyInfo)
      : base(id, propertyInfo)
    {
    }

    /// <inheritdoc />
    public override double Read(IDataReader reader)
      => reader.ReadDouble(Id);

    /// <inheritdoc />
    public override void Write(IDataWriter writer, double value)
      => writer.Write(Id, value);
  }

  /// <summary> Block property for typeof(bool). </summary>
  internal class BoolBlockProperty : SimpleValueBlockProperty<bool>
  {
    internal BoolBlockProperty(string id, PropertyInfo propertyInfo)
      : base(id, propertyInfo)
    {
    }

    /// <inheritdoc />
    public override bool Read(IDataReader reader)
      => reader.ReadBool(Id);

    /// <inheritdoc />
    public override void Write(IDataWriter writer, bool value)
      => writer.Write(Id, value);
  }

  /// <summary> Block property for typeof(string). </summary>
  internal class StringBlockProperty : SimpleValueBlockProperty<string>
  {
    internal StringBlockProperty(string id, PropertyInfo propertyInfo)
      : base(id, propertyInfo)
    {
    }

    /// <inheritdoc />
    public override string Read(IDataReader reader)
      => reader.ReadString(Id);

    /// <inheritdoc />
    public override void Write(IDataWriter writer, string value)
      => writer.Write(Id, value);
  }

}