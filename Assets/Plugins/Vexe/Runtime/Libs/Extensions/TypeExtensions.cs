using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using UnityEngine;

namespace Vexe.Runtime.Extensions
{
    public static class TypeExtensions
    {
        public static bool IsNumeric(this Type type)
        {
            if (type == typeof(bool) || type == typeof(char))
                return false;
            return type.IsPrimitive;
        }

        public static bool TryGetSequenceElementType(this Type type, out Type element)
        {
            if (type.IsArray)
            {
                element = type.GetElementType();
                return true;
            }

            if (!type.IsIList())
            { 
                element = null;
                return false;
            }

            element = type.GetProperty("Item").PropertyType;
            return true;
        }

        public static bool IsStatic(this Type type)
        {
            return type.IsSealed && type.IsAbstract;
        }

        public static object Instance(this Type type)
        {
            var ctor = type.DelegateForCtor<object>(Type.EmptyTypes);
            return ctor(null);
        }

        public static T Instance<T>(this Type type)
        {
            return Instance<T>(type, Type.EmptyTypes);
        }

        public static T Instance<T>(this Type type, Type[] paramTypes)
        {
            var ctor = type.DelegateForCtor<object>(paramTypes);
            return (T)ctor(null);
        }

        public static MethodCaller<object, object> DelegateForCall(this Type type, string name, BindingFlags flags, params Type[] paramTypes)
        {
            var method = type.GetMethod(name, flags, null, paramTypes, null);
            if (method == null)
                throw new Exception("Method not found " + name);

            return method.DelegateForCall();
        }

        public static MethodCaller<object, object> DelegateForCall(this Type type, string name, params Type[] paramTypes)
        {
            return DelegateForCall(type, name, Flags.InstanceAnyVisibility, paramTypes);
        }

        public static bool IsMethodImplemented(this Type src, string methodName, Type[] paramTypes)
        {
            var method = src.GetMethod(methodName, paramTypes);
            if (method == null)
                return false;

            var result = method.DeclaringType != typeof(object);
            return result;
        }

        public static MemberInfo GetMemberFromAll(this Type type, string memberName, Type peak, BindingFlags flags)
        {
            var result = GetAllMembers(type, peak, flags).FirstOrDefault(x => x.Name == memberName);
            return result;
        }

        public static MemberInfo GetMemberFromAll(this Type type, string memberName, BindingFlags flags)
        {
            var peak = type.IsA<MonoBehaviour>() ? typeof(MonoBehaviour)
                     : type.IsA<ScriptableObject>() ? typeof(ScriptableObject)
                     : typeof(object);

            return GetMemberFromAll(type, memberName, peak, flags);
        }

        /// <summary>
        /// Returns all members (including private ones) from this type till peak
        /// http://stackoverflow.com/questions/1155529/not-getting-fields-from-gettype-getfields-with-bindingflag-default/1155549#1155549
        /// </summary>
        public static IEnumerable<MemberInfo> GetAllMembers(this Type type, Type peak, BindingFlags flags)
        {
            if (type == null || type == peak)
                return Enumerable.Empty<MemberInfo>();

            return type.GetMembers(flags).Concat(GetAllMembers(type.BaseType, peak, flags));
        }

        public static IEnumerable<MemberInfo> GetAllMembers(this Type type, Type peak)
        {
            var flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly | BindingFlags.Static;
            return GetAllMembers(type, peak, flags);
        }

        public static IEnumerable<MemberInfo> GetAllMembers(this Type type)
        {
            return GetAllMembers(type, typeof(object));
        }

        public static object ActivatorInstance(this Type type)
        {
            return Activator.CreateInstance(type);
        }

        public static T ActivatorInstance<T>(this Type type)
        {
            return (T)ActivatorInstance(type);
        }

        public static bool AnyDefined<T>(this IEnumerable<Attribute> attributes) where T : Attribute
        {
            return attributes.Any(a => a.GetType().IsDefined<T>());
        }

        public static T GetAttribute<T>(this IEnumerable<Attribute> attributes) where T : Attribute
        {
            return attributes.OfType<T>().FirstOrDefault() as T;
        }

        public static Attribute[] GetAttributes(this Type type)
        {
            return type.GetCustomAttributes<Attribute>().ToArray();
        }

