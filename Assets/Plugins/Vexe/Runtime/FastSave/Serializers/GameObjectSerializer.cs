using System.IO;
using BX20Serializer;
using UnityEngine;

namespace Vexe.FastSave.Serializers
{
    public static class GameObjectSerializer
    {
        public static void Write(Stream stream, GameObject instance)
        {
            stream.WriteString(instance.name);
            stream.WriteString(instance.tag);
            stream.WriteInt(instance.layer);
            stream.WriteBool(instance.isStatic);
            stream.WriteBool(instance.activeSelf);
        }

        public static void Read(Stream stream, GameObject instance)
        {
            instance.name = stream.ReadString();
            instance.tag = stream.ReadString();
            instance.layer = stream.ReadInt();
            instance.isStatic = stream.ReadBool();
            instance.SetActive(stream.ReadBool());
        }
    }
}
