using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;

namespace TextRight.Core.ObjectModel.Blocks.Text
{
  /// <summary> String methods that are accessed via reflection. </summary>
  internal static class InternalStringMethods
  {
    private static readonly GetUnicodeCategoryDelegate GetUnicodeCategoryCallback =
      GetDelegate<GetUnicodeCategoryDelegate>(typeof(CharUnicodeInfo), "InternalGetUnicodeCategory", 3);

    private static readonly GetCurrentTextElementLenDelegate GetCurrentTextElementLenCallback =
      GetDelegate<GetCurrentTextElementLenDelegate>(typeof(StringInfo), "GetCurrentTextElementLen", 5);

    // http://referencesource.microsoft.com/#mscorlib/system/globalization/charunicodeinfo.cs,510
    public static UnicodeCategory GetUnicodeCategory(string str, int index, out int charLength)
      => GetUnicodeCategoryCallback.Invoke(str, index, out charLength);

    // http://referencesource.microsoft.com/#mscorlib/system/globalization/stringinfo.cs,25f98b41c5f354a1
    public static int GetCurrentTextElementLen(string str,
                                               int index,
                                               int len,
                                               ref UnicodeCategory ucCurrent,
                                               ref int currentCharCount)
      => GetCurrentTextElementLenCallback(str, index, len, ref ucCurrent, ref currentCharCount);

    /// <summary>
    ///  Gets a delegate which represents the internal static method with the given name on the given
    ///  type.
    /// </summary>
    private static T GetDelegate<T>(Type type, string methodName, int numArguments)
    {
      return (T)(object)type.GetTypeInfo().GetMethods(BindingFlags.Static | BindingFlags.NonPublic)
                            .Where(m => m.Name == methodName)
                            .Single(m => m.GetParameters().Length == numArguments)
                            .CreateDelegate(typeof(T));
    }

    internal delegate UnicodeCategory GetUnicodeCategoryDelegate(string str, int index, out int charLength);

    internal delegate int GetCurrentTextElementLenDelegate(string str,
                                                           int index,
                                                           int len,
                                                           ref UnicodeCategory ucCurrent,
                                                           ref int currentCharCount);
  }
}