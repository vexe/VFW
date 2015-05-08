//#define PROFILE

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Vexe.Fast.Serialization;
using Vexe.Fast.Serializer;
using Vexe.Fast.Serializer.Serializers;
using Vexe.Runtime.Extensions;
using Vexe.Runtime.Helpers;
using Vexe.Runtime.Types;

namespace Vexe.Runtime.Serialization
{
    public class FastSerializerBackend : SerializerBackend
    {
        public static readonly FastSerializer Serializer;

#if UNITY_EDITOR
        [UnityEditor.MenuItem("Tools/Vexe/CompileAsm")]
        public static void CompileAsm()
        {
            //var types = ReflectionHelper.CachedGetRuntimeTypes().Concat(ReflectionHelper.GetUnityEngineTypes()).ToList();
            //var types = ReflectionHelper.CachedGetRuntimeTypes().ToList();
            //var serializer = new FastSerializer();
            //var adapters = new IBaseSerializer[] { new UnityObjectSerializer() };
            //serializer.CompileAssembly(types, adapters, new VFWPredicates(), "SerTest", "Assets", "SERIALIZER");
            //UnityEditor.AssetDatabase.ImportAsset("Assets/SerTest.dll", UnityEditor.ImportAssetOptions.ForceUpdate);
        }
#endif

        static FastSerializerBackend()
        {
            //var types = ReflectionHelper.CachedGetRuntimeTypes().ToList();
            var types = new List<Type>() { typeof(string[]) };
            var adapters = new IBaseSerializer[] { new UnityObjectSerializer() };
            //Serializer = FastSerializer.BindSerializer(typeof(GeneratedSerializer));
            Serializer = FastSerializer.CompileDynamic(types, adapters, new VFWPredicates());
            //Serializer.CompileAssembly(types, adapters, new VFWPredicates(), "SerTest", null, "Serializer");
        }

        public FastSerializerBackend()
        {
            Logic = VFWSerializationLogic.Instance;
        }

        class VFWPredicates : ISerializationPredicates
        {
            public bool IsSerializableField(System.Reflection.FieldInfo field)
            {
                return VFWSerializationLogic.Instance.IsSerializableField(field);
            }

            public bool IsSerializableProperty(System.Reflection.PropertyInfo property)
            {
                return VFWSerializationLogic.Instance.IsSerializableProperty(property);
            }

            public bool IsSerializableType(Type type)
            {
                return VFWSerializationLogic.Instance.IsSerializableType(type);
            }
        }

        public override string Serialize(Type type, object graph, object context)
        {
            using (var stream = new MemoryStream())
            {
                Serializer.Serialize(stream, graph, context);
                return stream.ToArray().GetString();
            }
        }

        public override object Deserialize(Type type, string serializedState, object context)
        {
            var bytes = serializedState.GetBytes();
            using (var stream = new MemoryStream(bytes))
            {
                object result = Serializer.Deserialize(stream, context);
                return result;
            }
        }
    }
}

