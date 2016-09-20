using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TextRight.ContentEditor.Core.ObjectModel.Blocks;

namespace TextRight.Core.Bootstrapping
{
  /// <summary> Retrieves all of the descriptors exposed in the assembly. </summary>
  public interface IAssemblyDescriptorFactory
  {
    /// <summary> Gets the registered descriptors for the assembly. </summary>
    /// <returns> The registered descriptors. </returns>
    IEnumerable<RegisteredDescriptor> GetRegisteredDescriptors();
  }
}