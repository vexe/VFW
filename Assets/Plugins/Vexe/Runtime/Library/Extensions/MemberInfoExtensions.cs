using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Vexe.Runtime.Extensions
{
    public static class MemberInfoExtensions
    {
        public static bool IsDefined(this MemberInfo minfo, Type type)
        {
            return minfo.IsDefined(type, false);
        }

        /// <summary>
        /// Returns true if the attribute whose type is specified by the generic argument is defined on this member
        /// </summary>
        public static bool IsDefined<T>(this MemberInfo member, bool inherit) where T : Attribute
        {
            return member.IsDefined(typeof(T), inherit);
        }

        /// <summary>
        /// Returns true if the attribute whose type is specified by the generic argument is defined on this member
        /// </summary>
        public static bool IsDefined<T>(this MemberInfo member) where T : Attribute
        {
            return IsDefined<T>(member, false);
        }

        /// <summary>
        /// Returns PropertyInfo.PropertyType if the info was a PropertyInfo,
        /// FieldInfo.FieldType if FieldInfo, otherwise MethodInfo.ReturnType
        /// </summary>
        public static Type GetDataType(this MemberInfo memberInfo, Func<MemberInfo, Type> fallbackType)
        {
            var field = memberInfo as FieldInfo;
            if (field != null)
                return field.FieldType;

            var property = memberInfo as PropertyInfo;
            if (property != null)
                return property.PropertyType;

            var method = memberInfo as MethodInfo;
            if (method != null)
                return method.ReturnType;

            if (fallbackType == null)
                throw new InvalidOperationException("Member is not a field, property, method nor does it have a fallback type");

            return fallbackType(memberInfo);
        }

        public static Type GetDataType(this MemberInfo memberInfo)
        {
            return GetDataType(memberInfo, null);
        }

        /// <summary>
        /// Returns the first found custom attribute of type T on this member
        /// Returns null if none was found
        /// </summary>
        public static T GetCustomAttribute<T>(this MemberInfo member, bool inherit) where T : Attribute
        {
            var all = GetCustomAttributes<T>(member, inherit).ToArray();
            return all.IsNullOrEmpty() ? null : all[0];
        }

        /// <summary>
        /// Returns the first found non-inherited custom attribute of type T on this member
        /// Returns null if none was found
        /// </summary>
        public static T GetCustomAttribute<T>(this MemberInfo member) where T : Attribute
        {
            return GetCustomAttribute<T>(member, false);
        }

        public static IEnumerable<T> GetCustomAttributes<T>(this MemberInfo member) where T : Attribute
        {
            return GetCustomAttributes<T>(member, false);
        }

        public static IEnumerable<T> GetCustomAttributes<T>(this MemberInfo member, bool inherit) where T : Attribute
        {
            return member.GetCustomAttributes(typeof(T), inherit).Cast<T>();
        }

        public static Attribute[] GetAttributes(this MemberInfo member)
        {
            return member.GetCustomAttributes<Attribute>().ToArray();
        }

        /// <summary>
        /// If this member is a method, returns the full method name (name + params) otherwise the member name paskal splitted
        /// </summary>
        public static string GetNiceName(this MemberInfo member)
        {
            var method = member as MethodInfo;
            if (method != null)
            {
                return method.GetFullName();
            }
            string name = member.Name;
            if (string.IsNullOrEmpty(name)) return name;
            return name[0] == '_' ? name.Remove(0, 1).SplitPascalCase() : name.SplitPascalCase();
        }


        public static bool IsStatic(this MemberInfo member)
        {
            var field = member as FieldInfo;
            if (field != null)
                return field.IsStatic;

            var property = member as PropertyInfo;
            if (property != null)
                return property.CanRead ? property.GetGetMethod(true).IsStatic : property.GetSetMethod(true).IsStatic;

            var method = member as MethodInfo;
            if (method != null)
                return method.IsStatic;

            string message = string.Format("Unable to determine IsStatic for member {0}.{1}" +
                "MemberType was {2} but only fields, properties and methods are supported.",
                member.Name, member.MemberType, Environment.NewLine);

            throw new NotSupportedException(message);
        }
    }
}