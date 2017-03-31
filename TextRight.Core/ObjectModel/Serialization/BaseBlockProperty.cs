using System;
using System.Collections.Generic;
using System.Linq;
using TextRight.Core.ObjectModel.Blocks;

namespace TextRight.Core.ObjectModel.Serialization
{
  /// <summary> Base class for all block properties. </summary>
  public abstract class BaseBlockProperty
  {
    /// <summary> Reads the data from the given reader and places the data into the block. </summary>
    /// <param name="block"> The block to which the properties from <paramref name="reader"/> will be
    ///  written. </param>
    /// <param name="reader"> The reader providing the data for the property(s) for the block. </param>
    public abstract void Read(Block block, IDataReader reader);

    /// <summary> Writes the properties from the given block into the given writer. </summary>
    /// <param name="block"> The block from which the properties that are to be written are retreived. </param>
    /// <param name="writer"> The writer to which the propert(ies) should be written. </param>
    public abstract void Write(Block block, IDataWriter writer);
  }
}