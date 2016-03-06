using System;
using UnityEngine;
using Vexe.Runtime.Extensions;
using Vexe.Runtime.Types;
using UnityObject = UnityEngine.Object;

namespace Vexe.Runtime.Serialization
{
    public abstract class SerializerBackend
    {
        /// <summary>
        /// The default serializer backend type to use
        /// </summary>
        public static readonly Type DefaultType = typeof(FullSerializerBackend);

        /// <summary>
        /// The serialization logic that this serializer use to fetch the serializable members of a given target
        /// </summary>
        public ISerializationLogic Logic;

        /// <summary>
        /// Serializes the specified target and stores the result in the specified serialization data
        /// such that all Unity object references are stored in the data's serializedObjects list,
        /// and the serializable members' values in the data's serializedStrings
        /// </summary>
        public void SerializeTargetIntoData(UnityObject target, SerializationData data)
        {
            data.Clear();

            var members = VFWSerializationLogic.Instance.CachedGetSerializableMembers(target.GetType());
            for (int i = 0; i < members.Length; i++)
            {
                var member    = members[i];
                member.Target = target;
                var value     = member.Value;

                try
                {
                    string memberKey = GetMemberKey(member);
                    string serializedState = Serialize(member.Type, value, data.serializedObjects);
                    data.serializedStrings[memberKey] = serializedState;
                }
                catch (Exception e)
                {
                    Debug.LogError(
                        "Error serializing member `" + member.Name + "` (" + member.Type.Name + ")" +
                        " in `" + target.GetType().Name +
                        "` Error message: \"" + e.Message +
                        "\" Stacktrace: " + e.StackTrace);
                }
            }
        }

        /// <summary>
        /// Fetches the serialized state of the specified target from the specified serialization data
        /// to use it to deserialize/reload the target reassigning all the target's member values
        /// </summary>
        public void DeserializeTargetFromData(UnityObject target, SerializationData data)
        {
            var members = VFWSerializationLogic.Instance.CachedGetSerializableMembers(target.GetType());
            for(int i = 0; i < members.Length; i++)
            {
                var member    = members[i];
                var memberKey = GetMemberKey(member);
                member.Target = target;

                ConvertLegacyKeys(data);

                try
                {
                    string result;
                    if (data.serializedStrings.TryGetValue(memberKey, out result))
                    {
                        var value = Deserialize(member.Type, result, data.serializedObjects);
                        member.Value = value;
                    }
                }
                catch (Exception e)
                {
                    Debug.LogError(
                        "Error deserializing member `" + member.Name + "` (" + member.Type.Name + ")" +
                        " in `" + target.GetType().Name +
                        "` Error message: \"" + e.Message +
                        "\" Stacktrace: " + e.StackTrace);
                }
            }
        }

        private static void ConvertLegacyKeys(SerializationData data)
        {
            var keys = data.serializedStrings.Keys;
            for (int i = 0; i < keys.Count; i++)
            {
                var key = keys[i];
                key = key.Replace("Field: ", string.Empty);
                key = key.Replace("Property: ", string.Empty);
                keys[i] = key;
            }
        }

        private static Func<RuntimeMember, string> cachedGetMemberKey;

        /// <summary>
        /// Gets the serialization key used to serialize the specified member
        /// The key in general is: "TypeNiceName MemberName"
        /// Ex: "int someValue", "GameObject go"
        /// </summary>
        public static string GetMemberKey(RuntimeMember member)
        {
            if (cachedGetMemberKey == null)
                cachedGetMemberKey = new Func<RuntimeMember, string>(x => x.TypeNiceName + " " + x.Name).Memoize();
            return cachedGetMemberKey(member);
        }

        public abstract string Serialize(Type type, object value, object context);

        public string Serialize(object value, object context)
        {
            if (value == null)
                return null;

            return Serialize(value.GetType(), value, context);
        }
        
        public string Serialize(object value)
        {
            return Serialize(value, null);
        }

        public abstract object Deserialize(Type type, string serializedState, object context);

        public T Deserialize<T>(string serializedState, object context)
        {
            return (T)Deserialize(typeof(T), serializedState, context);
        }

        public T Deserialize<T>(string serializedState)
        {
            return Deserialize<T>(serializedState, null);
        }
    }
}
