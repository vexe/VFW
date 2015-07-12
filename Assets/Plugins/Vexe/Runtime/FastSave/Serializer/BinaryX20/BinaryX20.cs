//#define PROFILE
//#define DBG

using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

/*
* New serializer idea:
- FLEXIBLE (not rigid - can easily adapt to new types)
- EASY TO DEBUG
- uses reflection (the fast one)
- no more IL (except for reflection, that's fine...)
- binary, no parsing
- fast, robust, simple
- customizable (custom serialization logic/attributes etc)
- optional runtimetypeinfo/cycle handling
*/

namespace BX20Serializer
{
    public delegate void StrongReader<T>(Stream stream, ref T value);
    public delegate void StrongWriter<T>(Stream stream, T value);

    public delegate bool FieldPredicate(FieldInfo field);
    public delegate bool PropertyPredicate(PropertyInfo property);

    public class BinaryX20
    {
        public static Action<string> Log = delegate { };

        delegate void SerializeFunction(Stream stream, Type type, object value);
        delegate void DeserializeFunction(Stream stream, Type type, ref object instance);

        readonly List<BaseSerializer> _Serializers;
        readonly Dictionary<Type, BaseSerializer> _CachedSerializers;
        readonly SerializationContext _Ctx;
        readonly ReferenceMarker _Marker;

        private SerializeFunction _Serialize;
        private DeserializeFunction _Deserialize;

        private int _CurrentRefId;

        internal readonly X20Logic _Logic;

        /// <summary>
        /// 1: runtime type info (polymorphic serialization), cycle/serialize by reference, string cache
        /// 0: string cache
        /// </summary>
        public void SetMode(int value)
        {
            if (value == 0)
            {
                _Serialize = Serialize_Minimal;
                _Deserialize = Deserialize_Minimal;
            }
            else
            {
                _Serialize = Serialize_Standard;
                _Deserialize = Deserialize_Standard;
            }
        }

        public BinaryX20(FieldPredicate isSerializableField, PropertyPredicate isSerializableProperty)
        {
            _Logic = new X20Logic(isSerializableField, isSerializableProperty);

            _Serializers = new List<BaseSerializer>()
            {
                PrimitiveSerializer.Instance,
                new V2Serializer(),
                new V3Serializer(),
                new V4Serializer(),
                new BoundsSerializer(),
                new LayerMaskSerializer(),
                new RectSerializer(),
                new ArraySerializer(),
                new DictionarySerializer(),
                new EnumSerializer(),
                new CollectionSerializer(),
                new TypeSerializer(),
                new ReflectiveSerializer(),
            };

            _Ctx = new SerializationContext(this);
            _Marker = new ReferenceMarker();
            _CachedSerializers = new Dictionary<Type, BaseSerializer>();

            SetMode(1);
        }

        public BinaryX20 AddSerializer(BaseSerializer serializer)
        {
            _Serializers.Insert(0, serializer);
            return this;
        }

        public BinaryX20 AddStrongReflective<TTarget, TMember0>(string member0, StrongWriter<TMember0> writer, StrongReader<TMember0> reader)
        {
            _Serializers.Insert(0, new StrongReflectiveSerializer<TTarget, TMember0>(member0, writer, reader));
            return this;
        }

        public BinaryX20 AddStrongArray<T>(StrongWriter<T> write, StrongReader<T> read)
        {
            _Serializers.Insert(0, new ArraySerializer<T>(write, read));
            return this;
        }

        public BinaryX20 AddStrongList<T>(StrongWriter<T> write, StrongReader<T> read)
        {
            _Serializers.Insert(0, new ListSerializer<T>(write, read));
            return this;
        }

        public BinaryX20 AddStrongDictionary<TK, TV>(StrongWriter<TK> keyWriter, StrongReader<TK> keyReader, StrongWriter<TV> valueWriter, StrongReader<TV> valueReader)
        {
            _Serializers.Insert(0, new DictionarySerializer<TK, TV>(keyWriter, keyReader, valueWriter, valueReader));
            return this;
        }