        public static bool IsConstructedGenType(this Type type)
        {
            return !(string.IsNullOrEmpty(type.FullName) && !type.IsGenericTypeDefinition && type.ContainsGenericParameters);
        }

        /// <summary>
        /// Returns the generic arguments of the specified parent type def
        /// </summary>
        public static Type[] GetParentGenericArguments(this Type type, Type parentDefinition)
        {
            return type.GetBaseClasses()
                          .First(c => c.IsGenericType && c.GetGenericTypeDefinition() == parentDefinition)
                          .GetGenericArguments();
        }

        /// <summary>
        /// Returns true if this type implements ICollection
        /// </summary>
        public static bool IsCollection(this Type type)
        {
            return type.IsA<ICollection>();
        }

        /// <summary>
        /// Returns the generic arguments of the first generic base type of this type
        /// </summary>
        public static Type[] FirstBaseGenArgs(this Type type)
        {
            var genBase = GetBaseClasses(type).FirstOrDefault(t => t.IsGenericType);
            return genBase == null ? Type.EmptyTypes : genBase.GetGenericArguments();
        }

        /// <summary>
        /// Returns a lazy enumerable of all the base types of this type including interfaces and classes
        /// </summary>
        public static IEnumerable<Type> GetBaseTypes(this Type type)
        {
            return GetBaseClasses(type).Concat(type.GetInterfaces());
        }

        /// <summary>
        /// Returns a lazy enumerable of all the base classes of this type
        /// </summary>
        public static IEnumerable<Type> GetBaseClasses(this Type type)
        {
            if (type == null || type.BaseType == null)
                yield break;

            var current = type.BaseType;
            while (current != null)
            {
                yield return current;
                current = current.BaseType;
            }
        }

        /// <summary>
        /// Returns true if this type is abstract but not an interface
        /// </summary>
        public static bool IsAbstractNotInterface(this Type type)
        {
            return type.IsAbstract && !type.IsInterface;
        }

        public static readonly Dictionary<string, string> TypeNameAlternatives = new Dictionary<string, string>()
        {
            { "Single"   , "float"    },
            { "Int32"    , "int"      },
            { "String"   , "string"   },
            { "Boolean"  , "bool"     },
            { "Single[]" , "float[]"  },
            { "Int32[]"  , "int[]"    },
            { "String[]" , "string[]" },
            { "Boolean[]", "bool[]"   }
        };

        /// <summary>
        /// Used to filter out unwanted type names. Ex "int" instead of "Int32"
        /// </summary>
        public static string TypeNameGauntlet(this Type type)
        {
            string typeName = type.Name;
            if (typeName == "Object") // Could be a UnityEngine.Object, or System.Object - avoid confusion
            {
                typeName = typeof(UnityEngine.Object) == type ? "UnityObject" : "object";
            }
            else
            {
                string altTypeName = string.Empty;
                if (TypeNameAlternatives.TryGetValue(typeName, out altTypeName))
                    typeName = altTypeName;
            }
            return typeName;
        }

        private static Func<Type, string> _getNiceName;
        private static Func<Type, string> getNiceName
        {
            get
            {
                return _getNiceName ?? (_getNiceName = new Func<Type, string>(type =>
                {
                    if (type.IsArray)
                    {
                        int rank = type.GetArrayRank();
                        return type.GetElementType().GetNiceName() + (rank == 1 ? "[]" : "[,]");
                    }

                    if (type.IsSubclassOfRawGeneric(typeof(Nullable<>)))
                        return type.GetGenericArguments()[0].GetNiceName() + "?";

                    if (type.IsGenericParameter || !type.IsGenericType)
                        return TypeNameGauntlet(type);

                    var builder = new StringBuilder();
                    var name = type.Name;
                    var index = name.IndexOf("`");
                    builder.Append(name.Substring(0, index));
                    builder.Append('<');
                    var args = type.GetGenericArguments();
                    for (int i = 0; i < args.Length; i++)
                    {
                        var arg = args[i];
                        if (i != 0)
                        {
                            builder.Append(", ");
                        }
                        builder.Append(GetNiceName(arg));
                    }
                    builder.Append('>');
                    return builder.ToString();
                }).Memoize());
            }
        }

