using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using TextRight.Core;
using TextRight.Core.Commands;

namespace TextRight.Editor.Wpf.View
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
    public void Add(Key key, IContextualCommand command)
    {
      Add(0, key, command);
    }

    /// <summary> Adds a keyboard shortcut associated with a set of commands. </summary>
    /// <param name="key"> The key to associate with the commands. </param>
    /// <param name="commands"> The commands to add. </param>
    public void Add(Key key, IContextualCommand[] commands)
    {
      foreach (var command in commands)
      {
        Add(0, key, command);
      }
    }

    /// <summary> Adds a keyboard shortcut associated with a set of commands. </summary>
    /// <param name="modifier"> The modifiers that must be present for the command.
    ///  If CTRL is specified and not SHIFT, the command will still be active when
    ///  SHIFT is held. </param>
    /// <param name="key"> The key to associate with the command. </param>
    /// <param name="commands"> The commands to add. </param>
    public void Add(ModifierKeys modifier, Key key, IContextualCommand[] commands)
    {
      foreach (var command in commands)
      {
        Add(modifier, key, command);
      }
    }

    /// <summary> Adds a keyboard shortcut associated with a command. </summary>
    /// <param name="modifier"> The modifiers that must be present for the command.
    ///  If CTRL is specified and not SHIFT, the command will still be active when
    ///  SHIFT is held. </param>
    /// <param name="key"> The key to associate with the command. </param>
    /// <param name="contextualCommand"> The command. </param>
    public void Add(ModifierKeys modifier, Key key, IContextualCommand contextualCommand)
    {
      // TODO thrown an exception on duplicate
      var newShortcut = new Shortcut(modifier, key, contextualCommand);

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
    public IContextualCommand LookupContextAction(ModifierKeys modifers, Key key, DocumentEditorContext context)
    {
      List<Shortcut> list;
      if (!_keyLookup.TryGetValue(key, out list))
        return null;

      return FindMatchOnModifierKeys(list, modifers, context);
    }

    /// <summary>
    ///  Searches this list looking for the command whose modifier keys match the passed in modifiers
    ///  and that can be enabled.
    /// </summary>
    private static IContextualCommand FindMatchOnModifierKeys(List<Shortcut> list,
                                                              ModifierKeys modifers,
                                                              DocumentEditorContext context)
    {
      foreach (var item in list)
      {
        var command = item.ContextualCommand;
        if (item.Modifers == modifers && command?.CanActivate(context) == true)
        {
          return command;
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
      public Shortcut(ModifierKeys modifers, Key key, IContextualCommand contextualCommand)
      {
        Modifers = modifers;
        Key = key;
        ContextualCommand = contextualCommand;
      }

      public ModifierKeys Modifers { get; }

      public Key Key { get; }

      public IContextualCommand ContextualCommand { get; }
    }
  }
}