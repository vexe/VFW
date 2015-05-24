//#define PROFILE

using System;
using System.IO;
using System.Linq;
using Vexe.Fast.Serialization;
using Vexe.Fast.Serializer;
using Vexe.Fast.Serializer.Serializers;
using Vexe.Runtime.Extensions;
using Vexe.Runtime.Helpers;

namespace Vexe.Runtime.Serialization
{
    public class ALPHA_FastSerializerBackend : SerializerBackend
    {
        public static readonly FastSerializer Serializer;

#if UNITY_EDITOR
        //[UnityEditor.MenuItem("Tools/Vexe/CompileAsm")]
        //public static void CompileAsm()
        //{
            //var types = ReflectionHelper.CachedGetRuntimeTypes().Concat(ReflectionHelper.GetUnityEngineTypes()).ToList();
            //var types = ReflectionHelper.CachedGetRuntimeTypes().ToList();
            //var serializer = new FastSerializer();
            //var adapters = new IBaseSerializer[] { new UnityObjectSerializer() };
            //serializer.CompileAssembly(types, adapters, new VFWPredicates(), "SerTest", "Assets", "SERIALIZER");
            //UnityEditor.AssetDatabase.ImportAsset("Assets/SerTest.dll", UnityEditor.ImportAssetOptions.ForceUpdate);
        //}
#endif

        static ALPHA_FastSerializerBackend()
        {
            var types = ReflectionHelper.CachedGetRuntimeTypes().ToList();
            var serializsers = new IBaseSerializer[] { new UnityObjectSerializer() };
            Serializer = FastSerializer.CompileDynamic(types, serializsers, new VFWPredicates());
        }

        public ALPHA_FastSerializerBackend()
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