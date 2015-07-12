//#define DBG

using System;
using System.IO;
using System.Linq;
using BX20Serializer;
using UnityEngine;
using UnityObject = UnityEngine.Object;

namespace Vexe.FastSave.Serializers
{
    /// <summary>
    /// The FullSerializer converter to use to serialize/deserialize asset references (Textures, Meshes, Materials, AudioClips, AnimationClips, etc)
    /// Instead of serializing the actual bytes of meshes, audio etc we just save thsoe references into a store object live in the scene.
    /// When loading, we just ask the store to give us the item.
    /// Note the use of the name of the assets meaning assets you want to save *must* have unique names
    /// </summary>
    public class AssetReferenceSerializer : StrongSerializer<UnityObject>
    {
        public override bool Handles(Type type)
        {
            var types = AssetStorage.SupportedTypes;
            for (int i = 0; i < types.Count; i++)
            {
                var supportedType = types[i];
                if (type == supportedType || type.IsA(supportedType))
                    return true;
            }
            return false;
        }

        public override void StrongSerialize(Stream stream, UnityObject value)
        {
            Write(stream, value);
        }

        public override void StrongDeserialize(Stream stream, ref UnityObject instance)
        {
            instance = Read(stream);
        }

        public static void Write(Stream stream, UnityObject asset)
        {
            if (asset == null)
            {
                Write(stream, string.Empty);
                return;
            }

            var name = AssetStorage.Current.Store(asset);
            Write(stream, name);

#if DBG
            Debug.Log("Wrote asset: " + asset);
#endif
        }

        public static UnityObject Read(Stream stream)
        {
            var name = stream.ReadString();
            var asset = AssetStorage.Current.Get(name);
#if DBG
            Debug.Log("Read asset: " + asset);
#endif
            return asset;
        }
    }
}
