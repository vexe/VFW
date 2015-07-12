using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Vexe.Runtime.Extensions;

namespace BX20Serializer
{
    public class DictionarySerializer : BaseSerializer
    {
        public override bool Handles(Type type)
        {
            return typeof(IDictionary).IsAssignableFrom(type);
        }

        void GetKVTypes(Type type, out Type keyType, out Type valueType)
        {
            var args = type.GetGenericArgsInRawParentInterface(typeof(IDictionary<,>));
            if (args == Type.EmptyTypes)
                keyType = valueType = typeof(object);
            else
            {
                keyType = args[0];
                valueType = args[1];
            }
        }

        public override void Serialize(Stream stream, Type type, object value)
        {
            Type keyType, valueType;
            GetKVTypes(type, out keyType, out valueType);

            var dictionary = value as IDictionary;
            Basic.WriteInt(stream, dictionary.Count);
            var iter = dictionary.GetEnumerator();
            while(iter.MoveNext())
            {
                ctx.Serializer.Serialize_Main(stream, keyType, iter.Key);
                ctx.Serializer.Serialize_Main(stream, valueType, iter.Value);
            }
        }

        public override void Deserialize(Stream stream, Type type, ref object instance)
        {
            Type keyType, valueType;
            GetKVTypes(type, out keyType, out valueType);

            int count = Basic.ReadInt(stream);
            var dictionary = instance as IDictionary;
            dictionary.Clear();
            for (int i = 0; i < count; i++)
            {
                object key = null;
                ctx.Serializer.Deserialize_Main(stream, keyType, ref key);
                object value = null;
                ctx.Serializer.Deserialize_Main(stream, valueType, ref value);
                dictionary.Add(key, value);
            }
        }

        public override object GetInstance(Stream stream, Type type)
        {
            var ctor = type.DelegateForCtor();
            return ctor(null);
        }
    }

    public class DictionarySerializer<TK, TV> : StrongSerializer<IDictionary<TK, TV>>
    {
        private readonly StrongWriter<TK> WriteKey;
        private readonly StrongWriter<TV> WriteValue;
        private readonly StrongReader<TK> ReadKey;
        private readonly StrongReader<TV> ReadValue;

        public DictionarySerializer(StrongWriter<TK> keyWriter, StrongReader<TK> keyReader, StrongWriter<TV> valueWriter, StrongReader<TV> valueReader)
        {
            this.WriteValue = valueWriter;
            this.ReadValue = valueReader;
            this.WriteKey = keyWriter;
            this.ReadKey = keyReader;
        }

        public override bool Handles(Type type)
        {
            return typeof(IDictionary<TK, TV>).IsAssignableFrom(type);
        }

        public override void StrongSerialize(Stream stream, IDictionary<TK, TV> value)
        {
            Basic.WriteInt(stream, value.Count);
            var iter = value.GetEnumerator();
            while(iter.MoveNext())
            {
                var current = iter.Current;
                WriteKey(stream, current.Key);
                WriteValue(stream, current.Value);
            }
        }

        public override void StrongDeserialize(Stream stream, ref IDictionary<TK, TV> instance)
        {
            instance.Clear();
            int count = Basic.ReadInt(stream);
            for (int i = 0; i < count; i++)
            {
                TK key = default(TK);
                ReadKey(stream, ref key);
                TV value = default(TV);
                ReadValue(stream, ref value);
                instance.Add(key, value);
            }
        }

        public override object GetInstance(Stream stream, Type type)
        {
            var ctor = type.DelegateForCtor();
            return ctor(null);
        }
    }
}
