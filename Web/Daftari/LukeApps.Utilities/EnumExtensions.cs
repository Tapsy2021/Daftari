using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Web.Mvc;

namespace LukeApps.Utilities
{
    public static class EnumExtensions
    {
        public static string GetDisplay<T>(this T value)
            where T : struct
        {
            var fieldInfo = value.GetType().GetField(value.ToString());

            if (fieldInfo.GetCustomAttributes(typeof(DisplayAttribute), false) is DisplayAttribute[] displayAttributes)
                return (displayAttributes.Length > 0) ? displayAttributes[0].Name : value.ToString();
            else
                return string.Empty;
        }
        public static string GetDescription<T>(this T value)
            where T : struct
        {
            var fieldInfo = value.GetType().GetField(value.ToString());

            if (fieldInfo.GetCustomAttributes(typeof(DisplayAttribute), false) is DisplayAttribute[] displayAttributes)
                return (displayAttributes.Length > 0) ? displayAttributes[0].Description : value.ToString();
            else
                return string.Empty;
        }

        public static SelectList ToSelectList<TEnum>(this TEnum enumObj)
            where TEnum : struct, IComparable, IFormattable, IConvertible
        {
            var values = from TEnum e in Enum.GetValues(typeof(TEnum))
                         select new { Id = e, Name = e.GetDisplay() };
            return new SelectList(values, "Id", "Name", enumObj);
        }

        public static string GetPropertyDisplay<T>(this T value, Expression<Func<T, object>> propertyExpression)
        {
            var memberInfo = GetPropertyInformation(propertyExpression.Body);
            if (memberInfo == null)
            {
                throw new ArgumentException(
                    "No property reference expression was found.",
                    "propertyExpression");
            }

            if (memberInfo.GetCustomAttributes(typeof(DisplayAttribute), false) is DisplayAttribute[] displayAttributes)
                return (displayAttributes.Length > 0) ? displayAttributes[0].Name : value.ToString();
            else
                return memberInfo.Name;
        }

        public static string GetPropertyShortDisplay<T>(this T value, Expression<Func<T, object>> propertyExpression)
        {
            var memberInfo = GetPropertyInformation(propertyExpression.Body);
            if (memberInfo == null)
            {
                throw new ArgumentException(
                    "No property reference expression was found.",
                    "propertyExpression");
            }

            if (memberInfo.GetCustomAttributes(typeof(DisplayAttribute), false) is DisplayAttribute[] displayAttributes)
                return (displayAttributes.Length > 0) ? displayAttributes[0].ShortName : value.ToString();
            else
                return memberInfo.Name;
        }

        public static MemberInfo GetPropertyInformation(Expression propertyExpression)
        {
            MemberExpression memberExpr = propertyExpression as MemberExpression;
            if (memberExpr == null)
            {
                UnaryExpression unaryExpr = propertyExpression as UnaryExpression;
                if (unaryExpr != null && unaryExpr.NodeType == ExpressionType.Convert)
                {
                    memberExpr = unaryExpr.Operand as MemberExpression;
                }
            }

            if (memberExpr != null && memberExpr.Member.MemberType == MemberTypes.Property)
            {
                return memberExpr.Member;
            }

            return null;
        }
    }
}