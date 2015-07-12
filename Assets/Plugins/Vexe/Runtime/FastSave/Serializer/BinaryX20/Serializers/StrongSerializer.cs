using System;
using System.IO;

namespace BX20Serializer
{
    public abstract class StrongSerializer<T> : BaseSerializer
    {
        public override void Serialize(Stream stream, Type type, object value)
        {
            StrongSerialize(stream, (T)value);
        }

        public override void Deserialize(Stream stream, Type type, ref object instance)
        {
            T result;
            if (instance == null)
                result = default(T);
            else
                result = (T)instance;
            StrongDeserialize(stream, ref result);
            instance = result;
        }

        public override bool Handles(Type type)
        {
            return type == typeof(T);
        }

        public abstract void StrongSerialize(Stream stream, T value);
        public abstract void StrongDeserialize(Stream stream, ref T instance);
    }
}