        public void ClearSerializersCache()
        {
            _CachedSerializers.Clear();
        }

        private BaseSerializer GetSerializer(Type type)
        {
            BaseSerializer result;
            if (!_CachedSerializers.TryGetValue(type, out result))
            {
                for (int i = 0; i < _Serializers.Count; i++)
                {
                    var serializer = _Serializers[i];
                    if (serializer.Handles(type))
                    {
                        _CachedSerializers[type] = serializer;
#if DBG
                        Log("Cached serializer for type: " + type.Name + " is: " + serializer.GetType().Name);
#endif
                        result = serializer;
                        result.ctx = _Ctx;
                        break;
                    }
                }
            }
            return result;
        }

        public void Serialize(Stream stream, Type type, object value)
        {
            Serialize(stream, type, value, null);
        }

        public void Serialize(Stream stream, Type type, object value, object ctx)
        {
            _Ctx.Context = ctx;
            {
#if PROFILE
                Profiler.BeginSample("Binary: Serializing " + type.Name);
#endif
                Serialize_Main(stream, type, value);
#if PROFILE
                Profiler.EndSample();
#endif
            }
            _Marker.Clear();
            Basic.ClearMarkedStrings();
        }

        public void Serialize_Main(Stream stream, Type type, object value)
        {
            _Serialize(stream, type, value);
        }

        private void Serialize_Minimal(Stream stream, Type type, object value)
        {
            if (type.IsValueType)
            {
                // Primitives
                {
                    if (type.IsPrimitive || type == typeof(DateTime))
                    {
#if DBG
                        Log("Writing primitive: " + value + " (" + type.Name + ")");
#endif
                        PrimitiveSerializer.Instance.Serialize(stream, type, value);
                        return;
                    }
                }

                // Structs
                {
#if DBG
                    Log("Writing struct: " + value + " (" + type.Name + ")");
#endif
                    var serializer = GetSerializer(type);
                    serializer.Serialize(stream, type, value);
                    return;
                }
            }

            // Reference-types
            {
#if DBG
                Log("Writing reference type: " + type.Name);
#endif

                if (type == typeof(string))
                {
#if DBG
                    Log("Writing string: " + value);
#endif
                    Basic.WriteString(stream, value as string);
                    return;
                }

                if (value == null)
                {
                    // set null flag
#if DBG
                    Log("Setting null flag");
#endif
                    Basic.WriteByte(stream, 1);
                    return;
                }

#if DBG
                Log("Clearing null flag");
#endif
                Basic.WriteByte(stream, 0); // clear null flag

                var serializer = GetSerializer(type);
                serializer.Serialize(stream, type, value);
            }
        }