        /// <summary>
        /// Ex: typeof(Dictionary<int, string>) => "Dictionary<int, string>"
        /// Credits to @jaredpar: http://stackoverflow.com/questions/401681/how-can-i-get-the-correct-text-definition-of-a-generic-type-using-reflection
        /// </summary>
        public static string GetNiceName(this Type type)
        {
            return getNiceName(type);
        }

        /// <summary>
        /// Returns true if this type is a sturct
        /// </summary>
        public static bool IsStruct(this Type type)
        {
            return type.IsValueType && !type.IsEnum && !type.IsPrimitive;
        }

        /// <summary>
        /// Returns the empty constructor of this type object
        /// </summary>
        public static ConstructorInfo GetEmptyConstructor(this Type type)
        {
            return type.GetConstructor(Type.EmptyTypes);
        }

        /// <summary>
        /// Generic version of IsSubclassOf(Type)
        /// </summary>
        public static bool IsSubclassOf<T>(this Type type)
        {
            return type.IsSubclassOf(typeof(T));
        }

        /// <summary>
        /// Returns all members declared in T and all of its derivatives if includeGenArgMembers was true
        /// otherwise just the derivatives of T using the specified bindings
        /// </summary>
        public static IEnumerable<MemberInfo> GetMembersBeneath<T>(this Type type, BindingFlags flags, bool includeGenArgMembers = false)
        {
            return type.GetMembers(flags).Where(m => includeGenArgMembers ? m.DeclaringType.IsA<T>() : m.DeclaringType.IsSubclassOf<T>());
        }

        /// <summary>
        /// Returns all members declared in T and all of its derivatives if includeGenArgMembers was true
        /// otherwise just the derivatives of T
        /// Uses the following BindingFlags: Instance | NonPublic | Public
        /// </summary>
        public static IEnumerable<MemberInfo> GetMembersBeneath<T>(this Type type, bool includeGenArgMembers = false)
        {
            return GetMembersBeneath<T>(type, Flags.AllMembers, includeGenArgMembers);
        }

        /// <summary>
        /// Returns all members declared in T and all of its derivatives if includeGenArgMembers was true
        /// otherwise just the derivatives of T using the specified bindings
        /// </summary>
        public static IEnumerable<MemberInfo> GetMembersBeneath(this Type type, Type other, BindingFlags flags, bool includeGenArgMembers = false)
        {
            return type.GetMembers(flags).Where(m => includeGenArgMembers ? m.DeclaringType.IsA(other) : m.DeclaringType.IsSubclassOf(other));
        }

        /// <summary>
        /// Returns all members declared in T and all of its derivatives if includeGenArgMembers was true
        /// otherwise just the derivatives of T
        /// Uses the following BindingFlags: Instance | NonPublic | Public
        /// </summary>
        public static IEnumerable<MemberInfo> GetMembersBeneath(this Type type, Type other, bool includeGenArgMembers = false)
        {
            return GetMembersBeneath(type, other, Flags.AllMembers, includeGenArgMembers);
        }

        /// <summary>
        /// Returns true if the type exists within the hierarchy chain of the specified generic type
        /// (is equal to it or a subclass of it)
        /// </summary>
        public static bool IsA<T>(this Type type)
        {
            return type.IsA(typeof(T));
        }

        /// <summary>
        /// Returns true if the type exists within the hierarchy chain of the specified type
        /// (is equal to it or a subclass of it)
        /// </summary>
        public static bool IsA(this Type type, Type other)
        {
            return other.IsAssignableFrom(type);
        }

        /// <summary>
        /// Returns the first found custom attribute of type T on this type
        /// Returns null if none was found
        /// </summary>
        public static T GetCustomAttribute<T>(this Type type, bool inherit) where T : Attribute
        {
            var all = GetCustomAttributes<T>(type, inherit).ToArray();
            return all.IsNullOrEmpty() ? null : all[0];
        }

        /// <summary>
        /// Returns the first found non-inherited custom attribute of type T on this type
        /// Returns null if none was found
        /// </summary>
        public static T GetCustomAttribute<T>(this Type type) where T : Attribute
        {
            return GetCustomAttribute<T>(type, false);
        }

        public static IEnumerable<T> GetCustomAttributes<T>(this Type type) where T : Attribute
        {
            return GetCustomAttributes<T>(type, false);
        }

        public static IEnumerable<T> GetCustomAttributes<T>(this Type type, bool inherit) where T : Attribute
        {
            return type.GetCustomAttributes(typeof(T), inherit).Cast<T>();
        }

