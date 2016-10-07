using System;
using System.Collections.Generic;
using System.Linq;
using TextRight.Core.Bootstrapping;
using TextRight.Core.ObjectModel.Blocks;
using TextRight.Core.ObjectModel.Blocks.Collections;
using TextRight.Core.ObjectModel.Blocks.Text;

[assembly: AssemblyDescriptorType(typeof(CoreFactory))]

namespace TextRight.Core.Bootstrapping
{
  /// <summary> Contains the assemblies that exist in the core assembly. </summary>
  public class CoreFactory : IAssemblyDescriptorFactory
  {
    /// <inheritdoc/>
    public IEnumerable<RegisteredDescriptor> GetRegisteredDescriptors()
    {
      yield return RootBlockCollection.RegisteredDescriptor;
      yield return ParagraphBlock.RegisteredDescriptor;
      yield return HeadingBlock.RegisteredDescriptor;
    }
  }
}