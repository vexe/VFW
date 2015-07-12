using System;
using System.IO;

namespace BX20Serializer
{
    public class TypeSerializer : StrongSerializer<Type>
    {
        public override bool Handles(Type type)
        {
            return typeof(Type).IsAssignableFrom(type);
        }

        public override void StrongSerialize(Stream stream, Type type)
        {
            Write(stream, type);
        }

        public override void StrongDeserialize(Stream stream, ref Type value)
        {
            value = Read(stream);
        }

        public static void Write(Stream stream, Type type)
        {
            var name = type.AssemblyQualifiedName;
            Basic.WriteString(stream, name);
        }

        public static Type Read(Stream stream)
        {
            string name = Basic.ReadString(stream);
            return Type.GetType(name);
        }
    }
}