        /// <summary>
        /// Returns true if the attribute whose type is specified by the generic argument is defined on this type
        /// </summary>
        public static bool IsDefined<T>(this Type type) where T : Attribute
        {
            return type.IsDefined(typeof(T));
        }

        /// <summary>
        /// Alternative version of <see cref="Type.IsSubclassOf"/> that supports raw generic types (generic types without
        /// any type parameters).
        /// </summary>
        /// <param name="baseType">The base type class for which the check is made.</param>
        /// <param name="toCheck">To type to determine for whether it derives from <paramref name="baseType"/>.</param>
        /// Credits to JaredPar: http://stackoverflow.com/questions/457676/check-if-a-class-is-derived-from-a-generic-class
        public static bool IsSubclassOfRawGeneric(this Type toCheck, Type baseType)
        {
            while (toCheck != typeof(object) && toCheck != null)
            {
                Type current = toCheck.IsGenericType ? toCheck.GetGenericTypeDefinition() : toCheck;
                if (current == baseType)
                    return true;
                toCheck = toCheck.BaseType;
            }
            return false;
        }

        /// <summary>
        /// Similar to IsSubclassOfRawGeneric, but for interfaces
        /// Ex: typeof(List<int>).IsImplementerOfRawGeneric(typeof(IList<>)) is true
        /// </summary>
        public static bool IsImplementerOfRawGeneric(this Type type, Type baseType)
        {
            return type.GetInterfaces().Any(interfaceType =>
            {
                var current = interfaceType.IsGenericType ? interfaceType.GetGenericTypeDefinition() : interfaceType;
                return current == baseType;
            });
        }

        /// <summary>
        /// Returns true if 'type' is an implementer or subclass of raw generic of 'baseType'
        /// </summary>
        public static bool IsSubclassOrImplementerOfRawGeneric(this Type type, Type baseType)
        {
            return IsSubclassOfRawGeneric(type, baseType) || IsImplementerOfRawGeneric(type, baseType);
        }

        // http://stackoverflow.com/questions/2490244/default-value-of-a-type-at-runtime
        public static object GetDefaultValue(this Type t)
        {
            return t.IsValueType ? Activator.CreateInstance(t) : null;
        }

        public static object GetDefaultValueEmptyIfString(this Type t)
        {
            return t == typeof(string) ? string.Empty : t.GetDefaultValue();
        }

        public static bool IsIList(this Type type)
        {
            return typeof(IList).IsAssignableFrom(type);
        }

        public static Type[] GetGenericArgsInThisOrAbove(this Type type)
        {
            while (!type.IsGenericType && type != typeof(object))
                type = type.BaseType;

            if (type == typeof(object))
                return Type.EmptyTypes;
            return type.GetGenericArguments();
        }

        public static Type[] GetGenericArgsInRawParents(this Type source, Type rawParentType)
        {
            if (!rawParentType.IsGenericTypeDefinition)
                return Type.EmptyTypes;

            Type baseType = source.BaseType;

            while (baseType.GetGenericTypeDefinition() != rawParentType && baseType != typeof(object))
                baseType = baseType.BaseType;

            return baseType == typeof(object) ? Type.EmptyTypes : baseType.GetGenericArguments();
        }

        public static MethodInfo GetMethod(this Type source, string methodName, Type[] paramTypes, BindingFlags flags)
        {
            return source.GetMethod(methodName, flags, null, paramTypes, null);
        }

        public static IEnumerable<MethodInfo> GetExtensionMethods(
            this Type forType, Assembly[] asms, Type higherType,
            Type returnType, Type[] paramTypes,
            BindingFlags modifiers, bool exactBinding)
        {
            IEnumerable<MethodInfo> extMethods = Enumerable.Empty<MethodInfo>();
            foreach (var asm in asms)
                extMethods = extMethods.Concat(forType.GetExtensionMethods(asm, higherType, returnType, paramTypes, modifiers, exactBinding));
            return extMethods;
        }

