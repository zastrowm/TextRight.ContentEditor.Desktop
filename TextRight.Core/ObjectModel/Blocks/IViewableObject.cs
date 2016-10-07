using System;
using System.Collections.Generic;
using System.Linq;

namespace TextRight.Core.ObjectModel.Blocks
{
  /// <summary> An object that may have an associated view that it notifies of changes. </summary>
  /// <typeparam name="T"> Generic type parameter. </typeparam>
  public interface IViewableObject<out T>
    where T : class
  {
    /// <summary> The view associated with the object. </summary>
    T Target { get; }
  }
}