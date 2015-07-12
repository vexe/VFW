using System;
using System.Collections.Generic;
using System.IO;

namespace BX20Serializer
{
    public class ListSerializer<T> : StrongSerializer<List<T>>
    {
        public readonly StrongWriter<T> WriteElement;
        public readonly StrongReader<T> ReadElement;

        public ListSerializer(StrongWriter<T> write, StrongReader<T> read)
        {
            this.WriteElement = write;
            this.ReadElement = read;
        }

        public override bool Handles(Type type)
        {
            return type == typeof(List<T>);
        }

        public override void StrongSerialize(Stream stream, List<T> value)
        {
            int count = value.Count;
            stream.WriteInt(count);
            for(int i = 0; i < count; i++)
            {
                T element = value[i];
                WriteElement(stream, element);
            }
        }

        public override void StrongDeserialize(Stream stream, ref List<T> instance)
        {
            int count = stream.ReadInt();
            if (instance == null)
            { 
                instance = new List<T>(count);
                ctx.Serializer.ReMark(instance);
            }
            else
                instance.Clear();

            for(int i = 0; i < count; i++)
            {
                T element = default(T);
                ReadElement(stream, ref element);
                instance[i] = element;
            }
        }
    }
}
