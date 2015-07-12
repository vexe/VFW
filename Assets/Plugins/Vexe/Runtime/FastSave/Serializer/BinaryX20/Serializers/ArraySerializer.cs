using System;
using System.Collections;
using System.IO;

namespace BX20Serializer
{
    public class ArraySerializer : BaseSerializer
    {
        public override bool Handles(Type type)
        {
            return type.IsArray && type.GetArrayRank() == 1;
        }

        public override void Serialize(Stream stream, Type type, object value)
        {
            var array = value as Array;
            var elementType = type.GetElementType();
            stream.WriteInt(array.Length);
            for (int i = 0; i < array.Length; i++)
                ctx.Serializer.Serialize_Main(stream, elementType, array.GetValue(i));
        }

        public override void Deserialize(Stream stream, Type type, ref object instance)
        {
            int count = stream.ReadInt();
            if (instance == null || ((Array)instance).Length != count)
            {
                instance = Array.CreateInstance(type.GetElementType(), count);
                ctx.Serializer.ReMark(instance);
            }

            var array = instance as IList;
            var elementType = type.GetElementType();
            for (int i = 0; i < count; i++)
            {
                object element = null;
                ctx.Serializer.Deserialize_Main(stream, elementType, ref element);
                array[i] = element;
            }
        }
    }

    public class ArraySerializer<T> : StrongSerializer<T[]>
    {
        public readonly StrongWriter<T> WriteElement;
        public readonly StrongReader<T> ReadElement;

        public ArraySerializer(StrongWriter<T> write, StrongReader<T> read)
        {
            this.WriteElement = write;
            this.ReadElement = read;
        }

        public override bool Handles(Type type)
        {
            return type.IsArray && type.GetArrayRank() == 1 && type.GetElementType() == typeof(T);
        }

        public override void StrongSerialize(Stream stream, T[] value)
        {
            stream.WriteInt(value.Length);
            for (int i = 0; i < value.Length; i++)
                WriteElement(stream, value[i]);
        }

        public override void StrongDeserialize(Stream stream, ref T[] instance)
        {
            int count = stream.ReadInt();
            if (instance == null || instance.Length != count)
            {
                instance = new T[count];
                ctx.Serializer.ReMark(instance);
            }

            for (int i = 0; i < instance.Length; i++)
            {
                T element = default(T);
                ReadElement(stream, ref element);
                instance[i] = element;
            }
        }
    }
}
