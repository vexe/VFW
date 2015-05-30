using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using UnityEngine;
using Vexe.Runtime.Extensions;
using Vexe.Runtime.Serialization;
using Vexe.Runtime.Types;
using UnityObject = UnityEngine.Object;

namespace Vexe.Runtime.Helpers
{
    public static class RuntimeHelper
    {
        public static string GetCallStack()
        {
            return GetCallStack(false);
        }

        public static string GetCallStack(bool verbose)
        {
            if (verbose)
            {
                return string.Join(" -> \r\n", new StackTrace()
                             .GetFrames()
                             .Select(x =>
                             {
                                 var method = x.GetMethod();
                                 return string.Format("{0}.{1}",
                                     method.DeclaringType.GetNiceName(),
                                     x.GetMethod().GetNiceName());
                             })
                             .ToArray());
            }

            return string.Join(" -> ", new StackTrace()
                         .GetFrames()
                         .Select(x => x.GetMethod().Name)
                         .ToArray());
        }

        public static void Swap<T>(ref T value0, ref T value1)
        {
            T tmp = value0;
            value0 = value1;
            value1 = tmp;
        }

        public static string GetCurrentSceneName()
        {
            string sceneName;

#if UNITY_EDITOR
            sceneName = Application.isPlaying ? Application.loadedLevelName : System.IO.Path.GetFileNameWithoutExtension(UnityEditor.EditorApplication.currentScene);
            if ( string.IsNullOrEmpty(sceneName) )
            {
                sceneName = "Unnamed";
            }
#else
            sceneName = Application.loadedLevelName;
#endif

            return sceneName;
        }

        public static int GetTargetID(object target)
        {
            var obj = target as IVFWObject;
            if (obj != null)
                return obj.GetPersistentId();

            return target.GetHashCode();
        }

        public static bool IsModified(UnityObject target, SerializerBackend serializer, SerializationData data)
        {
            var members = serializer.Logic.CachedGetSerializableMembers(target.GetType());
            for (int i = 0; i < members.Length; i++)
            {
                var member    = members[i];
                var memberKey = SerializerBackend.GetMemberKey(member);
                member.Target = target;
                var value = member.Value;

                string prevState;
                if (!data.serializedStrings.TryGetValue(memberKey, out prevState))
                    return true;

                if (value.IsObjectNull() && prevState == "null")
                    return true;

                string curState = serializer.Serialize(member.Type, value, data.serializedObjects);

                if (prevState != null && prevState != curState)
                    return true;
            }

            return false;
        }

        public static void ResetTarget(object target)
        {
            //Func<Type, bool> IsTypeSerializedByUnity = type =>
            //{
            //    // Might be forgetting something here...
            //    return (type.IsPrimitive || type.IsEnum || type == typeof(string)
            //        || type.IsA<UnityObject>()
            //        || VFWSerializationLogic.UnityStructs.ContainsValue(type)
            //        || type == typeof(AnimationCurve)
            //        || type == typeof(Gradient));
            //};

            //Func<FieldInfo, bool> IsFieldSerializedByUnity = field =>
            //{
            //    var type = field.FieldType;
            //    if (!IsTypeSerializedByUnity(type))
            //        return false;

            //    if (!field.IsPublic && !field.IsDefined<SerializeField>())
            //        return false;

            //    if (type.IsArray)
            //        return type.GetArrayRank() == 1 && IsTypeSerializedByUnity(type.GetElementType());

            //    if (type.IsGenericType)
            //        return type.GetGenericTypeDefinition() == typeof(List<>) && IsTypeSerializedByUnity(type.GetGenericArguments()[0]);

            //    if (type.IsAbstract)
            //        return false;

            //    return type.IsDefined<SerializableAttribute>();
            //};

            var members = RuntimeMember.CachedWrapMembers(target.GetType());
            for (int i = 0; i < members.Count; i++)
			{
				var member = members[i];
				member.Target = target;
				var defAttr = member.Info.GetCustomAttribute<DefaultAttribute>();
                if (defAttr == null)
                { 
                    // if a field is not serializable by Unity, then Unity won't be able to set it to whatever we initialized it with,
                    // so we will reset it to its default value and unfortunately ignore the field initialization because it's very
                    // hard to obtain that value
                    //var field = member.Info as FieldInfo;
                    //if (field != null && !IsFieldSerializedByUnity(field) || member.Info is PropertyInfo)
                    //    member.Value = member.Type.GetDefaultValue();
                }
				else
				{ 
					var value = defAttr.Value;
					if (value == null && !member.Type.IsAbstract) // null means to instantiate a new instance
						value = member.Type.ActivatorInstance();
					member.Value = value;
				}
			}
		}

        public static int CombineHashCodes<Item0, Item1>(Item0 item0, Item1 item1)
        {
            int hash = 17;
            hash = hash * 31 + item0.GetHashCode();
            hash = hash * 31 + item1.GetHashCode();
            return hash;
        }

        public static int CombineHashCodes<Item0, Item1, Item2>(Item0 item0, Item1 item1, Item2 item2)
        {
            int hash = 17;
            hash = hash * 31 + item0.GetHashCode();
            hash = hash * 31 + item1.GetHashCode();
            hash = hash * 31 + item2.GetHashCode();
            return hash;
        }

        public static void Measure(string msg, int nTimes, Action<string> log, Action code)
        {
            log(string.Format("Time took to `{0}` is `{1}` ms", msg, Measure(nTimes, code)));
        }

        public static float Measure(int nTimes, Action code)
        {
            var w = System.Diagnostics.Stopwatch.StartNew();
            for (int i = 0; i < nTimes; i++)
                code();
            return w.ElapsedMilliseconds;
        }

        public static Dictionary<T, U> CreateDictionary<T, U>(IEnumerable<T> keys, IList<U> values)
        {
            return keys.Select((k, i) => new { k, v = values[i] }).ToDictionary(x => x.k, x => x.v);
        }

        public static Texture2D GetTexture(byte r, byte g, byte b, byte a, HideFlags flags)
        {
            var texture = new Texture2D(1, 1);
            texture.SetPixel(0, 0, new Color32(r, g, b, a));
            texture.Apply();
            texture.hideFlags = flags;
            return texture;
        }

        public static Texture2D GetTexture(Color32 c, HideFlags flags)
        {
            return GetTexture(c.r, c.g, c.b, c.a, flags);
        }

        public static Texture2D GetTexture(Color32 c)
        {
            return GetTexture(c, HideFlags.None);
        }

        public static Color HexToColor(string hex)
        {
            byte r = byte.Parse(hex.Substring(0, 2), System.Globalization.NumberStyles.HexNumber);
            byte g = byte.Parse(hex.Substring(2, 2), System.Globalization.NumberStyles.HexNumber);
            byte b = byte.Parse(hex.Substring(4, 2), System.Globalization.NumberStyles.HexNumber);
            return new Color32(r, g, b, 255);
        }
    }
}
