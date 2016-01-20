using System;
using System.Collections.Generic;
using System.Linq;

namespace TextRight.ContentEditor.Core.Editing.Commands
{
  /// <summary> An editor command. </summary>
  public class EditorCommand : IEquatable<EditorCommand>
  {
    private readonly Type _owner;
    private readonly int _commandId;
    private readonly string _id;

    private const int MaxGlobalCommands = Int32.MaxValue - 1;
    private static int _globalCommandId = 1;

    public EditorCommand(string id)
    {
      _commandId = _globalCommandId;
      _globalCommandId += 1;

      _id = id;
    }

    public bool Equals(EditorCommand other)
    {
      if (ReferenceEquals(null, other)) return false;
      if (ReferenceEquals(this, other)) return true;
      return _commandId == other._commandId;
    }

    public override bool Equals(object obj)
    {
      if (ReferenceEquals(null, obj)) return false;
      if (ReferenceEquals(this, obj)) return true;
      return obj is EditorCommand && Equals((EditorCommand)obj);
    }

    public override int GetHashCode()
    {
      return _commandId;
    }

    public static bool operator ==(EditorCommand left, EditorCommand right)
    {
      return Equals(left, right);
    }

    public static bool operator !=(EditorCommand left, EditorCommand right)
    {
      return !Equals(left, right);
    }
  }
}