using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TextRight.ContentEditor.Desktop.ObjectModel
{
  /// <summary> An element within a DocumentTree. </summary>
  public class DocumentElement<TEnum>
  {
    /// <summary> Editor specific data that can be stored to any object. </summary>
    public INotifee<TEnum> AssociatedNotifiee { get; set; }

    public void Notify(TEnum changeType)
    {
      AssociatedNotifiee?.MarkChange(this, changeType);
    }
  }

  /// <summary> AN object that wants to be notified of changes from a DocumentElement. </summary>
  public interface INotifee<TEnum>
  {
    /// <summary> Mark change. </summary>
    /// <param name="element"> The element that notifying the notifee of the change. </param>
    /// <param name="changeType"> Type of the change. </param>
    void MarkChange(DocumentElement<TEnum> element, TEnum changeType);
  }
}