        public static IEnumerable<MethodInfo> GetExtensionMethods(
            this Type forType, Assembly asm, Type higherType,
            Type returnType, Type[] paramTypes,
            BindingFlags modifiers, bool exactBinding)
        {
            return asm.GetTypes()
                .Where(t => t.IsSealed && !t.IsGenericType && !t.IsNested)
                .SelectMany(t => t.GetMethods(BindingFlags.Static | modifiers))
                .Where(m => m.IsDefined(typeof(ExtensionAttribute), false))
                .Where(m => m.ReturnType == returnType)
                .Select(m => new
                {
                    Method = m,
                    ExtParamTypes = m.GetParameters().Select(p => p.ParameterType).ToArray(),
                })
                .Where(x =>
                {
                    // the 'this' param
                    var thisParamType = x.ExtParamTypes[0];
                    bool validBinding = (exactBinding ? thisParamType == forType :
                                         thisParamType.IsAssignableFrom(forType) &&
                                            higherType.IsAssignableFrom(thisParamType));

                    if (paramTypes == null)
                        return validBinding;

                    bool equalLength = paramTypes.Length == x.ExtParamTypes.Length - 1;
                    return equalLength && (exactBinding ? validBinding &&
                                                            x.ExtParamTypes.Skip(1).ToArray().IsEqualTo(paramTypes) :
                                                          validBinding &&
                                                            x.ExtParamTypes.Skip(1).ToArray().IsAssignableFrom(paramTypes));
                })
                .Select(x => x.Method)
                .ToArray();
        }

        public static MethodInfo[] GetMethods(this Type source, Type returnType, Type[] paramTypes, BindingFlags flags, bool exactBinding)
        {
            return source.GetMethods(flags)
                .Where(m => m.ReturnType == returnType)
                .Select(m => new { Method = m, pTypes = m.GetParameters().Select(p => p.ParameterType).ToArray() })
                .Where(x =>
                {
                    if (paramTypes == null)
                        return true;

                    if (paramTypes == Type.EmptyTypes)
                        return x.pTypes.Length == 0;

                    bool equalLength = paramTypes.Length == x.pTypes.Length;
                    return equalLength && (exactBinding ?
                            x.pTypes.IsEqualTo(paramTypes) :
                            paramTypes.IsAssignableFrom(x.pTypes));
                })
                .Select(x => x.Method)
                .ToArray();
        }

        public static bool IsAssignableFrom(this Type[] to, Type[] from)
        {
            return to.Zip(from, (t, f) => t.IsAssignableFrom(f)).All(x => x);
        }

        /// <summary>
        /// Gets all the children types (abstract/concrete) of this type from the specified assembly
        /// Pass true to directlyUnder to only get the children that are directly under this type (i.e. no grandchildren)
        /// </summary>
        public static IEnumerable<Type> GetChildren(this Type type, Assembly from, bool directlyUnder = false)
        {
            return from.GetTypes().Where(t => t.IsA(type) && (!directlyUnder || t.BaseType == type)).Disinclude(type);
        }

        /// <summary>
        /// Gets all the children types (abstract/concrete) of this type from the specified assembly
        /// Pass true to directlyUnder to only get the children that are directly under this type (i.e. no grandchildren)
        /// </summary>
        public static IEnumerable<Type> GetChildren(this Type type, bool directlyUnder = false)
        {
            return GetChildren(type, type.Assembly, directlyUnder);
        }

        /// <summary>
        /// Gets all the concrete (non-abstract) children of this type from the specified assembly
        /// Pass true to directlyUnder to only get the children that are directly under this type (i.e. no grandchildren)
        /// </summary>
        public static IEnumerable<Type> GetConcreteChildren(this Type type, Assembly from, bool directlyUnder = false)
        {
            return GetChildren(type, from, directlyUnder).Where(c => !c.IsAbstract);
        }

        /// <summary>
        /// Gets all the concrete (non-abstract) children of this type from its own assembly
        /// Pass true to directlyUnder to only get the children that are directly under this type (i.e. no grandchildren)
        /// </summary>
        public static IEnumerable<Type> GetConcreteChildren(this Type type, bool directlyUnder = false)
        {
            return GetConcreteChildren(type, type.Assembly, directlyUnder);
        }

        public static Type[] GetConcreteChildren(this Type type, string[] dlls)
        {
            List<Type> allTypes = new List<Type>();
            foreach (var dll in dlls)
            {
                var types = type.GetConcreteChildren(Assembly.LoadFile(dll));
                foreach (var t in types)
                {
                    allTypes.Add(t);
                }
            }
            return allTypes.ToArray();
        }
    }
}
