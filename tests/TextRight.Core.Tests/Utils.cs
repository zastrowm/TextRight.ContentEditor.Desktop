using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;

namespace TextRight.Core.Tests
{
  /// <summary> Utility helpers. </summary>
  public static class Utils
  {
    public static PropertyInfo GetPropertyInfo<TProperty>(Expression<Func<TProperty>> propertyLambda)
    {
      MemberExpression member = propertyLambda.Body as MemberExpression;
      if (member == null)
        throw new ArgumentException(String.Format(
                                      "Expression '{0}' refers to a method, not a property.",
                                      propertyLambda.ToString()));

      PropertyInfo propInfo = member.Member as PropertyInfo;
      if (propInfo == null)
        throw new ArgumentException(String.Format(
                                      "Expression '{0}' refers to a field, not a property.",
                                      propertyLambda.ToString()));

      return propInfo;
    }

    public static FieldInfo GetFieldInfo<TProperty>(Expression<Func<TProperty>> fieldLambda)
    {
      MemberExpression member = fieldLambda.Body as MemberExpression;
      if (member == null)
        throw new ArgumentException(String.Format(
                                      "Expression '{0}' refers to a method, not a property.",
                                      fieldLambda.ToString()));

      FieldInfo fieldInfo = member.Member as FieldInfo;
      if (fieldInfo == null)
        throw new ArgumentException($"Expression '{fieldLambda.ToString()}' refers to a property, not a field.");

      return fieldInfo;
    }
  }
}