public static class GeneratedSerializer
{
    private static readonly Dictionary<Type, int> TypeLookup;
    static GeneratedSerializer()
    {
        GeneratedSerializer.TypeLookup = new Dictionary<Type, int>(2);
        GeneratedSerializer.TypeLookup.Add(typeof(string[]), 1);
        GeneratedSerializer.TypeLookup.Add(typeof(string), 2);
    }
    public static void Serialize(Stream stream, object value, SerializationContext ctx)
    {
        int objId = GeneratedSerializer.GetObjId(value);
        Basic.WriteInt32(stream, objId);
        switch (objId)
        {
            case 0:
                return;
            case 1:
                GeneratedSerializer.Write_StringArray1(stream, (string[])value, ctx);
                return;
            case 2:
                GeneratedSerializer.Write_String(stream, (string)value, ctx);
                return;
            default:
                throw new Exception("Unknown id: " + objId);
        }
    }
    public static void Write_StringArray1(Stream stream, string[] value, SerializationContext ctx)
    {
        if (value == null)
        {
            Basic.WriteInt32(stream, -1);
            return;
        }
        Basic.WriteInt32(stream, value.Length);
        int refId = ctx.Cache.GetRefId(value);
        Basic.WriteInt32(stream, refId);
        if (ctx.Cache.IsRefMarked(refId))
        {
            Basic.WriteBoolean(stream, true);
            return;
        }
        Basic.WriteBoolean(stream, false);
        ctx.Cache.MarkRef(value, refId);
        for (int i = 0; i < value.Length; i++)
        {
            GeneratedSerializer.Write_String(stream, value[i], ctx);
        }
    }
    private static int GetObjId(object obj)
    {
        if (obj == null)
        {
            return 0;
        }
        Type type = obj.GetType();
        int result;
        if (GeneratedSerializer.TypeLookup.TryGetValue(type, out result))
        {
            return result;
        }
        Dictionary<Type, int>.KeyCollection.Enumerator enumerator = GeneratedSerializer.TypeLookup.Keys.GetEnumerator();
        while (enumerator.MoveNext())
        {
            Type current = enumerator.Current;
            if (current.IsAssignableFrom(type))
            {
                return GeneratedSerializer.TypeLookup[current];
            }
        }
        throw new Exception("No id for type: " + type.Name);
    }
    public static object Deserialize(Stream stream, SerializationContext ctx)
    {
        int num = Basic.ReadInt32(stream);
        switch (num)
        {
            case 0:
                return null;
            case 1:
                return GeneratedSerializer.Read_StringArray1(stream, ctx);
            case 2:
                return GeneratedSerializer.Read_String(stream, ctx);
            default:
                throw new Exception("Unknown id: " + num);
        }
    }
    public static string[] Read_StringArray1(Stream stream, SerializationContext ctx)
    {
        int num = Basic.ReadInt32(stream);
        if (num == -1)
        {
            return null;
        }
        int num2 = Basic.ReadInt32(stream);
        bool flag = Basic.ReadBoolean(stream);
        if (flag)
        {
            return (string[])ctx.Cache.GetRef(num2);
        }
        string[] array = new string[num];
        ctx.Cache.MarkRef(array, num2);
        for (int i = 0; i < array.Length; i++)
        {
            array[i] = GeneratedSerializer.Read_String(stream, ctx);
        }
        return array;
    }

    // called by serializer. shouldn't be touched by user
    public static void Write_String(Stream stream, string value, SerializationContext ctx)
    {
        //WriteString(stream, value);
        //return;

        if (value == null)
        {
            Basic.WriteInt32(stream, -1);
            return;
        }

        int strId = ctx.Cache.GetStrId(value);

        Basic.WriteInt32(stream, strId);

        if (ctx.Cache.IsStrMarked(strId))
        {
            Basic.WriteBoolean(stream, true);
            return;
        }

        Basic.WriteBoolean(stream, false);

        ctx.Cache.MarkStr(value, strId);

        WriteStringDirect(stream, value);
    }

    private static void WriteStringDirect(Stream stream, string value)
    {
        if (value.Length == 0)
        {
            Basic.WriteInt32(stream, 0);
            return;
        }

        var byteCount = Encoding.Unicode.GetByteCount(value);
        Basic.WriteInt32(stream, byteCount);
        Encoding.Unicode.GetBytes(value, 0, value.Length, _buffer, 0);
        stream.Write(_buffer, 0, byteCount);
    }

    // called by serializer. shouldn't be touched by user
    public static string Read_String(Stream stream, SerializationContext ctx)
    {
        //return ReadString(stream);

        int refId = Basic.ReadInt32(stream);
        if (refId == -1)
            return null;

        bool isMarked = Basic.ReadBoolean(stream);
        if (isMarked)
            return ctx.Cache.GetStr(refId);

        int byteCount = Basic.ReadInt32(stream);
        string result = ReadStringDirect(stream, byteCount);
        ctx.Cache.MarkStr(result, refId);
        return result;
    }

    private static string ReadStringDirect(Stream stream, int byteCount)
    {
        if (byteCount == 0)
            return string.Empty;

        stream.Read(_buffer, 0, byteCount);
        var result = Encoding.Unicode.GetString(_buffer, 0, byteCount);
        return result;
    }

    static byte[] _buffer = new byte[1024];
}

