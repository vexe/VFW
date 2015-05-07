//#define PROFILE

using System;
using System.IO;
using System.Linq;
using Vexe.Fast.Serializer.Serializers;
using Vexe.Fast.Serializer;
using Vexe.Runtime.Extensions;
using Vexe.Runtime.Helpers;
using Vexe.Fast.Serialization;

namespace Vexe.Runtime.Serialization
{
    public class FastSerializerBackend : SerializerBackend
    {
        public readonly FastSerializer Serializer;

        [UnityEditor.MenuItem("Tools/Vexe/CompileAsm")]
        public static void CompileAsm()
        {
            //var types = ReflectionHelper.CachedGetRuntimeTypes().Concat(ReflectionHelper.GetUnityEngineTypes()).ToList();
            var types = ReflectionHelper.CachedGetRuntimeTypes().ToList();
            var serializer = new FastSerializer();
            var adapters = new IBaseSerializer[] { new UnityObjectSerializer() };
            serializer.CompileAssembly(types, adapters, new VFWPredicates(), "SerTest", "Assets", "SERIALIZER");
            UnityEditor.AssetDatabase.ImportAsset("Assets/SerTest.dll", UnityEditor.ImportAssetOptions.ForceUpdate);
        }

        public FastSerializerBackend()
        {
            var types = ReflectionHelper.CachedGetRuntimeTypes().ToList();
            Serializer = new FastSerializer();
            var adapters = new IBaseSerializer[] { new UnityObjectSerializer() };
            Logic = VFWSerializationLogic.Instance;
            Serializer.CompileDynamic(types, adapters, new VFWPredicates());
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
