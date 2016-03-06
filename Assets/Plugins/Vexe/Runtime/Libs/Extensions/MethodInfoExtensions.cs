using System;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;

namespace Vexe.Runtime.Extensions
{
    public static class MethodInfoExtensions
    {
        /// <summary>
        /// Invokes this method on the specified target with no arguments
        /// </summary>
        public static object Invoke(this MethodInfo method, object target)
        {
            return method.Invoke(target, null);
        }

        /// <summary>
        /// Skips the first parameter if this was an extension method then returns the remaining parameters
        /// returns the whole parameters otherwise
        /// </summary>
        public static ParameterInfo[] GetActualParams(this MethodInfo method)
        {
            return method.IsExtensionMethod() ? method.GetParameters().Skip(1).ToArray() : method.GetParameters();
        }

        /// <summary>
        /// Returns the specified method's full name "methodName(argType1 arg1, argType2 arg2, etc)"
        /// Uses the specified gauntlet to replaces type names, ex: "int" instead of "Int32"
        /// </summary>
        public static string GetFullName(this MethodInfo method, string extensionMethodPrefix)
        {
            var builder = new StringBuilder();
            bool isExtensionMethod = method.IsExtensionMethod();
            if (isExtensionMethod)
                builder.Append(extensionMethodPrefix);
            builder.Append(method.Name);
            builder.Append("(");
            builder.Append(method.GetParamsNames());
            builder.Append(")");
            return builder.ToString();
        }

        /// <summary>
        /// Returns a string representing the passed method parameters names. Ex "int num, float damage, Transform target"
        /// </summary>
        public static string GetParamsNames(this MethodInfo method)
        {
            ParameterInfo[] pinfos = method.IsExtensionMethod() ? method.GetParameters().Skip(1).ToArray() : method.GetParameters();
            var builder = new StringBuilder();
            for (int i = 0, len = pinfos.Length; i < len; i++)
            {
                var param = pinfos[i];
                var pTypeName = param.ParameterType.GetNiceName();
                builder.Append(pTypeName);
                builder.Append(" ");
                builder.Append(param.Name);
                if (i < len - 1)
                    builder.Append(", ");
            }
            return builder.ToString();
        }

        public static string GetFullName(this MethodInfo method)
        {
            return GetFullName(method, "[ext] ");
        }

        // http://stackoverflow.com/questions/4168489/methodinfo-equality-for-declaring-type
        public static bool AreMethodsEqualForDeclaringType(this MethodInfo first, MethodInfo second)
        {
            first = first.ReflectedType == first.DeclaringType ? first : first.DeclaringType.GetMethod(first.Name, first.GetParameters().Select(p => p.ParameterType).ToArray());
            second = second.ReflectedType == second.DeclaringType ? second : second.DeclaringType.GetMethod(second.Name, second.GetParameters().Select(p => p.ParameterType).ToArray());
            return first == second;
        }

        public static bool IsDeclaredIn(this MethodInfo method, Type type)
        {
            return method.DeclaringType == type;
        }

        public static bool IsExtensionMethod(this MethodInfo method)
        {
            var mType = method.DeclaringType;
            return mType.IsSealed &&
                !mType.IsGenericType &&
                !mType.IsNested &&
                method.IsDefined(typeof(ExtensionAttribute), false);
        }
    }
}