        private void Serialize_Standard(Stream stream, Type type, object value)
        {
            if (type.IsValueType)
            {
                // Primitives
                {
                    if (type.IsPrimitive || type == typeof(DateTime))
                    {
#if DBG
                        Log("Writing primitive: " + type.Name);
#endif
                        PrimitiveSerializer.Instance.Serialize(stream, type, value);
                        return;
                    }
                }

                // Structs
                {
#if DBG
                    Log("Writing struct: " + type.Name);
#endif
                    var serializer = GetSerializer(type);
                    serializer.Serialize(stream, type, value);
                    return;
                }
            }

            if (type == typeof(string))
            {
#if DBG
                Log("Writing string: " + value);
#endif
                Basic.WriteString(stream, value as string);
                return;
            }

            // Reference-types
            {
#if DBG
                Log("Writing refernece type: " + type.Name);
#endif
                if (value == null)
                {
#if DBG
                    Log("Setting null flag");
#endif
                    // set null flag
                    Basic.WriteByte(stream, 1);
                    return;
                }
#if DBG
                Log("Clearing null flag");
#endif
                Basic.WriteByte(stream, 0); // clear null flag

                int id = _Marker.GetRefId(value);
#if DBG
                Log("Writing reference id");
#endif
                Basic.WriteInt(stream, id);
                if (_Marker.IsReference(id))
                {
                    // set isref flag
#if DBG
                    Log("Setting isref flag");
#endif
                    Basic.WriteByte(stream, 1);
                    return;
                }
#if DBG
                Log("Clearing isref flag");
#endif
                Basic.WriteByte(stream, 0); // clear isref flag

                _Marker.Mark(value, id);

                var runtimeType = value.GetType();
                if (runtimeType != type)
                {
                    // set rti flag
#if DBG
                    Log("Setting rti flag. And writing runtime type: " + runtimeType);
#endif
                    Basic.WriteByte(stream, 1);
                    TypeSerializer.Write(stream, runtimeType);
                }
                else // clear rti flag
                {
#if DBG
                    Log("Clearing rti flag");
#endif
                    Basic.WriteByte(stream, 0);
                }

                var serializer = GetSerializer(runtimeType);
#if DBG
                Log("Serializing with: " + serializer.GetType().Name);
#endif
                serializer.Serialize(stream, runtimeType, value);
            }
        }

        public void Deserialize<T>(Stream stream, Type type, ref T instance)
        {
            object obj = instance;
            Deserialize(stream, type, ref obj);
            instance = (T)obj;
        }

        public T Deserialize<T>(Stream stream)
        {
            T obj = default(T);
            Deserialize(stream, typeof(T), ref obj);
            return obj;
        }

        public void Deserialize(Stream stream, Type type, ref object instance)
        {
            Deserialize(stream, type, ref instance, null);
        }

        public void Deserialize(Stream stream, Type type, ref object instance, object ctx)
        {
            _Ctx.Context = ctx;
            {
                Deserialize_Main(stream, type, ref instance);
            }
            _Marker.Clear();
            Basic.ClearMarkedStrings();
        }

        public void Deserialize_Main(Stream stream, Type type, ref object instance)
        {
            _Deserialize(stream, type, ref instance);
        }

        private void Deserialize_Minimal(Stream stream, Type type, ref object instance)
        {
            if (type.IsValueType)
            {
                // Primitives
                {
                    if (type.IsPrimitive || type == typeof(DateTime))
                    {
                        PrimitiveSerializer.Instance.Deserialize(stream, type, ref instance);
#if DBG
                        Log("Read primitive: " + instance);
#endif
                        return;
                    }
                }
                // Structs
                {
                    var serializer = GetSerializer(type);
                    if (instance == null)
                        instance = serializer.GetInstance(stream, type);
                    serializer.Deserialize(stream, type, ref instance);
#if DBG
                    Log("Read struct: " + instance);
#endif
                    return;
                }
            }

            // Reference-types
            {
                if (type == typeof(string))
                {
                    instance = stream.ReadString();
#if DBG
                    Log("Read string: " + instance);
#endif
                    return;
                }

                bool nullFlag = stream.ReadBool();
#if DBG
                Log("Read null flag: " + nullFlag);
#endif
                if (nullFlag)
                {
                    instance = null;
                    return;
                }

                var serializer = GetSerializer(type);
                if (instance == null)
                    instance = serializer.GetInstance(stream, type);
                serializer.Deserialize(stream, type, ref instance);
            }
        }

