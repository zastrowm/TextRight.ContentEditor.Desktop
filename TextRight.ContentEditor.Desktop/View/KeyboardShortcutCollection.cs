using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using TextRight.ContentEditor.Core.Editing.Commands;

namespace TextRight.ContentEditor.Desktop.View
{
  /// <summary> Contains a collection of keyboard shortcuts. </summary>
  public class KeyboardShortcutCollection : IEnumerable<KeyboardShortcutCollection.Shortcut>
  {
    private readonly Dictionary<Key, List<Shortcut>> _keyLookup;

    /// <summary> Default constructor. </summary>
    public KeyboardShortcutCollection()
    {
      _keyLookup = new Dictionary<Key, List<Shortcut>>();
    }

    /// <summary> Adds a keyboard shortcut associated with a command. </summary>
    /// <param name="key"> The key to associate with the command. </param>
    /// <param name="command"> The command. </param>
    public void Add(Key key, EditorCommand command)
    {
      Add(0, key, command);
    }

    /// <summary> Adds a keyboard shortcut associated with a command. </summary>
    /// <param name="modifier"> The modifiers that must be present for the command.
    ///  If CTRL is specified and not SHIFT, the command will still be active when
    ///  SHIFT is held. </param>
    /// <param name="key"> The key to associate with the command. </param>
    /// <param name="command"> The command. </param>
    public void Add(ModifierKeys modifier, Key key, EditorCommand command)
    {
      // TODO thrown an exception on duplicate
      var newShortcut = new Shortcut(modifier, key, command);

      List<Shortcut> existingValue;

      if (_keyLookup.TryGetValue(key, out existingValue))
      {
        existingValue.Add(newShortcut);
      }
      else
      {
        _keyLookup[key] = new List<Shortcut>
                          {
                            newShortcut
                          };
      }
    }

    /// <summary>
    ///  Finds the command associated with the given key/modifiers.
    /// </summary>
    /// <param name="modifers"> The current modifiers. If CTRL and SHIFT are
    ///  present, but no keyboard shortcut is associated with the given shortcut
    ///  but there is a shortcut with CTRL without SHIFT, the latter shortcut will
    ///  be returned. </param>
    /// <param name="key"> The key to associate with the command. </param>
    /// <returns> The command associated with the modifier keys/key. </returns>
    public EditorCommand Lookup(ModifierKeys modifers, Key key)
    {
      List<Shortcut> list;
      if (!_keyLookup.TryGetValue(key, out list))
        return null;

      foreach (var item in list)
      {
        if (item.Modifers == modifers)
        {
          return item.Command;
        }
      }

      // if it's a shortcut based on Control, then try the search without the SHIFT (this
      // allows navigation shortcuts without explicitly needing duplicate commands
      // for extending selection
      if (modifers.HasFlag(ModifierKeys.Control) && modifers.HasFlag(ModifierKeys.Shift))
      {
        modifers &= ~ModifierKeys.Shift;

        foreach (var item in list)
        {
          if (item.Modifers == modifers)
          {
            return item.Command;
          }
        }
      }

      return null;
    }

    /// <inheritdoc />
    public IEnumerator<Shortcut> GetEnumerator()
    {
      return _keyLookup.SelectMany(kvp => kvp.Value).GetEnumerator();
    }

    /// <inheritdoc />
    IEnumerator IEnumerable.GetEnumerator()
    {
      return GetEnumerator();
    }

    /// <summary> Data storage for a keyboard shortcut. </summary>
    public class Shortcut
    {
      public Shortcut(ModifierKeys modifers, Key key, EditorCommand command)
      {
        Modifers = modifers;
        Key = key;
        Command = command;
      }

      public ModifierKeys Modifers { get; }

      public Key Key { get; }

      public EditorCommand Command { get; }
    }
  }
}