//#define DBG

using System;
using System.IO;
using BX20Serializer;
using UnityEngine;
using Vexe.Runtime.Extensions;

namespace Vexe.FastSave.Serializers
{
    public class ReflectiveComponentSerializer : ReflectiveSerializer
    {
        public override bool Handles(Type type)
        {
            return typeof(Component).IsAssignableFrom(type);
        }

        public override void Serialize(Stream stream, Type type, object value)
        {
            var members = GetMembers(type);
            for (int i = 0; i < members.Length; i++)
            {
                var member = X20Member.WrapMember(members[i], value);

#if DBG
                Debug.Log("Serializing: " + member.Name);
#endif

                if (typeof(Component).IsAssignableFrom(member.Type))
                {
                    SerializeComponentReference(stream, member.Value as Component);
                }
                else if (member.Type == typeof(GameObject))
                {
                    SerializeGameObjectReference(stream, member.Value as GameObject);
                }
                else
                {
                    ctx.Serializer.Serialize_Main(stream, member.Type, member.Value);
                }
            }
        }

        public override void Deserialize(Stream stream, Type type, ref object instance)
        {
            var members = GetMembers(type);
            for (int i = 0; i < members.Length; i++)
            {
                var member = X20Member.WrapMember(members[i], instance);

#if DBG
                Debug.Log("Deserializing: " + member.Name);
#endif

                if (typeof(Component).IsAssignableFrom(member.Type))
                {
                    member.Value = DeserializeComponentReference(stream, member.Type);
                }
                else if (member.Type == typeof(GameObject))
                {
                    member.Value = DeserializeGameObjectReference(stream);
                }
                else
                {
                    var memberValue = member.Value;
                    ctx.Serializer.Deserialize_Main(stream, member.Type, ref memberValue);
                    member.Value = memberValue;
                }
            }
        }

        static void SerializeComponentReference(Stream stream, Component value)
        {
            if (value == null)
            {
                Basic.WriteByte(stream, 0);
                return;
            }

            var prefabId = PrefabStorage.Current.GetIndex(value.gameObject);
            if (prefabId != -1)
            {
                Basic.WriteByte(stream, 1);
                Write(stream, prefabId);
            }
            else
            {
                Basic.WriteByte(stream, 2);
                int refId = value.GetOrAddComponent<FSReference>().GetPersistentId();
                Write(stream, refId);
            }
        }

        static Component DeserializeComponentReference(Stream stream, Type type)
        {
            byte b = Basic.ReadByte(stream);
            if (b == 0)
                return null;

            if (b == 1)
            {
                var prefabId = stream.ReadInt();
                var go = PrefabStorage.Current.Get(prefabId);
                return go.GetComponent(type);
            }
            else
            {
                var refId = stream.ReadInt();
                var go = FSReference.Get<Component>(refId);
                return go.GetComponent(type);
            }
        }

        public static void SerializeGameObjectReference(Stream stream, GameObject value)
        {
            if (value == null)
            {
                Basic.WriteByte(stream, 0);
                return;
            }

            var prefabId = PrefabStorage.Current.GetIndex(value);
            if (prefabId != -1)
            {
                Basic.WriteByte(stream, 1);
                Write(stream, prefabId);
            }
            else
            {
                Basic.WriteByte(stream, 2);
                int refId = value.GetOrAddComponent<FSReference>().GetPersistentId();
                Write(stream, refId);
            }
        }

        public static GameObject DeserializeGameObjectReference(Stream stream)
        {
            var b = Basic.ReadByte(stream);
            if (b == 0)
                return null;

            if (b == 1)
            {
                var prefabId = stream.ReadInt();
                return PrefabStorage.Current.Get(prefabId);
            }
            else
            {
                var refId = stream.ReadInt();
                return FSReference.Get(refId);
            }
        }
    }
}
