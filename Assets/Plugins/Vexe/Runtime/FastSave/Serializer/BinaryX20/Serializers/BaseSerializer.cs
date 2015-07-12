using System;
using System.IO;

namespace BX20Serializer
{
    public abstract class BaseSerializer
    {
        public BinaryX20.SerializationContext ctx;

        public abstract bool Handles(Type type);

        public abstract void Serialize(Stream stream, Type type, object value);

        public abstract void Deserialize(Stream stream, Type type, ref object instance);

        public virtual object GetInstance(Stream stream, Type type)
        {
            return null;
        }

        public static void Write(Stream stream, int value)
        {
            Basic.WriteInt(stream, value);
        }

        public static void Write(Stream stream, string value)
        {
            Basic.WriteString(stream, value);
        }

        public static void Write(Stream stream, float value)
        {
            Basic.WriteFloat(stream, value);
        }

        public static void Write(Stream stream, bool value)
        {
            Basic.WriteBool(stream, value);
        }
    }
}