        private void Deserialize_Standard(Stream stream, Type type, ref object instance)
        {
            if (type.IsValueType)
            {
                // Primitives
                {
                    if (type.IsPrimitive || type == typeof(DateTime))
                    {
                        PrimitiveSerializer.Instance.Deserialize(stream, type, ref instance);
#if DBG
                        Log("Read primitive: " + instance + " (" + type.Name + ")");
#endif
                        return;
                    }
                }
                // Structs
                {
                    var serializer = GetSerializer(type);
                    if (instance == null)
                        instance = serializer.GetInstance(stream, type);
#if DBG
                    Log("Reading struct: " + type.Name + " with: " + serializer.GetType().Name);
#endif
                    serializer.Deserialize(stream, type, ref instance);
#if DBG
                    Log("struct value: " + instance);
#endif
                    return;
                }
            }

            if (type == typeof(string))
            {
                instance = stream.ReadString();
#if DBG
                Log("Read string: " + instance);
#endif
                return;
            }

            // Reference-types
            {
#if DBG
                Log("Reading reference type: " + type.Name);
#endif

                bool nullFlag = stream.ReadBool();
#if DBG
                Log("Read null flag: " + nullFlag);
#endif
                if (nullFlag)
                {
                    instance = null;
                    return;
                }

                int id = stream.ReadInt();
#if DBG
                Log("Read ref id (int): " + id);
#endif
                bool refFlag = stream.ReadBool();
#if DBG
                Log("Read ref flag (bool/byte): " + refFlag);
#endif
                if (refFlag)
                {
                    instance = _Marker.GetRef(id);
                    return;
                }

                Type runtimeType;
                bool typeInfoFlag = stream.ReadBool();
#if DBG
                Log("Read rti flag: " + typeInfoFlag);
#endif
                if (typeInfoFlag)
                    runtimeType = TypeSerializer.Read(stream);
                else
                    runtimeType = type;

                var serializer = GetSerializer(runtimeType);

                if (instance == null)
                {
#if DBG
                    Log("instance == null - Creating new instance (" + runtimeType + ")");
#endif
                    instance = serializer.GetInstance(stream, runtimeType);
                }
#if DBG
                Log("Marked instance: " + instance + " with id: " + id);
#endif
                _Marker.Mark(instance, id);

                _CurrentRefId = id;

                serializer.Deserialize(stream, runtimeType, ref instance);
            }
        }

        internal void ReMark(object instance)
        {
            _Marker.Mark(instance, _CurrentRefId);
        }

        public string SerializeToString(object value)
        {
            using (var ms = new MemoryStream())
            {
                Serialize(ms, value.GetType(), value, null);
                var bytes = ms.ToArray();
                var result = Convert.ToBase64String(bytes);
                return result;
            }
        }

        public T DeserializeFromString<T>(string serializedState)
        {
            var bytes = Convert.FromBase64String(serializedState);
            using (var ms = new MemoryStream(bytes))
            {
                T instance = default(T);
                Deserialize<T>(ms, typeof(T), ref instance);
                return instance;
            }
        }

        private class ReferenceMarker
        {
            private int _NextRefId;

            readonly Dictionary<int, object> _Marked = new Dictionary<int, object>();
            readonly Dictionary<object, int> _Ids = new Dictionary<object, int>(RefComparer.Instance);

            public object GetRef(int id)
            {
                return _Marked[id];
            }

            public bool IsReference(int objId)
            {
                return _Marked.ContainsKey(objId);
            }

            public void Mark(object obj, int id)
            {
                _Marked[id] = obj;
            }

            public int GetRefId(object obj)
            {
                int id;
                if (!_Ids.TryGetValue(obj, out id))
                {
                    id = _NextRefId++;
                    _Ids[obj] = id;
                }
                return id;
            }

            public void Clear()
            {
                _Marked.Clear();
                _Ids.Clear();
                _NextRefId = 0;
            }

            private class RefComparer : IEqualityComparer<object>
            {
                public static readonly RefComparer Instance = new RefComparer();

                private RefComparer()
                {
                }

                new public bool Equals(object x, object y)
                {
                    return x == y;
                }

                public int GetHashCode(object obj)
                {
                    return System.Runtime.CompilerServices.RuntimeHelpers.GetHashCode(obj);
                }
            }
        }

        public class SerializationContext
        {
            public readonly BinaryX20 Serializer;

            public object Context;

            public SerializationContext(BinaryX20 serializer)
            {
                this.Serializer = serializer;
            }
        }

    }
}
