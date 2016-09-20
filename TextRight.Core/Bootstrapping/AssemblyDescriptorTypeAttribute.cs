using System;
using System.Collections.Generic;
using System.Linq;

namespace TextRight.Core.Bootstrapping
{
  /// <summary> Attribute for assembly descriptor type. </summary>
  [AttributeUsage(AttributeTargets.Assembly)]
  public class AssemblyDescriptorTypeAttribute : Attribute
  {
    private readonly Type _type;

    /// <summary> Constructor. </summary>
    /// <param name="type"> The type that should be constructed to get the registered descriptors for
    ///  the assembly.  Must be of type <see cref="IAssemblyDescriptorFactory"/>. </param>
    public AssemblyDescriptorTypeAttribute(Type type)
    {
      _type = type;
    }

    /// <summary> Retrieves an instance of the type provided to the attribute. </summary>
    public IAssemblyDescriptorFactory GetInstance()
      => (IAssemblyDescriptorFactory)Activator.CreateInstance(_type);
  }
}